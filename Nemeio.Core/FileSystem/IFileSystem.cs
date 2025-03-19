using System.Threading.Tasks;

namespace Nemeio.Core.FileSystem
{
    public interface IFileSystem
    {
        bool CreateDirectoryIfNotExists(string path);
        bool FileExists(string filePath);
        bool RemoveFileIfExists(string filePath);
        bool RemoveFolderIfExists(string folderPath);
        Task WriteAsync(string filePath, byte[] fileContent);
        Task<string> ReadTextAsync(string filePath);
        Task<byte[]> ReadByteArrayAsync(string filePath);
        Task WriteTextAsync(string filePath, string content);
        void CopyTo(string filePath, string destinationFolderPath);
        bool FileChecksumIsValid(string filePath, string checksum);
        bool DirectoryExists(string path);
        string GetFileExtension(string path);
        string GetDirectoryName(string path);
        string GetFileName(string path);
        string GetCurrentDirectory();
        bool IsReady(string filePath);
        IDirectory Unzip(string filePath, string toPath);
        IFile GetFile(string path);
        IDirectory GetDirectory(string path);
    }
}
