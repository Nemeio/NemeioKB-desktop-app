using System.IO;
using System.Threading.Tasks;

namespace Nemeio.Core.FileSystem
{
    public class File : FileSystemEntry, IFile
    {
        public string Extension => System.IO.Path.GetExtension(Name);

        public File(string name, string path) 
            : base(name, path) { }

        public File(IFile file)
            : this(file.Name, file.Path) { }

        public async Task<string> ReadContentAsync()
        {
            using (var fileStream = System.IO.File.OpenText(Path))
            {
                return await fileStream.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadByteArrayContentAsync()
        {
            byte[] result;

            using (var fileStream = System.IO.File.Open(Path, FileMode.Open))
            {
                result = new byte[fileStream.Length];
                await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
            }

            return result;
        }
    }
}
