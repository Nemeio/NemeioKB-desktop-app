using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Test.Dummy;
using Nemeio.Core.Errors;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    [TestFixture]
    public class CategoryControllerShould : BaseControllerShould
    {
        [Test]
        public async Task PostNewCategory()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories";
            var postData = "{ \"index\": 8, \"title\": \"my category from postman\" }";
            var beforePostCount = _fakeCategoryDbRepository.Categories.Count();

            var jsonContent = CreateJsonContent(postData);
            var response = await _client.PostAsync(url, jsonContent);

            var afterPostCount = _fakeCategoryDbRepository.Categories.Count();

            await CheckRequestIsSuccess<BaseOutDto>(response);

            _fakeCategoryDbRepository.SaveCalled.Should().BeTrue();
            beforePostCount.Should().BeLessThan(afterPostCount);
        }

        [Test]
        public async Task PostNewCategoryWithBadFormat()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories";
            var postData = "{ \"toto\": 8, \"title\": \"my category from postman\" }";
            var beforePostCount = _fakeCategoryDbRepository.Categories.Count();

            using (var stream = GetStream(postData))
            {
                var streamContent = new StreamContent(stream);

                var response = await _client.PostAsync(url, streamContent);

                var afterPostCount = _fakeCategoryDbRepository.Categories.Count();

                _fakeCategoryDbRepository.SaveCalled.Should().BeFalse();
                response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
                beforePostCount.Should().Be(afterPostCount);
            }
        }

        [Test]
        public async Task GetCategoryById()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/1";

            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _fakeCategoryDbRepository.FindOneByIdCalled.Should().BeTrue();
        }

        [Test]
        public async Task GetCategoryByIdWithBadId()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/3596";

            var response = await _client.GetAsync(url);

            _fakeCategoryDbRepository.FindOneByIdCalled.Should().BeTrue();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiGetCategoryIdNotFound);
        }

        [Test]
        public async Task GetCategoriesWithAllLayouts()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories?optionLayout=all";

            var response = await _client.GetAsync(url);

            _fakeCategoryDbRepository.ReadAllCalled.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCategoriesWithoutLayouts()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories?optionLayout=none";

            var response = await _client.GetAsync(url);

            _fakeCategoryDbRepository.ReadAllCalled.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCategoriesWithOptionLayoutsHaveBadItem()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories?optionLayout=fake";

            var response = await _client.GetAsync(url);

            _fakeCategoryDbRepository.ReadAllCalled.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task PutCategory()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/1";

            var putData = "{ \"index\": 8, \"title\": \"my category from postman\" }";
            var beforePostCount = _fakeCategoryDbRepository.Categories.Count();

            var jsonContent = CreateJsonContent(putData);
            var response = await _client.PutAsync(url, jsonContent);

            var afterPostCount = _fakeCategoryDbRepository.Categories.Count();

            _fakeCategoryDbRepository.UpdateCalled.Should().BeTrue();
            beforePostCount.Should().Be(afterPostCount);

            await CheckRequestIsSuccess<BaseOutDto>(response);
        }

        [Test]
        public async Task PutCategoryWithEmptyDatabase()
        {
            //  GOAL : Replace only for this one
            //  WARNING : Instance need to be removed at the end
            _forceFakeCategoryDbRepositoryInstance = new DummyCategoryDbRepository();

            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/1";
            var putData = "{ \"index\": 8, \"title\": \"my category from postman\" }";
            var jsonContent = CreateJsonContent(putData);

            var response = await _client.PutAsync(url, jsonContent);

            _fakeCategoryDbRepository.UpdateCalled.Should().BeFalse();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiPutCategoryNotFound);

            _forceFakeCategoryDbRepositoryInstance = null;
        }

        [Test]
        public async Task PutCategoryWithoutId()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories";

            var putData = "{ \"index\": 8, \"title\": \"my category from postman\" }";
            var beforePostCount = _fakeCategoryDbRepository.Categories.Count();

            using (var stream = GetStream(putData))
            {
                var streamContent = new StreamContent(stream);

                var response = await _client.PutAsync(url, streamContent);

                var afterPostCount = _fakeCategoryDbRepository.Categories.Count();

                _fakeCategoryDbRepository.UpdateCalled.Should().BeFalse();
                response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
                beforePostCount.Should().Be(afterPostCount);
            }
        }

        [Test]
        public async Task PutCategoryWithBadFormat()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/1";

            var putData = "{ \"toto\": 8, \"title\": \"my category from postman\" }";
            var beforePostCount = _fakeCategoryDbRepository.Categories.Count();
            var jsonContent = CreateJsonContent(putData);

            var response = await _client.PutAsync(url, jsonContent);

            var afterPostCount = _fakeCategoryDbRepository.Categories.Count();

            _fakeCategoryDbRepository.UpdateCalled.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            beforePostCount.Should().Be(afterPostCount);
        }

        [Test]
        public async Task DeleteCategory()
        {
            base.Setup();

            var beforeDeleteCount = _fakeCategoryDbRepository.Categories.Count();
            var url = $"{GetServerUrl()}/api/categories/1";

            var response = await _client.DeleteAsync(url);
            var afterDeleteCount = _fakeCategoryDbRepository.Categories.Count();

            _fakeCategoryDbRepository.FindOneByIdCalled.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            beforeDeleteCount.Should().BeGreaterThan(afterDeleteCount);
        }

        [Test]
        public async Task DeleteCategoryWithoutId()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories";

            var response = await _client.DeleteAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task DeleteCategoryWithBadId()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/categories/5000";

            var response = await _client.DeleteAsync(url);

            _fakeCategoryDbRepository.FindOneByIdCalled.Should().BeTrue();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiDeleteCategoryIdNotFound);
        }
    }
}
