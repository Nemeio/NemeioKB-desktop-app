using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.Layouts.Color
{
    /// <summary>
    /// This class represnt color with hexadecimal format
    /// It doesn't support alpha
    /// </summary>
    public sealed class HexColor : IEquatable<HexColor>, IEqualityComparer<HexColor>, IComparable<HexColor>, IBackupable<HexColor>
    {
        public string HexValue { get; private set; }

        public HexColor(string hexValue)
        {
            if (string.IsNullOrEmpty(hexValue))
            {
                throw new ArgumentNullException(nameof(hexValue));
            }

            var regex = "^#(?:[0-9a-fA-F]{3}){1,2}$";
            var match = Regex.Match(hexValue, regex, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new ArgumentException($"The value <{hexValue}> is not valid. It must conform to regex <{regex}>");
            }

            HexValue = hexValue;
        }

        public static HexColor Black => new HexColor("#000000");
        public static HexColor White => new HexColor("#ffffff");
        public static HexColor Gray => new HexColor("#999999");
        public bool IsBlack() => Equals(Black);

        public HexColor CreateBackup()
        {
            var backup = new HexColor(HexValue);

            return backup;
        }

        public override string ToString() => HexValue;

        public int CompareTo(HexColor other) => HexValue.CompareTo(other.ToString());

        public bool Equals(HexColor other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(HexValue, other.HexValue);
        }

        public bool Equals(HexColor x, HexColor y) => x.Equals(y);

        public int GetHashCode(HexColor obj) => HexValue.GetHashCode();

        #region Operators

        public static implicit operator string(HexColor value) => value?.ToString();

        public static bool operator ==(HexColor a, HexColor b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(HexColor a, HexColor b) => !(a == b);

        #endregion
    }
}
