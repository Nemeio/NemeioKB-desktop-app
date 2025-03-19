namespace Nemeio.Tools.LayoutConverter.Models.Requirements
{
    internal class RequirementError
    {
        internal string Description { get; private set; }

        internal string FilePath { get; private set; }

        internal RequirementError(string filePath, string description)
        {
            FilePath = filePath;
            Description = description;
        }
    }
}
