using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutIdWithHash
    {
        public LayoutId Id { get; set; }
        public LayoutHash Hash { get; set; }
        public LayoutIdWithHash(LayoutId layoutId, LayoutHash layoutHash)
        {
            Id = layoutId;
            Hash = layoutHash;
        }

        public override string ToString()
        {
            return $"Id : {Id} Hash : {Hash}";
        }
    }
}
