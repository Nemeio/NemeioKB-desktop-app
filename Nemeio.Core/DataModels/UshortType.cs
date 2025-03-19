using System;
using System.Collections.Generic;

namespace Nemeio.Core.DataModels
{
    public abstract class UShortType<T> : IEquatable<T>, IEqualityComparer<T> where T : UShortType<T>
    {
        protected readonly ushort Value;

        protected UShortType(ushort value) => Value = value;

        public static implicit operator ushort(UShortType<T> value) => value.Value;

        public static bool operator ==(UShortType<T> a, UShortType<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(UShortType<T> a, UShortType<T> b) => !(a == b);

        public bool Equals(T other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return string.Equals(Value, ((UShortType<T>)obj).Value);
        }

        public override string ToString() => Value.ToString();

        public bool Equals(T x, T y) => x.Equals(y);

        public override int GetHashCode() =>  Value.GetHashCode();

        public int GetHashCode(T obj) => obj.GetHashCode();
    }
}
