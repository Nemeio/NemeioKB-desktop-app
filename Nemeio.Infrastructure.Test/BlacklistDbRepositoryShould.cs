using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Core.Test.Fakes;
using Nemeio.Infrastructure.Repositories;
using NUnit.Framework;

namespace Nemeio.Infrastructure.Test
{
    [TestFixture]
    public class BlacklistDbRepositoryShould : DbRepositoryTestBase
    {
        private const string TestString = "test";
        private const string DummyString = "dummy";
        private const string SpaceEmptyString = "    ";
        private const string TabEmptyString = "\t\t";
        private const int DummyBlacklistId = 666;

        private BlacklistDbRepository _blacklistDbRepository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _blacklistDbRepository = new BlacklistDbRepository(new LoggerFactory(), _databaseAccessFactory);
        }

        //// ReadSystemBlacklists tests

        [Test]
        public void BlacklistDbRepository_01_01_ReadSystemBlacklists_DefaultSystemSeed_WorksOk()
        {
            var systemBlacklists = _blacklistDbRepository.ReadSystemBlacklists();

            systemBlacklists.Count().Should().Be(2);
            Assert.IsTrue(BlacklistContains(systemBlacklists, FakeBlacklistDbRepository.NemeioString));
            Assert.IsTrue(BlacklistContains(systemBlacklists, FakeBlacklistDbRepository.ExplorerString));
        }

        //// FindBlacklistByName tests

        [Test]
        public void BlacklistDbRepository_02_01_FindBlacklistByName_DefaultSystemName_WorksOk()
        {
            var systemBlacklists = _blacklistDbRepository.ReadSystemBlacklists();
            Assert.IsTrue(BlacklistContains(systemBlacklists, FakeBlacklistDbRepository.NemeioString));
            Assert.IsTrue(BlacklistContains(systemBlacklists, FakeBlacklistDbRepository.ExplorerString));

            var nemeio = _blacklistDbRepository.FindBlacklistByName(FakeBlacklistDbRepository.NemeioString);
            Assert.IsNotNull(nemeio);
            Assert.IsTrue(nemeio.IsSystem);

            var explorer = _blacklistDbRepository.FindBlacklistByName(FakeBlacklistDbRepository.ExplorerString);
            Assert.IsNotNull(explorer);
            Assert.IsTrue(explorer.IsSystem);
        }

        [Test]
        public void BlacklistDbRepository_02_02_FindBlacklistByName_UnknownName_ReturnsNull()
        {
            var dummy = _blacklistDbRepository.FindBlacklistByName(DummyString);
            Assert.IsNull(dummy);
        }

        [Test]
        public void BlacklistDbRepository_02_03_FindBlacklistByName_NullOrEmptyName_ReturnsNull()
        {
            var notFound = _blacklistDbRepository.FindBlacklistByName(null);
            Assert.IsNull(notFound);

            notFound = _blacklistDbRepository.FindBlacklistByName(string.Empty);
            Assert.IsNull(notFound);

            notFound = _blacklistDbRepository.FindBlacklistByName(SpaceEmptyString);
            Assert.IsNull(notFound);

            notFound = _blacklistDbRepository.FindBlacklistByName(TabEmptyString);
            Assert.IsNull(notFound);
        }

        //// FindBlacklistById tests

        [Test]
        public void BlacklistDbRepository_03_01_FindBlacklistById_OneSystemId_WorksOk()
        {
            var systemBlacklists = _blacklistDbRepository.ReadSystemBlacklists();
            var explorer = _blacklistDbRepository.FindBlacklistByName(FakeBlacklistDbRepository.ExplorerString);
            Assert.IsNotNull(explorer);

            var idSearch = _blacklistDbRepository.FindBlacklistById(explorer.Id);
            Assert.IsTrue(idSearch.Name == explorer.Name);

            var dummyIdSearch = _blacklistDbRepository.FindBlacklistById(666);
            Assert.IsNull(dummyIdSearch);
        }

        [Test]
        public void BlacklistDbRepository_03_01_FindBlacklistById_InvalidId_ReturnsNull()
        {
            var dummyIdSearch = _blacklistDbRepository.FindBlacklistById(DummyBlacklistId);
            Assert.IsNull(dummyIdSearch);
        }

        //// SaveBlacklist tests

        [Test]
        public void BlacklistDbRepository_04_01_SaveBlacklist_SystemBlacklist_ReturnsExistingElement()
        {
            var systemItem = _blacklistDbRepository.FindBlacklistByName(FakeBlacklistDbRepository.ExplorerString);
            var oldNewItem = _blacklistDbRepository.SaveBlacklist(FakeBlacklistDbRepository.ExplorerString);

            Assert.IsNotNull(oldNewItem);
            Assert.IsTrue(oldNewItem.Id == systemItem.Id);
        }

        [Test]
        public void BlacklistDbRepository_04_02_SaveBlacklist_NewItem_ReturnsNewItem()
        {
            var nonExistingItem = _blacklistDbRepository.FindBlacklistByName(TestString);
            Assert.IsNull(nonExistingItem);

            var newItem = _blacklistDbRepository.SaveBlacklist(TestString);
            Assert.IsNotNull(newItem);
            Assert.IsTrue(newItem.Name.Equals(TestString));
            Assert.IsTrue(newItem.IsSystem == false);
        }

        [Test]
        public void BlacklistDbRepository_04_03_SaveBlacklist_AnyExistingItem_ReturnsExistingItem()
        {
            var nonExistingItem = _blacklistDbRepository.FindBlacklistByName(TestString);
            Assert.IsNull(nonExistingItem);

            var newItem = _blacklistDbRepository.SaveBlacklist(TestString);
            Assert.IsNotNull(newItem);

            var oldNewItem = _blacklistDbRepository.SaveBlacklist(TestString);
            Assert.IsNotNull(oldNewItem);
            Assert.IsTrue(oldNewItem.Id == newItem.Id);
        }

        [Test]
        public void BlacklistDbRepository_04_04_SaveBlacklist_NullOrEmptyString_AddsNothing()
        {
            var newItem = _blacklistDbRepository.SaveBlacklist(null);
            Assert.IsNull(newItem);

            newItem = _blacklistDbRepository.SaveBlacklist(string.Empty);
            Assert.IsNull(newItem);

            newItem = _blacklistDbRepository.SaveBlacklist(SpaceEmptyString);
            Assert.IsNull(newItem);

            newItem = _blacklistDbRepository.SaveBlacklist(TabEmptyString);
            Assert.IsNull(newItem);
        }

        //// ReadBlacklists tests

        [Test]
        public void BlacklistDbRepository_05_01_ReadBlacklists_DefaultSystemSeed_EmptyList()
        {
            var blacklists = _blacklistDbRepository.ReadBlacklists();
            blacklists.Count().Should().Be(0);
        }

        [Test]
        public void BlacklistDbRepository_05_02_ReadBlacklists_SeedAFewTestCases_ReturnsFullList()
        {
            var systemBlacklists = _blacklistDbRepository.ReadSystemBlacklists();
            var blacklists = _blacklistDbRepository.ReadBlacklists();
            int systemCount = systemBlacklists.Count();
            blacklists.Count().Should().Be(0);

            var testList = new List<string>() { "Test1", "Test2", "Test3", "Test4", "Test5" };
            var addedList = testList.Select(item => _blacklistDbRepository.SaveBlacklist(item)).ToList();
            Assert.IsTrue(addedList.Count == testList.Count);
            testList.ForEach(item => Assert.IsNotNull(_blacklistDbRepository.FindBlacklistByName(item)));

            blacklists = _blacklistDbRepository.ReadBlacklists();
            Assert.IsTrue(blacklists.Count() == testList.Count);
            Assert.IsTrue(systemBlacklists.Count() == systemCount);
            testList.ForEach(item => Assert.IsTrue(BlacklistContains(blacklists, item)));
        }

        //// DeleteBlacklist tests

        [TestCase(FakeBlacklistDbRepository.ExplorerString)]
        [TestCase(FakeBlacklistDbRepository.NemeioString)]
        public void BlacklistDbRepository_06_01_DeleteBlacklist_SystemBlacklist_ReturnsFalse(string name)
        {
            var systemItem = _blacklistDbRepository.FindBlacklistByName(name);
            Assert.IsNotNull(systemItem);
            Assert.IsFalse(_blacklistDbRepository.DeleteBlacklist(systemItem.Id));
        }

        [Test]
        public void BlacklistDbRepository_06_01_DeleteBlacklist_SystemBlacklist_Nonexistent()
        {
            Assert.IsFalse(_blacklistDbRepository.DeleteBlacklist(0));
        }

        [Test]
        public void BlacklistDbRepository_06_02_DeleteBlacklist_NonSystemBlacklist_WorksOk()
        {
            var testItem = _blacklistDbRepository.FindBlacklistByName(TestString);
            Assert.IsNull(testItem);
            testItem = _blacklistDbRepository.SaveBlacklist(TestString);
            Assert.IsNotNull(testItem);

            Assert.IsTrue(_blacklistDbRepository.DeleteBlacklist(testItem.Id));
            Assert.IsNull(_blacklistDbRepository.FindBlacklistById(testItem.Id));
            Assert.IsNull(_blacklistDbRepository.FindBlacklistByName(TestString));
        }

        //// private utilities

        private bool BlacklistContains(IEnumerable<Blacklist> blacklist, string name)
        {
            return blacklist.FirstOrDefault(b => b.Name.Equals(name)) != null;
        }
    }
}
