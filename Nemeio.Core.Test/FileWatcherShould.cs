using System;
using System.IO;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    class FileWatcherShould
    {
        [Test]
        public void FileWatcher_01_NullOrEmptyFileName_DoesNotThroughButIsNull()
        {
            var watcher = new FileWatcher(null);
            Assert.IsNotNull(watcher);
            Assert.IsNull(watcher.FullPath);

            watcher = new FileWatcher(string.Empty);
            Assert.IsNotNull(watcher);
            Assert.IsNull(watcher.FullPath);
        }

        [Test]
        public void FileWatcher_02_SinglePathName_ThrowsInvalidOperationException()
        {
            string path = Path.GetTempPath();
            Assert.Throws<InvalidOperationException>(() => new FileWatcher(path));
        }

        [Test]
        public void FileWatcher_03_ValidExistingFileName_WorksOkAndCleansOk()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            Assert.IsTrue(File.Exists(path));

            using (var watcher = new FileWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.FullPath.Equals(path));
                Assert.IsTrue(File.Exists(path));
            }
            Assert.IsTrue(!File.Exists(path));
        }

        [Test]
        public void FileWatcher_04_ValidNonExistingFileName_CreatesFileAndThenWorksOkAndCleansOk()
        {
            // create temporary base
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            File.Delete(path);
            Assert.IsFalse(File.Exists(path));

            // change extension
            path = path.Replace(FileHelpers.TmpExtension, FileHelpers.WinExtension);
            Assert.IsFalse(File.Exists(path));

            // and check watcher creates the instance for use, then deletes
            using (var watcher = new FileWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.FullPath.Equals(path));
                Assert.IsTrue(File.Exists(path));
            }
            Assert.IsTrue(!File.Exists(path));
        }

        [Test]
        public void FileWatcher_05_ValidNonExistingFileAndFolderName_CreatesFileAndFolderAndThenWorksOkAndCleansOk()
        {
            // create temporary base
            string folderPath1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string folderPath2 = Path.Combine(folderPath1, Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(folderPath1));
            Assert.IsFalse(Directory.Exists(folderPath2));
            string path = Path.Combine(folderPath2, Guid.NewGuid().ToString() + FileHelpers.WinExtension);
            Assert.IsFalse(File.Exists(path));

            // and check watcher creates the instance for use, then deletes
            using (var watcher = new FileWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.FullPath.Equals(path));
                Assert.IsTrue(File.Exists(path));
                Assert.IsTrue(Directory.Exists(folderPath1));
                Assert.IsTrue(Directory.Exists(folderPath2));
            }
            Assert.IsFalse(File.Exists(path));
            Assert.IsFalse(Directory.Exists(folderPath2));
            Assert.IsFalse(Directory.Exists(folderPath1));
        }
    }
}
