namespace Nemeio.Core.Keyboard.Updates.Progress
{
    public enum UpdateComponent : byte
    {
        Unknown = 0,
        System = 1,
        FlashMemory = 2,
        FileSystem = 3,
        Display = 4,
        BluetoothLE = 5,
        FuelGauge = 6,
    }

    public enum UpdateStatusType : byte
    {
        Failed = 0x01,
        InProgress = 0x02,
        Rollback = 0x03
    }

    public abstract class UpdateProgressEventArgs
    {
        public UpdateComponent Component { get; private set; }
        public UpdateStatusType Type { get; private set; }

        protected UpdateProgressEventArgs(UpdateComponent component, UpdateStatusType type)
        {
            Component = component;
            Type = type;
        }
    }
}
