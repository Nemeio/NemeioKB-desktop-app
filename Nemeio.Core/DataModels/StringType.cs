using System;
using System.Collections.Generic;

namespace Nemeio.Core.DataModels
{
    public abstract class StringType<T> : IEquatable<T>, IEqualityComparer<T> where T : StringType<T>
    {
        protected readonly string Value;

        protected StringType(string value) => Value = value;

        public static implicit operator string(StringType<T> value) => value?.Value;

        public override string ToString() => Value;

        public static bool operator ==(StringType<T> a, StringType<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(StringType<T> a, StringType<T> b) => !(a == b);

        public virtual bool Equals(T other)
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
            return string.Equals(Value, ((StringType<T>)obj).Value);
        }

        public bool Equals(T x, T y) => x.Equals(y);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public int GetHashCode(T obj) => obj.GetHashCode();
    }
}
