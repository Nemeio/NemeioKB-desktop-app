using System;
using System.Linq;
using System.Text;

namespace Nemeio.Core.Keyboard.SerialNumber
{
    public class NemeioSerialNumber
    {
        private const int SerialNumberLength = 12;

        private readonly byte[] _serialNumber;

        public NemeioSerialNumber(byte[] serialNumber)
        {
            if (serialNumber.Length != SerialNumberLength)
            {
                throw new ArgumentOutOfRangeException(nameof(serialNumber));
            }

            _serialNumber = serialNumber;
        }

        public static bool operator ==(NemeioSerialNumber a, NemeioSerialNumber b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return _serialNumber.SequenceEqual(((NemeioSerialNumber)obj)._serialNumber);
        }

        public bool Equals(NemeioSerialNumber x, NemeioSerialNumber y) => x.Equals(y);

        public static bool operator !=(NemeioSerialNumber a, NemeioSerialNumber b) => !(a == b);

        public static implicit operator byte[](NemeioSerialNumber value) => value?._serialNumber;

        public override string ToString() => Encoding.ASCII.GetString(_serialNumber).ToCharArray().Select(x => x.ToString()).Aggregate((x, y) => $"{x}{y}");

        public override int GetHashCode() => _serialNumber.GetHashCode();
    }
}
