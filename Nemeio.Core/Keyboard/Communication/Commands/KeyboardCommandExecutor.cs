using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Stoppable;
using Monitor = Nemeio.Core.Keyboard.Monitors.Monitor;

namespace Nemeio.Core.Keyboard.Communication.Commands
{
    public class KeyboardCommandExecutor : AsyncStoppable, IKeyboardCommandExecutor
    {
        private BlockingCollection<IKeyboardCommand> _commands;
        private readonly IDictionary<CommandId, IList<Monitor>> _notificationMap;
        private readonly IKeyboardIO _keyboardIO;
        private readonly ILogger _logger;
        private readonly IFrameParser _frameParser;
        private byte[] _dataToRead;
        private CancellationTokenSource _cancellation;
        private Thread _writeCommandThread;
        private string _keyboardIdentifier;
        private SemaphoreSlim _writeSemaphore;
        private SemaphoreSlim _connectionSemaphore;

        private static object _lockReadData = new object();

        private IKeyboardCommand _currentCommandExecution;

        public KeyboardCommandExecutor(ILoggerFactory loggerFactory, IKeyboardIO keyboardIO, IFrameParser frameParser, string identifier)
            : base(false)
        {
            _dataToRead = new byte[0];
            _commands = new BlockingCollection<IKeyboardCommand>();
            _logger = loggerFactory.CreateLogger<KeyboardCommandExecutor>();
            _notificationMap = new Dictionary<CommandId, IList<Monitor>>();
            _frameParser = frameParser ?? throw new ArgumentNullException(nameof(frameParser));
            _keyboardIdentifier = identifier;
            _writeSemaphore = new SemaphoreSlim(0, 1);
            _connectionSemaphore = new SemaphoreSlim(1, 1);

            _keyboardIO = keyboardIO ?? throw new ArgumentNullException(nameof(keyboardIO));
            _keyboardIO.OnDataReceived += KeyboardIO_OnDataReceived;
        }

        public override async Task StopAsync()
        {
            if (Started)
            {
                await _connectionSemaphore.WaitAsync();

                AliveState = AliveState.Stopping;

                try
                {
                    _logger.LogInformation($"Stopping command executor");

                    _cancellation.Cancel();
                    _keyboardIO.OnDataReceived -= KeyboardIO_OnDataReceived;

                    IKeyboardCommand command;

                    if (_currentCommandExecution != null)
                    {
                        _commands.Add(_currentCommandExecution);

                        _currentCommandExecution = null;
                    }

                    while (_commands.TryTake(out command))
                    {
                        if (_notificationMap.ContainsKey(command.CommandId))
                        {
                            var monitors = _notificationMap[command.CommandId];

                            foreach (var monitor in monitors)
                            {
                                _logger.LogTrace($"We notify <{monitor.GetType().Name}> that command executor stopped");

                                monitor.ReceiveResponse(null, new ExecutorStoppedException());
                            }
                        }
                    }

                    await _keyboardIO.DisconnectAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Stop KeyboardCommentExecutor failed");
                }
                finally
                {
                    AliveState = AliveState.Stopped;

                    RaiseStop();

                    _connectionSemaphore.Release();
                }
            }
            else
            {
                if (_keyboardIO != null)
                {
                    _keyboardIO.OnDataReceived -= KeyboardIO_OnDataReceived;

                    await _keyboardIO.DisconnectAsync();
                }
                
                RaiseStop();
            }
        }

        public async Task Initialize()
        {
            await _connectionSemaphore.WaitAsync();

            AliveState = AliveState.Starting;

            try
            {
                await _keyboardIO.ConnectAsync(_keyboardIdentifier);

                StartCommandThreadIfNeeded();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"KeyboardCommandExecutor Initialize method failed");

                await StopAsync();
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        public bool ScheduleCommand(IKeyboardCommand command)
        {
            if (command == null)
            {
                return false;
            }

            if (!Started)
            {
                return false;
            }

            _commands.Add(command);

            return true;
        }

        public void RegisterNotification(CommandId commandId, Monitor monitor)
        {
            if (_notificationMap.ContainsKey(commandId))
            {
                var monitors = _notificationMap[commandId];

                monitors.Add(monitor);
            }
            else
            {
                var monitors = new List<Monitor>() { monitor };

                _notificationMap.Add(commandId, monitors);
            }
        }

        private void StartCommandThreadIfNeeded()
        {
            var threadExists = _writeCommandThread != null;
            var threadIsAlive = threadExists && _writeCommandThread.IsAlive;

            var mustStart = !threadExists || !threadIsAlive;

            if (mustStart)
            {
                StartCommandThread();
            }
        }

        private void StartCommandThread()
        {
            _cancellation = new CancellationTokenSource();
            _writeCommandThread = new Thread(Write)
            {
                Name = "KeyboardCommandExecutor:Write"
            };
            _writeCommandThread.IsBackground = true;
            _writeCommandThread.Start();

            AliveState = AliveState.Started;
        }

        private void Write()
        {
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    _currentCommandExecution = _commands.Take(_cancellation.Token);

                    var frames = _currentCommandExecution.ToFrames();

                    foreach (var frame in frames)
                    {
                        _keyboardIO.Write(frame.Bytes, 0, frame.Bytes.Length);

                        var releaseFromTimeout = _writeSemaphore.Wait(_currentCommandExecution?.Timeout ?? new TimeSpan(0, 0, 1), _cancellation.Token);
                        if (!releaseFromTimeout)
                        {
                            SendNotification(frame, new CommunicationTimeoutException());
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Try to take next command when command executor stopping");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"KeyboardCommandExecutor Write method failed");
            }
            finally
            {
                //  Don't need to be awaited
                _ = StopAsync();
            }
        }

        private IList<IFrame> Read()
        {
            var bytes = new byte[_keyboardIO.BytesToRead];

            _keyboardIO.Read(bytes, 0, bytes.Length);

            _dataToRead = _dataToRead.Append(bytes);

            var frames = new List<IFrame>();
            try
            {
                frames = _frameParser.FromByteArray(_dataToRead);

                _dataToRead = new byte[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                _logger.LogError($"SerialPortComm.Read with ArgumentOutOfRangeException {CoreHelpers.TraceBuffer(_dataToRead)}");
            }
            catch (InvalidFrameException exception)
            {
                _logger.LogError(exception, $"Frame is invalid");
            }

            return frames;
        }

        private void KeyboardIO_OnDataReceived(object sender, EventArgs e)
        {
            lock(_lockReadData)
            {
                if (_cancellation.IsCancellationRequested)
                {
                    //  Cancel has been asked
                    //  We never respond to monitor

                    return;
                }

                var frames = Read();

                foreach (var frame in frames)
                {
                    SendNotification(frame, null);

                    try
                    {
                        //  We not relase semaphore if it's a notification
                        if (_currentCommandExecution != null &&
                            _currentCommandExecution.CommandId == frame.CommandId &&
                            _currentCommandExecution.CommandId != CommandId.LayoutIds)
                        {
                            _writeSemaphore.Release();
                        }
                        //  LayoutIds is a special command
                        //  Continue to send data without asking anything
                        //  TODO: KSB https://adeneo-embedded.atlassian.net/browse/BLDLCK-3143
                        else if (_currentCommandExecution != null &&
                                _currentCommandExecution.CommandId == CommandId.LayoutIds &&
                                _writeSemaphore.CurrentCount > 0)
                        {
                            _writeSemaphore.Release();
                        }
                    }
                    catch (SemaphoreFullException exception)
                    {
                        _logger.LogError(exception, $"CommandId=<{frame.CommandId}>");
                    }
                }
            }
        }

        private void SendNotification(IFrame frame, Exception exception)
        {
            //  We search if a monitor is registered for this command id
            if (_notificationMap.ContainsKey(frame.CommandId))
            {
                var monitors = _notificationMap[frame.CommandId];

                foreach (var monitor in monitors)
                {
                    monitor.ReceiveResponse(frame, exception);
                }
            }
        }
    }
}
