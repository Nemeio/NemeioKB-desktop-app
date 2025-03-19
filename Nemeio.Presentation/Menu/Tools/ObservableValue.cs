using System;

namespace Nemeio.Presentation.Menu.Tools
{
    public sealed class ObservableValue<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke(this, new ObservableValueChangedEventArgs<T>(value));
            }
        }

        public event EventHandler<ObservableValueChangedEventArgs<T>> OnValueChanged;
    }
}
