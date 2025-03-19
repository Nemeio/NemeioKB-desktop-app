using System;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;

namespace Nemeio.Core.Keyboard.Nemeios.Proxy
{
    public abstract class KeyboardProxy : IKeyboard, IStateHolder
    {
        private readonly IStateHolder _stateHolder;
        private readonly IKeyboard _keyboard;

        public string Identifier { get; private set; }
        public NemeioState State => _stateHolder.State;

        public bool Started => _keyboard.Started;
        public CommunicationType CommunicationType => _keyboard.CommunicationType;
        public NemeioSerialNumber SerialNumber => _keyboard.SerialNumber;
        public FirmwareVersions Versions => _keyboard.Versions;
        public System.Version ProtocolVersion => _keyboard.ProtocolVersion;
        public string Name => _keyboard.Name;

        public event EventHandler OnStopRaised;
        public event EventHandler<StateChangedEventArgs> OnStateChanged;

        public KeyboardProxy(IKeyboard keyboard)
        {
            Identifier = keyboard.Identifier;

            _keyboard = keyboard as IKeyboard;
            _keyboard.OnStopRaised += Keyboard_OnStopRaised;

            _stateHolder = keyboard as IStateHolder;
            _stateHolder.OnStateChanged += StateHolder_OnStateChanged;
        }

        ~KeyboardProxy()
        {
            _stateHolder.OnStateChanged -= StateHolder_OnStateChanged;
        }

        private void StateHolder_OnStateChanged(object sender, StateChangedEventArgs e) => OnStateChanged?.Invoke(sender, e);

        private void Keyboard_OnStopRaised(object sender, EventArgs e) => OnStopRaised?.Invoke(sender, e);

        public bool Is(IKeyboard nemeio) => nemeio != null && Identifier == nemeio.Identifier;

        public static T CastTo<T>(IKeyboard keyboard) where T : KeyboardProxy
        {
            //  Load every interfaces of current types
            //  Recursive methods, find all interface tree

            var types = typeof(T).GetInterfaces();
            var valid = true;

            T result = default(T);

            foreach (var type in types)
            {
                //  We check only root interface and not main interface
                //  e.g. IMyProxy : IAHolder, IBHolder we don't want IMyProxy

                var isRootInterface = !type.GetInterfaces().Any();
                if (isRootInterface)
                {
                    if (!type.IsAssignableFrom(keyboard.GetType()))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
            {
                result = (T)Activator.CreateInstance(typeof(T), new object[] { keyboard });
            }

            return result;
        }

        public Task StopAsync() => _keyboard.StopAsync();

        public void Stop() => _keyboard.Stop();
    }
}
