using System;
using System.IO;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    class DirectoryWatcherShould
    {
        [Test]
        public void DirectoryWatcher_01_NullOrEmptyFolderName_DoesNotThroughButIsNull()
        {
            var watcher = new DirectoryWatcher(null);
            Assert.IsNotNull(watcher);
            Assert.IsNull(watcher.Folder);

            watcher = new DirectoryWatcher(string.Empty);
            Assert.IsNotNull(watcher);
            Assert.IsNull(watcher.Folder);

            watcher = new DirectoryWatcher("\t");
            Assert.IsNotNull(watcher);
            Assert.IsNull(watcher.Folder);
        }

        [Test]
        public void DirectoryWatcher_02_ExistingFileName_ThroughInvalidOperationException()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            Assert.IsTrue(File.Exists(path));
            Assert.Throws<InvalidOperationException>(() => new DirectoryWatcher(path));
            File.Delete(path);
            Assert.IsFalse(File.Exists(path));
        }

        [Test]
        public void DirectoryWatcher_03_ExistingFolderName_WorksOk()
        {
            string path = Path.GetTempPath();
            Assert.IsTrue(Directory.Exists(path));

            using (var watcher = new DirectoryWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.Folder.Equals(path));
                Assert.IsTrue(Directory.Exists(path));
            }
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void DirectoryWatcher_04_ValidNonExistingFolderName_CreatesFolderAndWorksOkAndCleansOk()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(path));

            using (var watcher = new DirectoryWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.Folder.Equals(path));
                Assert.IsTrue(Directory.Exists(path));
            }
            Assert.IsFalse(Directory.Exists(path));
        }

        [Test]
        public void DirectoryWatcher_05_ValidNonExistingFolderNameDepth2_CreatesFoldersAndThenWorksOkAndCleansOk()
        {
            // create temporary base
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(path));

            using (var watcher = new DirectoryWatcher(path))
            {
                Assert.IsNotNull(watcher);
                Assert.IsTrue(watcher.Folder.Equals(path));
                Assert.IsTrue(Directory.Exists(path));
            }
            Assert.IsFalse(Directory.Exists(path));
        }
    }
}
