using System.Collections.Generic;
using Nemeio.Core.Extensions;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.DataModels.Configurator
{
    public class SpecialSequences
    {
        private const int CTRL_POSITION = 71;
        private const int CTRL_RIGHT_POSITION = 77;
        private const int ALT_POSITION = 74;
        private const int ALTGR_POSITION = 76;
        private const int DELETE_POSITION = 15;

        public Permutation<int> WinSAS { get; }

        public SpecialSequences(Permutation<int> sas)
        {
            WinSAS = sas;
        }

        public static SpecialSequences Empty => new SpecialSequences(
            new Permutation<int>()
        );

        public static SpecialSequences Default => ObjectExtensions.IsOSXPlatform() ? MacDefault : WindowsDefault;

        private static SpecialSequences MacDefault = new SpecialSequences(new Permutation<int>());

        private static SpecialSequences WindowsDefault = new SpecialSequences(
            LayoutSecureAttentionSequenceBuilder.GetPermutations(new List<int>() { CTRL_POSITION, ALT_POSITION, DELETE_POSITION }, Layout.WinSASLength)
                .Concat(LayoutSecureAttentionSequenceBuilder.GetPermutations(new List<int>() { CTRL_POSITION, ALTGR_POSITION, DELETE_POSITION }, Layout.WinSASLength))
                .Concat(LayoutSecureAttentionSequenceBuilder.GetPermutations(new List<int>() { CTRL_RIGHT_POSITION, ALT_POSITION, DELETE_POSITION }, Layout.WinSASLength))
                .Concat(LayoutSecureAttentionSequenceBuilder.GetPermutations(new List<int>() { CTRL_RIGHT_POSITION, ALTGR_POSITION, DELETE_POSITION }, Layout.WinSASLength))
        );
    }
}
