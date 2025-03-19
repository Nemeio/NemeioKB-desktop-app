using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Nemeio.Core.FileSystem
{
    public class FileSystem : IFileSystem
    {
        public bool CreateDirectoryIfNotExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (System.IO.Directory.Exists(path))
            {
                return false;
            }

            System.IO.Directory.CreateDirectory(path);

            return true;
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var exists = System.IO.File.Exists(filePath);

            return exists;
        }

        public bool RemoveFileIfExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!FileExists(filePath))
            {
                return false;
            }

            System.IO.File.Delete(filePath);

            return true;
        }

        public bool RemoveFolderIfExists(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                return false;
            }

            System.IO.Directory.Delete(folderPath, true);

            return true;
        }

        public async Task WriteAsync(string filePath, byte[] fileContent)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (fileContent == null)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(fileContent, 0, fileContent.Length);
            }
        }

        public async Task<string> ReadTextAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var fileStream = System.IO.File.OpenText(filePath))
            {
                return await fileStream.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadByteArrayAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            byte[] result;

            using (var fileStream = System.IO.File.Open(filePath, FileMode.Open))
            {
                result = new byte[fileStream.Length];
                await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
            }

            return result;
        }

        public async Task WriteTextAsync(string filePath, string content)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var fileStream = System.IO.File.CreateText(filePath))
            {
                await fileStream.WriteAsync(content);
            }
        }

        public void CopyTo(string filePath, string destinationFolderPath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(destinationFolderPath))
            {
                throw new ArgumentNullException(nameof(destinationFolderPath));
            }

            var fileName = Path.GetFileName(filePath);

            System.IO.File.Copy(
                filePath,
                Path.Combine(destinationFolderPath, fileName)
            );
        }

        public bool FileChecksumIsValid(string filePath, string checksum)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrEmpty(checksum))
            {
                throw new ArgumentNullException(nameof(checksum));
            }

            var fileCheckSum = FileHelpers.CalculateMD5(filePath);

            return fileCheckSum == checksum;
        }

        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var exists = System.IO.Directory.Exists(path);

            return exists;
        }

        public string GetFileExtension(string path)
        {
            var extension = Path.GetExtension(path);

            return extension;
        }

        public string GetDirectoryName(string path)
        {
            var directoryName = Path.GetDirectoryName(path);

            return directoryName;
        }

        public string GetFileName(string path)
        {
            var fileName = Path.GetFileName(path);

            return fileName;
        }

        public string GetCurrentDirectory() => System.IO.Directory.GetCurrentDirectory();

        public bool IsReady(string filePath)
        {
            try
            {
                using (System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public IFile GetFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException($"File at path <{path}> doesn't exists");
            }

            var name = Path.GetFileName(path);
            var file = new File(name, path);

            return file;
        }

        public IDirectory GetDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!System.IO.Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory at path <{path}> doesn't exists");
            }

            var name = Path.GetFileName(path);
            var directory = new Directory(name, path);

            return directory;
        }

        public IDirectory Unzip(string filePath, string toPath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            ZipFile.ExtractToDirectory(filePath, toPath);

            if (!System.IO.Directory.Exists(toPath))
            {
                throw new System.IO.DirectoryNotFoundException($"Extract file <{filePath}> to <{toPath}> failed");
            }

            var directory = GetDirectory(toPath);
            var targetDirectory = directory.GetEntries().OfType<IDirectory>().First();

            return targetDirectory;
        }
    }
}
