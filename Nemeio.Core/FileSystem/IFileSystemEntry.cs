namespace Nemeio.Core.FileSystem
{
    public interface IFileSystemEntry
    {
        string Path { get; }
        string Name { get; }
    }
}
