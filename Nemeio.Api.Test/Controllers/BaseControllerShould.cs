using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MvvmCross.Platform.IoC;
using MvvmCross.Test.Core;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Test.Fakes;
using Nemeio.Core;
using Nemeio.Core.Applications;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Managers;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Systems;
using Nemeio.Core.Test.Fakes;
using Nemeio.Platform.Windows;
using Nemeio.Wpf;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    public abstract class BaseControllerShould : MvxIoCSupportingTest
    {
        protected System.Net.Http.HttpClient _client;
        protected WebServer _webserver;
        protected FakeLayoutDbRepository _fakeLayoutDbRepository;
        protected FakeLayoutGenService _fakeLayoutGenService;
        protected FakeCategoryDbRepository _fakeCategoryDbRepository;
        protected FakeCategoryDbRepository _forceFakeCategoryDbRepositoryInstance;
        protected FakeBlacklistDbRepository _fakeBlacklistDbRepository;
        protected FakeInformationService _fakeInformationService;
        protected FakeNemeioHttpService _fakeNemeioHttpService;
        protected ILanguageManager _languageManager;
        protected ILayoutLibrary _layoutLibrary;
        protected ILayoutFacade _layoutFacade;
        protected ISystem _system;

        public virtual void IocSetup(IMvxIoCProvider iocProvider, ILoggerFactory loggerFactory)
        {
        }

        /// <summary>
        /// Automatically called at test's startup
        /// </summary>
        protected override void AdditionalSetup()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockAugmentedLayoutImageProvider = Mock.Of<IAugmentedLayoutImageProvider>();

            var mockLayoutValidityChecker = Mock.Of<ILayoutValidityChecker>();
            //  We don't want to check layout integrity here
            Mock.Get(mockLayoutValidityChecker)
                .Setup(x => x.Check(It.IsAny<ILayout>()))
                .Returns(true);

            _fakeInformationService = new FakeInformationService();
            _fakeLayoutDbRepository = new FakeLayoutDbRepository();
            _fakeLayoutGenService = new FakeLayoutGenService();
            _fakeBlacklistDbRepository = new FakeBlacklistDbRepository();

            _layoutFacade = Mock.Of<ILayoutFacade>();

            var fontProvider = Mock.Of<IFontProvider>();

            _languageManager = Mock.Of<ILanguageManager>();
            _layoutLibrary = Mock.Of<ILayoutLibrary>();
            _system = Mock.Of<ISystem>();
            var applicationSettingsDbRepository = new FakeApplicationSettingsDbRepository();

            Mock.Get(_languageManager)
                .Setup(x => x.GetSupportedLanguages())
                .Returns(new List<CultureInfo>()
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-FR")
                });

            if (_forceFakeCategoryDbRepositoryInstance != null)
            {
                _fakeCategoryDbRepository = _forceFakeCategoryDbRepositoryInstance;
            }
            else
            {
                _fakeCategoryDbRepository = new FakeCategoryDbRepository();
            }

            Ioc.RegisterSingleton<IAugmentedLayoutImageProvider>(mockAugmentedLayoutImageProvider);
            Ioc.RegisterSingleton<ILayoutLibrary>(_layoutLibrary);
            Ioc.RegisterSingleton<ICategoryDbRepository>(_fakeCategoryDbRepository);
            Ioc.RegisterSingleton<IBlacklistDbRepository>(_fakeBlacklistDbRepository);
            Ioc.RegisterSingleton<ILayoutDbRepository>(_fakeLayoutDbRepository);
            Ioc.RegisterSingleton<ILayoutImageGenerator>(_fakeLayoutGenService);
            Ioc.RegisterSingleton<ILoggerFactory>(new LoggerFactory());
            Ioc.RegisterSingleton<IInformationService>(_fakeInformationService);
            Ioc.RegisterSingleton<IErrorManager>(new ErrorManager());
            Ioc.RegisterSingleton<IFontProvider>(fontProvider);
            Ioc.RegisterSingleton<ILanguageManager>(_languageManager);
            Ioc.RegisterType<ILayoutExportService, LayoutExportService>();
            Ioc.RegisterSingleton<IApplicationSettingsProvider>(new ApplicationSettingsProvider(applicationSettingsDbRepository));
            Ioc.RegisterSingleton<IApplicationLayoutManager>(new ApplicationLayoutManager(loggerFactory, _fakeBlacklistDbRepository, _layoutLibrary, mockErrorManager));
            Ioc.RegisterSingleton(_layoutFacade);
            Ioc.RegisterSingleton<IKeyboardMapFactory>(new WinKeyboardMapFactory());
            Ioc.RegisterSingleton<ISystem>(_system);
            IocSetup(Ioc, loggerFactory);

            if (_webserver == null)
            {
                var handler = new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls12
                };
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    //  Always accept https connection.
                    return true;
                };

                var mockSettings = Mock.Of<ISettingsHolder>();

                _client = new System.Net.Http.HttpClient(handler);

                _webserver = new WebServer(loggerFactory, mockSettings);
                _webserver.Start();
            }

            _fakeNemeioHttpService = new FakeNemeioHttpService();
            _fakeNemeioHttpService.WebServer = _webserver;
            Ioc.RegisterSingleton<INemeioHttpService>(_fakeNemeioHttpService);
        }

        [OneTimeTearDownAttribute]
        public void GlobalTearDown()
        {
            _client.Dispose();
            _client = null;

            Task.Run(async () =>
            {
                await _webserver.StopAsync();
            });
        }

        public string GetServerUrl() => _webserver.ConfiguratorUrl;

        protected static Stream GetStream(string str)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(str);
            streamWriter.Flush();
            stream.Position = 0;
            return stream;
        }

        protected async Task<HttpResponseMessage> GetRequestAsync(string partialUrl)
        {
            var url = $"{GetServerUrl()}/{partialUrl}";

            return await _client.GetAsync(url);

        }
        protected async Task<HttpResponseMessage> PostRequestAsync<T>(string partialUrl, T data)
        {
            var url = $"{GetServerUrl()}/{partialUrl}";

            var serializedData = JsonConvert.SerializeObject(data);

            using (var content = CreateJsonContent(serializedData))
            {
                return await _client.PostAsync(url, content);
            }
        }

        protected async Task<HttpResponseMessage> PostFileAsync<T>(string partialUrl, T data, string fileName)
        {
            var url = $"{GetServerUrl()}/{partialUrl}";

            using (var formData = CreateFileContent(data, fileName))
            {
                return await _client.PostAsync(url, formData);
            }
        }

        protected StringContent CreateJsonContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private MultipartFormDataContent CreateFileContent<T>(T data, string fileName)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var binary = Encoding.UTF8.GetBytes(serializedData);

            var memoryStream = new MemoryStream(binary);
            HttpContent fileStreamContent = new StreamContent(memoryStream);

            var formData = new MultipartFormDataContent
            {
                { fileStreamContent, "file", fileName },
            };

            return formData;
        }

        protected async Task<ErrorOutDto<T>> CheckRequestIsError<T>(HttpResponseMessage responseMessage, ErrorCode errorCode)
        {
            var result = await responseMessage.Content.ReadAsStringAsync();
            var errorOutDto = JsonConvert.DeserializeObject<ErrorOutDto<T>>(result);

            var expectedStatusCode = HttpStatusCode.InternalServerError;
            if (errorCode == ErrorCode.Success)
            {
                expectedStatusCode = HttpStatusCode.OK;
            }

            responseMessage.StatusCode.Should().Be(expectedStatusCode);
            errorOutDto.ErrorCode.Should().Be(errorCode);

            return errorOutDto;
        }

        protected Task<ErrorOutDto<T>> CheckRequestIsSuccess<T>(HttpResponseMessage responseMessage) => CheckRequestIsError<T>(responseMessage, ErrorCode.Success);
    }
}
