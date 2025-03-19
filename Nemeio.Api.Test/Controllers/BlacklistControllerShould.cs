using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Dto.Out.Blacklists;
using Nemeio.Core.Errors;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    [TestFixture]
    public class BlacklistControllerShould : BaseControllerShould
    {
        [Test]
        public async Task BlacklistController_01_01_GetBlacklists_FakeSetup_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;

            // check result
            foreach (var blacklist in _fakeBlacklistDbRepository.Blacklists)
            {
                Assert.IsNotNull(blacklists.First(item => item.Name.Equals(blacklist.Name)));
            }
        }

        [Test]
        public async Task BlacklistController_02_01_PostBlacklists_ExistingItem_ReturnsItem()
        {
            base.Setup();

            // inquire current content and get last stored name
            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            // check result
            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;

            Assert.IsNotEmpty(blacklists);

            // and now try to add again existing name
            var existingName = blacklists.Last().Name;
            url = $"{GetServerUrl()}/api/blacklists";
            response = await _client.PostAsync(url + "/" + existingName, null);

            // check result
            var postResult = await CheckRequestIsSuccess<BlacklistApiOutDto>(response);
            var blacklist = postResult.Result;

            Assert.IsTrue(blacklist.Name.Equals(existingName));
        }

        [Test]
        public async Task BlacklistController_02_02_PostBlacklists_NewName_ReturnsItem()
        {
            base.Setup();

            // inquire current content and check new name is not present
            string newName = "test";
            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            // check result
            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;
            Assert.IsNull(blacklists.FirstOrDefault(item => item.Name.Equals(newName)));

            // now check proper adding
            url = $"{GetServerUrl()}/api/blacklists";
            response = await _client.PostAsync(url + "/" + newName, null);

            // check result
            var postResult = await CheckRequestIsSuccess<BlacklistApiOutDto>(response);
            var blacklist = postResult.Result;

            Assert.IsTrue(blacklist.Name.Equals(newName));
        }

        [Test]
        public async Task BlacklistController_03_01_DeleteBlacklists_SystemItem_Forbidden()
        {
            base.Setup();

            // inquire current content and get one system item
            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            // check result

            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;
            Assert.IsNotEmpty(blacklists);

            var systemItem = blacklists.FirstOrDefault(item => item.IsSystem == true);
            Assert.IsNotNull(systemItem);

            // and now try to delete that system Item
            url = $"{GetServerUrl()}/api/blacklists";
            response = await _client.DeleteAsync(url + "/" + systemItem.Id.ToString());

            // check result
            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiDeleteBlacklistSystemForbidden);
        }

        [Test]
        public async Task BlacklistController_03_02_DeleteBlacklists_NonExistingId_NotFound()
        {
            base.Setup();

            // inquire current content and get one system item
            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            // check result
            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;
            Assert.IsNotEmpty(blacklists);

            var idList = blacklists.Select(item => item.Id).ToList();
            idList.Sort();

            var invalidId = idList.Last() + 1;

            // and now try to delete that system Item
            url = $"{GetServerUrl()}/api/blacklists";
            response = await _client.DeleteAsync(url + "/" + invalidId.ToString());

            // check result
            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiDeleteBlacklistIdNotFound);
        }

        [Test]
        public async Task BlacklistController_03_03_DeleteBlacklists_ValidId_DeletesOk()
        {
            base.Setup();

            // inquire current content and get one system item
            var url = $"{GetServerUrl()}/api/blacklists";
            var response = await _client.GetAsync(url);

            // check result
            var result = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var blacklists = result.Result.Blacklists;
            Assert.IsNotEmpty(blacklists);

            var idList = blacklists.Select(item => item.Id).ToList();
            idList.Sort();

            var deleteId = idList[idList.Count /2];

            // and now try to delete that system Item
            url = $"{GetServerUrl()}/api/blacklists";
            response = await _client.DeleteAsync(url + "/" + deleteId.ToString());

            // check result
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var newResult = await CheckRequestIsSuccess<BlacklistsApiOutDto>(response);
            var newBlacklists = newResult.Result.Blacklists;
            Assert.IsNotEmpty(newBlacklists);

            idList = newBlacklists.Select(item => item.Id).ToList();
            Assert.IsFalse(idList.Contains(deleteId));
        }
    }
}
