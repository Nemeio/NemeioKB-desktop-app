namespace Nemeio.Presentation.Menu.Tools
{
    public sealed class ObservableValueChangedEventArgs<T>
    {
        public T Value { get; private set; }

        public ObservableValueChangedEventArgs(T value)
        {
            Value = value;
        }
    }
}
