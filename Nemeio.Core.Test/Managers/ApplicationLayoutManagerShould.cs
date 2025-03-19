using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels;
using Nemeio.Core.Errors;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Test.Fakes;
using NUnit.Framework;

namespace Nemeio.Core.Test.Managers
{
    /*[TestFixture]
    public class ApplicationLayoutManagerShould
    {
        private ApplicationLayoutManager _applicationLayoutManager;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = new LoggerFactory();
            var mockBlacklistRepository = Mock.Of<IBlacklistDbRepository>();
            var mockLayoutLibrary = Mock.Of<ILayoutLibrary>();
            var mockErrorManager = Mock.Of<IErrorManager>();

            _applicationLayoutManager = new ApplicationLayoutManager(loggerFactory, mockBlacklistRepository, mockLayoutLibrary, mockErrorManager);
        }

        //// Constructor base tests

        [Test]
        public void ApplicationLayoutManager_01_01_Constructor_WorksOk()
        {
            Assert.IsNotNull(_applicationLayoutManager);
            Assert.IsTrue(_applicationLayoutManager.ExceptionNameList.Count > 0);
        }

        //// FindLatestAssociatedLayoutId base tests

        [Test]
        public void ApplicationLayoutManager_02_01_FindLatestAssociatedLayoutId_NullOrWhiteSpacePathName_ReturnNull()
        {
            // create default empty information field (only absolute path to be used)
            var application = new Application();

            // null case
            application.ApplicationPath = null;
            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(application));

            // empty case
            application.ApplicationPath = string.Empty;
            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(application));

            // whitespace case
            application.ApplicationPath = "   ";
            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(application));

            // tab case
            application.ApplicationPath = "\t";
            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(application));
        }

        [Test]
        public void ApplicationLayoutManager_02_02_FindLatestAssociatedLayoutId_BlacklistedName_ReturnNull()
        {
            // get processes entries and fileter on ProcessName to possibly obtain the explorer path
            var processesList = Process.GetProcesses().Where(proc => proc.ProcessName.ToLower().Equals(FakeBlacklistDbRepository.ExplorerString));
            var process = processesList.FirstOrDefault();
            var path = process.MainModule.FileName;
            var versionInfo = FileVersionInfo.GetVersionInfo(path);
            var processInformation = new Application() { ApplicationPath = path, Name = versionInfo.FileDescription.ToLower() };

            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(processInformation));

            // test for fake nemeio path
            var applicationName = FakeBlacklistDbRepository.NemeioString + " Application";
            var nemeioDummyPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), FakeBlacklistDbRepository.NemeioString + FileHelpers.WinExtension);
            using(var watcher = FileHelpers.WatchMe(nemeioDummyPath))
            {
                processInformation = new Application() { ApplicationPath = nemeioDummyPath, Name = applicationName };
                Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(processInformation));
            }
            Assert.IsFalse(File.Exists(nemeioDummyPath));
        }

        [Test]
        public void ApplicationLayoutManager_02_03_FindLatestAssociatedLayoutId_InvalidPath_ReturnNull()
        {
            // create information with invalid path name
            var processInformation = new Application() { ApplicationPath = "C:/Temp/InvalidPathName" };

            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(processInformation));
        }

        [Test]
        public void ApplicationLayoutManager_02_04_FindLatestAssociatedLayoutId_ModulePathWithNoDefaultSet_ReturnNull()
        {
            // get processes entries and filter on ProcessName to possibly obtain the explorer path
            var processInformation = new Application() { ApplicationPath = GetLocalModulePath() };

            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(processInformation));
        }

        [Test]
        public void ApplicationLayoutManager_02_05_FindLatestAssociatedLayoutId_ModulePathWithDefaultSet_ReturnNull()
        {
            // get processes entries and fileter on ProcessName to possibly obtain the explorer path
            var processInformation = new Application() { ApplicationPath = GetLocalModulePath() };

            //  FIXME
            //Assert.IsNotNull(_layoutManager.DefaultLayoutId);
            Assert.IsNull(_applicationLayoutManager.FindLatestAssociatedLayoutId(processInformation));
        }

        [Test]
        public void ApplicationLayoutManager_02_06_FindLatestAssociatedLayoutId_ModulePathWithDefaultSetAndLayoutChange_ReturnLatest()
        {
            _layoutManager.SetDefaultLayout(_layoutManager.LayoutList.Last().LayoutId);
            var manager = _layoutManager.ApplicationLayoutManager;

            // get current module name (not associated to any keyboard)
            var processInformation = new Application() { ApplicationPath = GetLocalModulePath() };

            Assert.IsNotNull(_layoutManager.DefaultLayoutId);
            Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation));

            _layoutManager.EnforceSystemLayout(FakeLayouts.FakeFR.LayoutId);
            Assert.IsTrue(FakeLayouts.FakeFR.LayoutId != _layoutManager.DefaultLayoutId);
            Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation));

            _layoutManager.EnforceSystemLayout(FakeLayouts.FakeCA.LayoutId);
            Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation));
        }

        [Test]
        public void ApplicationLayoutManager_02_07_FindLatestAssociatedLayoutId_ModulePathWithKeyboardAssociated_ReturnAssociated()
        {
            _layoutManager.ResetDefaultLayout();
            var manager = _layoutManager.ApplicationLayoutManager;

            // get current module name (not associated to any keyboard)
            var processInformation = new Application() { ApplicationPath = GetLocalModulePath() };

            Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation));

            // add the association to FakeCA layout (middle of test list)
            FakeLayouts.FakeCA.LayoutInfo.LinkApplicationEnable = true;
            FakeLayouts.FakeCA.LayoutInfo.LinkApplicationPaths = new List<string>() { GetLocalModulePath() };
            var result = manager.FindLatestAssociatedLayoutId(processInformation);
            Assert.IsTrue(result == FakeLayouts.FakeCA.LayoutId);
            Assert.IsNull(_layoutManager.DefaultLayoutId);

            // now switch to another layout and check it is kept
            _layoutManager.EnforceSystemLayout(FakeLayouts.FakeUS.LayoutId);
            result = manager.FindLatestAssociatedLayoutId(processInformation);
            Assert.IsFalse(result == FakeLayouts.FakeUS.LayoutId);
            Assert.IsTrue(result == FakeLayouts.FakeCA.LayoutId);
            Assert.IsNull(_layoutManager.DefaultLayoutId);
        }

        [Test]
        public void ApplicationLayoutManager_02_08_FindLatestAssociatedLayoutId_ModuleOnlyNameWithKeyboardAssociated_ReturnAssociated()
        {
            _layoutManager.ResetDefaultLayout();
            var manager = _layoutManager.ApplicationLayoutManager;

            // build varying paths with same module name and differing associations
            var fakeApplicationName = @"fake.exe";
            var fakeFolderPath1 = Path.Combine(Path.GetTempPath(), @"ApplicationLayoutManagerTestFake1");
            var fakeApplicationPath1 = Path.Combine(fakeFolderPath1, fakeApplicationName);
            Assert.False(Directory.Exists(fakeFolderPath1));
            var fakeFolderPath2 = Path.Combine(Path.GetTempPath(), @"ApplicationLayoutManagerTestFake2");
            var fakeApplicationPath2 = Path.Combine(fakeFolderPath2, fakeApplicationName);
            Assert.False(Directory.Exists(fakeFolderPath2));

            using (var watcher1 = FileHelpers.WatchMe(fakeApplicationPath1))
            using (var watcher2 = FileHelpers.WatchMe(fakeApplicationPath2))
            {
                // get current module name (not associated to any keyboard)
                var processInformation1 = new ProcessInformation() { ApplicationPath = fakeApplicationPath1 };
                var processInformation2 = new ProcessInformation() { ApplicationPath = fakeApplicationPath2 };

                Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation1));
                Assert.IsNull(manager.FindLatestAssociatedLayoutId(processInformation2));

                // add the 1st association to FakeCA layout (middle of test list)
                FakeLayouts.FakeCA.LayoutInfo.LinkApplicationEnable = true;
                FakeLayouts.FakeCA.LayoutInfo.LinkApplicationPaths = new List<string>() { fakeApplicationPath1 };
                var result = manager.FindLatestAssociatedLayoutId(processInformation1);
                Assert.IsTrue(result == FakeLayouts.FakeCA.LayoutId);
                Assert.IsNull(_layoutManager.DefaultLayoutId);

                // add the 2nd association to FakeFR layout (start of test list)
                FakeLayouts.FakeFR.LayoutInfo.LinkApplicationEnable = true;
                FakeLayouts.FakeFR.LayoutInfo.LinkApplicationPaths = new List<string>() { fakeApplicationName };
                result = manager.FindLatestAssociatedLayoutId(processInformation2);
                Assert.IsTrue(result == FakeLayouts.FakeFR.LayoutId);
                Assert.IsNull(_layoutManager.DefaultLayoutId);

                // now switch to another layout and check it is kept
                _layoutManager.EnforceSystemLayout(FakeLayouts.FakeUS.LayoutId);
                result = manager.FindLatestAssociatedLayoutId(processInformation2);
                Assert.IsFalse(result == FakeLayouts.FakeUS.LayoutId);
                Assert.IsTrue(result == FakeLayouts.FakeFR.LayoutId);
                Assert.IsNull(_layoutManager.DefaultLayoutId);
            }
            Assert.False(Directory.Exists(fakeFolderPath1));
            Assert.False(Directory.Exists(fakeFolderPath2));
        }

        //// Utilities

        private string GetLocalModulePath()
        {
            var uri = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return uri.AbsolutePath;
        }

        private string GetLocalModuleFileDescription()
        {
            var uri = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var versionInfo = FileVersionInfo.GetVersionInfo(uri.AbsolutePath);
            return versionInfo.FileDescription;
        }
    }*/
}
