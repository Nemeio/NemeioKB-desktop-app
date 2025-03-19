using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Firmware;
using File = Nemeio.Core.FileSystem.File;

namespace Nemeio.Cli.Package.Update.Commands.Read
{
    internal sealed class PackageUpdateFile : File
    {
        public PackageUpdateFile(IFile file) 
            : base(file) { }

        public async Task<PackageFirmware> LoadPackageAsync()
        {
            await Task.Yield();

            using (var stream = System.IO.File.Open(this.Path, FileMode.Open))
            using (var binaryReader = new BinaryReader(stream))
            {
                var package = new PackageFirmware();
                package.Read(binaryReader);

                return package;
            }
        }
    }
}
