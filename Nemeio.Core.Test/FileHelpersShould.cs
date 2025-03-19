using System;
using System.IO;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class FileHelpersShould
    {
        static string SpaceString = "   ";
        static string TabbedString = "\t\t";
        static string PathMissingExtension1 = @"C:\Temp\test";
        static string PathMissingExtension2 = "C:/Temp/test";
        static string PathMissingExtension3 = "/Application/test";
        static string NameMissingExtension1 = "test";
        static string PathMissingName1 = @"C:\Temp\.exe";
        static string PathMissingName2 = "C:/Temp/.exe";
        static string PathMissingName3 = "/Application/.app";
        static string MissingName1 = ".exe";
        static string MissingName2 = ".app";
        static string ValidName1 = "test.exe";
        static string ValidName2 = "test.app";

        [Test]
        public void FileHelpers_IsValidPathString_01_NullOrWhitespace_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(null));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(string.Empty));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(SpaceString));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(TabbedString));
        }

        [Test]
        public void FileHelpers_IsValidPathString_02_MissingExtenion_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingExtension1));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingExtension2));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingExtension3));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(NameMissingExtension1));
        }

        [Test]
        public void FileHelpers_IsValidPathString_03_MissingMainName_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingName1));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingName2));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(PathMissingName3));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(MissingName1));
            Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(MissingName2));
        }

        [Test]
        public void FileHelpers_IsValidPathString_04_ValidApplicationName_ReturnFalse()
        {
            Assert.IsFalse(FileHelpers.IsValidPathString(ValidName1));
            Assert.IsFalse(FileHelpers.IsValidPathString(ValidName2));
        }

        [Test]
        public void FileHelpers_IsValidPathString_05_PathFileFolderDoesNotExist_ThrowsInvalidOperationException()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            try
            {
                Assert.IsTrue(File.Exists(path));
                string newName = path.Replace(FileHelpers.TmpExtension, FileHelpers.WinExtension);
                File.Move(path, newName);
                path = newName;
                File.Delete(path);

                Assert.IsFalse(File.Exists(path));
                Assert.Throws<InvalidOperationException>(() => FileHelpers.IsValidPathString(path));
            }
            finally
            {
            }
        }

        [Test]
        public void FileHelpers_IsValidPathString_06_ValidPath_ReturnsTrue()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            try
            {
                Assert.IsTrue(File.Exists(path));
                string newName = path.Replace(FileHelpers.TmpExtension, FileHelpers.WinExtension);
                File.Move(path, newName);
                path = newName;
                Assert.IsTrue(FileHelpers.IsValidPathString(path));
            }
            finally
            {
                File.Delete(path);
                Assert.IsFalse(File.Exists(path));
            }
        }

        //// CalculateMD5_01

        [Test]
        public void FileHelpers_CalculateMD5_01_NullOrWhiteSpace_ReturnsNull()
        {
            Assert.IsNull(FileHelpers.CalculateMD5(null));
            Assert.IsNull(FileHelpers.CalculateMD5(string.Empty));
            Assert.IsNull(FileHelpers.CalculateMD5("   "));
            Assert.IsNull(FileHelpers.CalculateMD5("\t"));
        }

        [Test]
        public void FileHelpers_CalculateMD5_02_NonExistingFilename_ThrowsFileNotFoundException()
        {
            // create temporary base
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            File.Delete(path);
            Assert.IsFalse(File.Exists(path));

            Assert.Throws<FileNotFoundException>(() => FileHelpers.CalculateMD5(path));
        }

        [Test]
        public void FileHelpers_CalculateMD5_03_ExistingFilename_WorksOk()
        {
            // create temporary base
            string path = null;
            using (var watcher = FileHelpers.WatchMe(Path.Combine(Path.GetTempPath(), Path.GetTempFileName())))
            {
                path = watcher.FullPath;
                Assert.IsTrue(File.Exists(path));

                var md5 = FileHelpers.CalculateMD5(path);
                Assert.IsTrue(md5.Equals("d41d8cd98f00b204e9800998ecf8427e"));
            }
            Assert.IsFalse(File.Exists(path));
        }
    }
}
