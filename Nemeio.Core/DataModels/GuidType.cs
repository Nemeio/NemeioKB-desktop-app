using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.DataModels
{
    public abstract class GuidType<T> : IEquatable<T>, IEqualityComparer<T>, IComparable<T> where T : GuidType<T>
    {
        private const ushort StringGuidLength = 36;
        private const ushort ByteGuidLength = 16;
        private readonly Guid _value;

        protected GuidType(string value)
        {
            if (Guid.TryParse(value, out var res))
            {
                _value = res;
            }
            else
            {
                throw new ArgumentException($"Invalid Guid: {value}");
            }
        }

        protected GuidType(byte[] value)
        {
            if (value.Length == StringGuidLength)
            {
                var str = Encoding.UTF8.GetString(value);
                _value = new Guid(str);
            }
            else if (value.Length == ByteGuidLength)
            {
                _value = new Guid(value);
            }
            else
            {
                throw new ArgumentException($"Invalid Guid: {value}");
            }
        }

        public static implicit operator string(GuidType<T> value) => value?.ToString();

        public byte[] GetBytes()
        {
            var str = _value.ToString();
            return Encoding.UTF8.GetBytes(str);
        }

        public override string ToString() => _value.ToString();

        public static bool operator ==(GuidType<T> a, GuidType<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(GuidType<T> a, GuidType<T> b) => !(a == b);

        public virtual bool Equals(T other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return string.Equals(_value, ((GuidType<T>)obj)._value);
        }

        public bool Equals(T x, T y) => x.Equals(y);

        public override int GetHashCode() => _value.GetHashCode();

        public int GetHashCode(T obj) => obj.GetHashCode();

        public int CompareTo(T other)
        {
            return _value.ToString().CompareTo(other.ToString());
        }
    }
}
