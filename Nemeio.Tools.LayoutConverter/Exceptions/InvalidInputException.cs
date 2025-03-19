using Nemeio.Tools.LayoutConverter.Models;

namespace Nemeio.Tools.LayoutConverter.Exceptions
{
    internal enum InputType
    {
        FolderPath,
        LayoutId,
        ImageFormat,
        FolderContent,
        ImageType
    }

    internal class InvalidInputException : ToolException
    { 
        internal InputType Type { get; private set; }

        internal InvalidInputException(InputType type, string infos = null)
            : base(ToolErrorCode.InvalidInput, infos) => Type = type;
    }
}
