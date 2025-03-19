using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Models.SystemKeyboardCommand;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Systems.Hid
{
    public class SystemHidInteractor : Stoppable, ISystemHidInteractor
    {
        private string[] _lastKeys;

        private readonly ILogger _logger;
        private readonly ISystemHidAdapter _hidAdapter;
        private readonly ISystemModifierDelegate _modifierDelegate;
        private readonly ISystemKeyboardCommandHandler _keyboardCommandHandler;
        private readonly CancellationTokenSource _hookKeyboardCancellationTokenSource;

        private readonly object _mutexKey = new object();
        private readonly BlockingCollection<string[]> _queue = new BlockingCollection<string[]>();
        private readonly IList<string> _invalidKeys = new List<string>();
        private readonly SemaphoreSlim _inputMgrSemaphore = new SemaphoreSlim(0, 1);
        private readonly CancellationTokenSource _exitCts = new CancellationTokenSource();
        private List<SystemHidKey> _keysToPlay = new List<SystemHidKey>();

        public SystemHidInteractor(ILoggerFactory loggerFactory, ISystemHidAdapter hidAdapter, ISystemModifierDelegate modifierDelegate, ISystemKeyboardCommandHandler commandHandler)
        {
            _logger = loggerFactory.CreateLogger<SystemHidInteractor>();
            _hidAdapter = hidAdapter ?? throw new ArgumentNullException(nameof(hidAdapter));
            _modifierDelegate = modifierDelegate ?? throw new ArgumentNullException(nameof(modifierDelegate));
            _keyboardCommandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _hookKeyboardCancellationTokenSource = new CancellationTokenSource();
        }

        public override void Stop()
        {
            _exitCts.Cancel();
            _hookKeyboardCancellationTokenSource.Cancel();

            base.Stop();
        }

        public void PostHidStringKeys(string[] keys)
        {
            bool bUpdated = false;

            var myKeyList = new List<string>(keys);
            var myKeys = new List<SystemHidKey>();

            foreach (var key in _invalidKeys.ToList())
            {
                if (!keys.Contains(key))
                {
                    _invalidKeys.Remove(key);
                }
            }

            // Simplify keys
            var i = 1;
            while (i < myKeyList.Count)
            {
                var currentChar = myKeyList[i - 1];

                if (_invalidKeys.Contains(currentChar))
                {
                    myKeyList.RemoveAt(i - 1);

                    var nKey = new SystemHidKey(currentChar);

                    myKeys.Add(nKey);
                }
                else if (_modifierDelegate.IsModifierKey(currentChar))
                {
                    ++i;

                    var nKey = new SystemHidKey(currentChar);

                    myKeys.Add(nKey);
                }
                else
                {
                    _invalidKeys.Add(currentChar);

                    var nKey = new SystemHidKey(currentChar);

                    myKeys.Add(nKey);
                    myKeyList.RemoveAt(i - 1);
                }
            }

            if (myKeyList.Count == 1 && _invalidKeys.Contains(myKeyList[0]))
            {
                var nKey = new SystemHidKey(myKeyList[0]);

                myKeys.Add(nKey);
                myKeyList.Clear();
            }
            else
            {
                foreach (var key in myKeyList)
                {
                    if (!myKeys.Select(x => x.Data).Contains(key))
                    {
                        var nKey = new SystemHidKey(key, true);

                        myKeys.Add(nKey);
                    }
                }
            }

            lock (_mutexKey)
            {
                if (_lastKeys != null && !_lastKeys.SequenceEqual(myKeyList))
                {
                    _lastKeys = myKeyList.ToArray();
                    _keysToPlay = myKeys;
                    bUpdated = true;
                }
            }

            if (bUpdated)
            {
                try
                {
                    if (_inputMgrSemaphore.CurrentCount != 1)
                    {
                        _inputMgrSemaphore.Release();
                    }
                }
                catch (SemaphoreFullException exception)
                {
                    _logger.LogError(exception, "Release keys");
                }
            }

            _queue.Add(keys);
        }

        public void Run() => Initialize();

        public void SystemLayoutChanged(OsLayoutId id) => _hidAdapter.SystemLayoutChanged(id);

        public void StopSendKey()
        {
            _lastKeys = new string[0];
            _hidAdapter.ReleaseKeys();
            ClearQueue();
        }

        private void Initialize()
        {
            _hidAdapter.Init();

            new Task(HookKeyboard, _hookKeyboardCancellationTokenSource.Token).Start();
        }

        private void HookKeyboard()
        {
            var keys = new string[0];
            var bRepeatStarted = false;

            while (!_exitCts.Token.IsCancellationRequested)
            {
                var timeout = NemeioConstants.NemeioDefaultSpeed;
                if (keys?.Length == 0)
                {
                    timeout = -1;
                }
                else if (!bRepeatStarted)
                {
                    timeout = NemeioConstants.NemeioDefaultDelay;
                }

                try
                {
                    bRepeatStarted = !_inputMgrSemaphore.Wait(timeout, _exitCts.Token);
                }
                catch (OperationCanceledException)
                {
                    continue;
                }

                List<SystemHidKey> mKeys;
                lock (_mutexKey)
                {
                    keys = _lastKeys;
                    mKeys = _keysToPlay;
                }

                if (keys?.Length > 0)
                {
                    var handled = _keyboardCommandHandler.Handle(mKeys.Select(x => x.Data).ToList());
                    if (!handled)
                    {
                        _hidAdapter.ExecuteKeys(mKeys);
                    }
                }
                else
                {
                    _hidAdapter.ReleaseKeys();
                    _invalidKeys.Clear();
                }
            }
        }

        private void ClearQueue()
        {
            while (_queue.Count > 0)
            {
                _queue.Take();
            }
        }
    }
}
