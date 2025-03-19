namespace Nemeio.Core.JsonModels
{
    public class ActionModel
    {
        public NemeioActionType Type { get; set; }

        public string Data { get; set; }

        public bool IsShift() => Data == KeyboardLiterals.Shift;

        public bool IsAltGr() => Data == KeyboardLiterals.AltGr;

        public bool IsCtrl() => Data == KeyboardLiterals.Ctrl;

        public bool IsAlt() => Data == KeyboardLiterals.Alt;

        public bool IsModifier() => IsShift() || IsAltGr();

        public static ActionModel CreateModifierAction(string modifier)
        {
            return new ActionModel()
            {
                Data = modifier,
                Type = NemeioActionType.Special
            };
        }
    }
}
