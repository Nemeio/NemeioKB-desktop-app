using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Dto.Out.Layout;
using Nemeio.Api.Dto.Out.Templates;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Test.Fakes;
using Nemeio.LayoutGen;
using Nemeio.Platform.Windows;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemeio.Api.Test.Controllers
{
    class LayoutControllerShould : BaseControllerShould
    {
        public static LayoutId FakeLayoutId1 = new LayoutId("8667DF98-1529-4A76-B846-78F5BFE57B2F");
        public static LayoutId FakeLayoutId2 = new LayoutId("8667DD98-1529-4A76-B846-78F5B1E57B4D");
        public static LayoutId FakeLayoutId3 = new LayoutId("8667DD98-1529-5894-B846-78F5B1E57B7B");
        public static LayoutId FakeLayoutId4 = new LayoutId("6357DD98-7859-5574-B846-78F5B1E57B2F");


        public static string FakeLayoutTitle1 = "Layout1";
        public static string FakeLayoutSubtitle1 = "Subtitle Layout1";
        public static string FakeLayoutTitle2 = "Layout2";
        public static string FakeLayoutSubtitle2 = "Subtitle Layout2";
        public static string FakeLayoutTitle3 = "Layout3";
        public static string FakeLayoutSubtitle3 = "Subtitle Layout3";
        public static string FakeLayoutTitle4 = "Layout4";
        public static string FakeLayoutSubtitle4 = "Subtitle Layout4";

        private ILayout fakeLayout1;

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();

            var keys = new List<Key>();

            for (int i = 0; i < 81; i++)
            {
                var newKey = new Key()
                {
                    Index = i,
                    Actions = new List<KeyAction>()
                    {
                        new KeyAction()
                        {
                            Display = "A",
                            Modifier = KeyboardModifier.None,
                            Subactions = new List<KeySubAction>()
                            {
                                new KeySubAction("A", KeyActionType.Unicode)
                            }
                        }
                    },
                    Disposition = KeyDisposition.Full
                };

                keys.Add(newKey);
            }

            var loggerFactory = new LoggerFactory();
            var document = Mock.Of<IDocument>();
            var settingsHolder = Mock.Of<ISettingsHolder>();
            var keyboardMapFactory = new WinKeyboardMapFactory();
            var fontProvider = new FontProvider(loggerFactory, new ErrorManager(), null);
            var jpegPackageBuilder = new JpegImagePackageBuilder();
            var jpegRenderer = Registerer.CreateJpegRenderer(loggerFactory, document, fontProvider, jpegPackageBuilder);
            var oneBppRenderer = Registerer.CreateOneBppRenderer(loggerFactory, document, fontProvider);

            var screenFactory = new ScreenFactory(keyboardMapFactory, jpegRenderer, oneBppRenderer, settingsHolder);

            var screen = screenFactory.CreateEinkScreen();
            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                FontProvider.GetDefaultFont(),
                screen
            );

            fakeLayout1 = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(new OsLayoutId(""), false, false),
                layoutImageInfo,
                new byte[0],
                123,
                0,
                FakeLayoutTitle1,
                FakeLayoutSubtitle1,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId1,
                null,
                false,
                true
            );

            var layout2 = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(new OsLayoutId(""), false, false),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle2,
                FakeLayoutSubtitle2,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId2,
                null,
                false,
                true
            );

            var layout3 = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(new OsLayoutId(""), false, false, new List<string>() { @"C:\Programmes\Toto\toto.exe" }, true, true, true),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle3,
                FakeLayoutSubtitle3,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId3,
                null,
                false,
                true
            );

            var layout4 = new Nemeio.Core.Services.Layouts.Layout(
                new LayoutInfo(new OsLayoutId(""), false, true, new List<string>(), false, true, true),
                layoutImageInfo,
                new byte[0],
                12,
                0,
                FakeLayoutTitle4,
                FakeLayoutSubtitle4,
                DateTime.Now,
                DateTime.Now,
                keys,
                FakeLayoutId4,
                null,
                false,
                true
            );

            var layouts = new List<ILayout>() { fakeLayout1, layout2, layout3, layout4 };

            Mock.Get(_layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(layouts);
        }

        [Test]
        public async Task LayoutController_GetLayoutByIdWithAllKeys()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/{FakeLayoutId1}?optionKey=all";

            var response = await _client.GetAsync(url);
            var responseResult = await CheckRequestIsSuccess<LayoutApiOutDto>(response);

            if (responseResult.Result != null && responseResult.Result is LayoutApiOutDto responseJson)
            {
                responseJson.Keys.Should().NotBeNull();
                responseJson.Keys.First().Disposition.Should().NotBeNull();
            }
            else
            {
                Assert.Fail("Response result is null or not conform to expected body");
            }
        }

        [Test]
        public async Task LayoutController_GetLayoutByIdWithoutKeys()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/{FakeLayoutId1}?optionKey=none";

            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LayoutController_GetLayoutByIdWithoutOptionKeyParams()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8667DF98-1529-4A76-B846-78F5BFE57B2F";

            var response = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiGetLayoutOptionInvalid);
        }

        [Test]
        public async Task LayoutController_GetLayoutByIdWithBadOptionKey()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8667DF98-1529-4A76-B846-78F5BFE57B2F?optionKey=fakeValue";

            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_GetLayoutWithBadId()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8667DF98-1527-4A76-B846-78F5BFE57B2F?optionKey=all";

            var response = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiGetLayoutIdNotFound);
        }

        [Test]
        public async Task LayoutController_GetLayouts()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts";

            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LayoutController_PostLayout_WorksOk()
        {
            base.Setup();

            const string newLayoutTitle = "Test";

            var url = $"{GetServerUrl()}/api/layouts";
            var fakeLayoutJson = "{ 'templateId': '" + FakeLayoutDbRepository.FakeLayoutId4 + "', 'title': '" + newLayoutTitle + "' }";
            var jsonContent = CreateJsonContent(fakeLayoutJson);

            var httpResponse = await _client.PostAsync(url, jsonContent);

            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<LayoutApiOutDto>(responseBody);

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            responseJson.IsDefault.Should().BeFalse();
            responseJson.IsFactory.Should().BeFalse();
            responseJson.IsHid.Should().BeFalse();
        }

        [Test]
        public async Task LayoutController_PostLayout_WithWrongData_ReturnBadRequest()
        {
            base.Setup();

            var unknownCategoryId = 5936;

            var url = $"{GetServerUrl()}/api/layouts";
            var fakeJson = "{'id':'af4eb15p-809e-ccaf-d0f7-02740329cb70','default':0,'hid':0,'dateCreationd':1559748866279,'dateUpdate':1559748866279,'categoryId':" + unknownCategoryId + ",'title':'my new title','width':1496,'height':624,'keys':[{'Index':0,'Width':75,'Height':75,'X':5,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Ctrl','Subactions':[{'Data':'Ctrl','Type':2}]}]},{'Index':1,'Width':75,'Height':75,'X':99,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Alt','Subactions':[{'Data':'Alt','Type':2}]}]},{'Index':2,'Width':75,'Height':75,'X':193,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'AltGr','Subactions':[{'Data':'AltGr','Type':2}]}]},{'Index':3,'Width':75,'Height':75,'X':287,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Suppr','Subactions':[{'Data':'Del','Type':2}]}]},{'Index':4,'Width':75,'Height':75,'X':381,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Maj','Subactions':[{'Data':'Shift','Type':2}]}]},{'Index':5,'Width':75,'Height':75,'X':475,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':6,'Width':75,'Height':75,'X':569,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':7,'Width':75,'Height':75,'X':663,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':8,'Width':75,'Height':75,'X':757,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':9,'Width':75,'Height':75,'X':851,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':10,'Width':75,'Height':75,'X':945,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':11,'Width':75,'Height':75,'X':1039,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':12,'Width':75,'Height':75,'X':1133,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':13,'Width':75,'Height':75,'X':1227,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':14,'Width':75,'Height':75,'X':1321,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':15,'Width':75,'Height':75,'X':1415,'Y':0,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':16,'Width':83,'Height':83,'X':9,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':17,'Width':83,'Height':83,'X':111,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'S+AGr','Subactions':[{'Data':'Shift','Type':2},{'Data':'AltGr','Type':2}]}]},{'Index':18,'Width':83,'Height':83,'X':213,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'emb://windows.svg','Subactions':[{'Data':'Os','Type':2}]}]},{'Index':19,'Width':83,'Height':83,'X':315,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Toto','Subactions':[{'Data':'Toto','Type':1}]}]},{'Index':20,'Width':83,'Height':83,'X':417,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Smile','Subactions':[{'Data':'ὠD','Type':1}]}]},{'Index':21,'Width':83,'Height':83,'X':519,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'LDLC','Subactions':[{'Data':'http://www.ldlc.com','Type':4}]}]},{'Index':22,'Width':83,'Height':83,'X':621,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Lock','Subactions':[{'Data':'Os','Type':2},{'Data':'l','Type':1}]}]},{'Index':23,'Width':83,'Height':83,'X':723,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'l','Subactions':[{'Data':'l','Type':1}]}]},{'Index':24,'Width':83,'Height':83,'X':825,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'k','Subactions':[{'Data':'k','Type':1}]}]},{'Index':25,'Width':83,'Height':83,'X':927,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'np++','Subactions':[{'Data':'C:\\\\Program Files (x86)\\\\Notepad++\\\\notepad++.exe','Type':3}]}]},{'Index':26,'Width':83,'Height':83,'X':1029,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'¨','Subactions':[{'Data':'¨','Type':1}]}]},{'Index':27,'Width':83,'Height':83,'X':1131,'Y':122,'Background':0,'Actions':[{'Modifier':1,'Font':null,'Display':'A','Subactions':[{'Data':'A','Type':1}]},{'Modifier':0,'Font':null,'Display':'','Subactions':[{'Data':'a','Type':1}]},{'Modifier':2,'Font':null,'Display':'@','Subactions':[{'Data':'@','Type':1}]}]},{'Index':28,'Width':83,'Height':83,'X':1233,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Ð±','Subactions':[{'Data':'Ð±','Type':1}]}]},{'Index':29,'Width':166,'Height':83,'X':1335,'Y':122,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'emb://backspace.svg','Subactions':[{'Data':'BKSP','Type':2}]}]},{'Index':30,'Width':166,'Height':83,'X':0,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'Multi','Subactions':[{'Data':'multiple action','Type':1},{'Data':'http://witek.io','Type':4},{'Data':'http://www.ldlc.com','Type':4},{'Data':'C:\\\\Program Files\\\\DB Browser for SQLite\\\\DB Browser for SQLite.exe','Type':3}]}]},{'Index':31,'Width':83,'Height':83,'X':179,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':32,'Width':83,'Height':83,'X':281,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':33,'Width':83,'Height':83,'X':383,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':34,'Width':83,'Height':83,'X':485,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':35,'Width':83,'Height':83,'X':587,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':36,'Width':83,'Height':83,'X':689,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':37,'Width':83,'Height':83,'X':791,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':38,'Width':83,'Height':83,'X':893,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':39,'Width':83,'Height':83,'X':995,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':40,'Width':83,'Height':83,'X':1097,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':41,'Width':83,'Height':83,'X':1199,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':42,'Width':83,'Height':83,'X':1301,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':43,'Width':83,'Height':83,'X':1403,'Y':224,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':44,'Width':176,'Height':83,'X':0,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':45,'Width':83,'Height':83,'X':190,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':46,'Width':83,'Height':83,'X':292,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':47,'Width':83,'Height':83,'X':394,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':48,'Width':83,'Height':83,'X':496,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':49,'Width':83,'Height':83,'X':598,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':50,'Width':83,'Height':83,'X':700,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':51,'Width':83,'Height':83,'X':802,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':52,'Width':83,'Height':83,'X':904,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':53,'Width':83,'Height':83,'X':1006,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':54,'Width':83,'Height':83,'X':1108,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':55,'Width':83,'Height':83,'X':1210,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':56,'Width':176,'Height':83,'X':1312,'Y':326,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':57,'Width':166,'Height':83,'X':0,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':58,'Width':83,'Height':83,'X':179,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':59,'Width':83,'Height':83,'X':281,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':60,'Width':83,'Height':83,'X':383,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':61,'Width':83,'Height':83,'X':485,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':62,'Width':83,'Height':83,'X':587,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':63,'Width':83,'Height':83,'X':689,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':64,'Width':83,'Height':83,'X':791,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':65,'Width':83,'Height':83,'X':893,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':66,'Width':83,'Height':83,'X':995,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':67,'Width':83,'Height':83,'X':1097,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':68,'Width':83,'Height':83,'X':1199,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':69,'Width':83,'Height':83,'X':1301,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':70,'Width':83,'Height':83,'X':1403,'Y':428,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':71,'Width':83,'Height':83,'X':0,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':72,'Width':83,'Height':83,'X':102,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':73,'Width':83,'Height':83,'X':204,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':74,'Width':83,'Height':83,'X':306,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':75,'Width':525,'Height':83,'X':408,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':76,'Width':83,'Height':83,'X':952,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':77,'Width':83,'Height':83,'X':1054,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':78,'Width':83,'Height':83,'X':1199,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':79,'Width':83,'Height':83,'X':1301,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]},{'Index':80,'Width':83,'Height':83,'X':1403,'Y':530,'Background':0,'Actions':[{'Modifier':0,'Font':null,'Display':'a','Subactions':[{'Data':'a','Type':1}]},{'Modifier':1,'Font':null,'Display':'b','Subactions':[{'Data':'b','Type':1}]},{'Modifier':2,'Font':null,'Display':'c','Subactions':[{'Data':'c','Type':1}]},{'Modifier':3,'Font':null,'Display':'d','Subactions':[{'Data':'d','Type':1}]}]}]}";
            var jsonContent = CreateJsonContent(fakeJson);

            var resp = await _client.PostAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_PostLayout_WithUnknownCategory_ReturnBadRequest()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts";
            var fakeJson = "{'fakeKey': 33}";
            var jsonContent = CreateJsonContent(fakeJson);

            var resp = await _client.PostAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_PutLayout_WorksOk()
        {
            base.Setup();

            const string layoutId = "8667DF98-1529-4A76-B846-78F5BFE57B2F";
            const string newLayoutTitle = "Test_Put";

            var url = $"{GetServerUrl()}/api/layouts/{layoutId}";
            var fakeLayoutJson = "{ 'title': '" + newLayoutTitle + "' }";
            var jsonContent = CreateJsonContent(fakeLayoutJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            await _client.PutAsync(url, jsonContent);

            updateLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task LayoutController_PutLayout_WithWrongLayoutId_ReturnErrorCode()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/4567DF98-1529-4A76-B844-78F5BFE57B2F";
            var fakeLayoutJson = "{'index':0,'title':'Test','categoryId':123,'isEnable':true,'width':1496,'height':624,'isDarkMode':0,'keys':[{'index':0,'width':75,'height':75,'x':5,'y':0,'isDarkMode':1,'actions':[{'display':'New','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'n','type':1}]}]},{'index':1,'width':75,'height':75,'x':99,'y':0,'isDarkMode':1,'actions':[{'display':'Open','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'o','type':1}]}]},{'index':2,'width':75,'height':75,'x':193,'y':0,'isDarkMode':1,'actions':[{'display':'Diapo+','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'m','type':1}]}]},{'index':3,'width':75,'height':75,'x':287,'y':0,'isDarkMode':1,'actions':[{'display':'Help','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'F1','type':2}]}]},{'index':4,'width':75,'height':75,'x':381,'y':0,'isDarkMode':1,'actions':[{'display':'Start','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'F5','type':2}]}]},{'index':5,'width':75,'height':75,'x':475,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':6,'width':75,'height':75,'x':569,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':7,'width':75,'height':75,'x':663,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':8,'width':75,'height':75,'x':757,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':9,'width':75,'height':75,'x':851,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':10,'width':75,'height':75,'x':945,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':11,'width':75,'height':75,'x':1039,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':12,'width':75,'height':75,'x':1133,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':13,'width':75,'height':75,'x':1227,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':14,'width':75,'height':75,'x':1321,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':15,'width':75,'height':75,'x':1415,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':16,'width':83,'height':83,'x':9,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':17,'width':83,'height':83,'x':111,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':18,'width':83,'height':83,'x':213,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':19,'width':83,'height':83,'x':315,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':20,'width':83,'height':83,'x':417,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':21,'width':83,'height':83,'x':519,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':22,'width':83,'height':83,'x':621,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':23,'width':83,'height':83,'x':723,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':24,'width':83,'height':83,'x':825,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':25,'width':83,'height':83,'x':927,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':26,'width':83,'height':83,'x':1029,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':27,'width':83,'height':83,'x':1131,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':28,'width':83,'height':83,'x':1233,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':29,'width':166,'height':83,'x':1335,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':30,'width':166,'height':83,'x':0,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':31,'width':83,'height':83,'x':179,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':32,'width':83,'height':83,'x':281,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':33,'width':83,'height':83,'x':383,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':34,'width':83,'height':83,'x':485,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':35,'width':83,'height':83,'x':587,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':36,'width':83,'height':83,'x':689,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':37,'width':83,'height':83,'x':791,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':38,'width':83,'height':83,'x':893,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':39,'width':83,'height':83,'x':995,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':40,'width':83,'height':83,'x':1097,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':41,'width':83,'height':83,'x':1199,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':42,'width':83,'height':83,'x':1301,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':43,'width':83,'height':83,'x':1403,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':44,'width':176,'height':83,'x':0,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':45,'width':83,'height':83,'x':190,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':46,'width':83,'height':83,'x':292,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':47,'width':83,'height':83,'x':394,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':48,'width':83,'height':83,'x':496,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':49,'width':83,'height':83,'x':598,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':50,'width':83,'height':83,'x':700,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':51,'width':83,'height':83,'x':802,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':52,'width':83,'height':83,'x':904,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':53,'width':83,'height':83,'x':1006,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':54,'width':83,'height':83,'x':1108,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':55,'width':83,'height':83,'x':1210,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':56,'width':176,'height':83,'x':1312,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':57,'width':166,'height':83,'x':0,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':58,'width':83,'height':83,'x':179,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':59,'width':83,'height':83,'x':281,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':60,'width':83,'height':83,'x':383,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':61,'width':83,'height':83,'x':485,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':62,'width':83,'height':83,'x':587,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':63,'width':83,'height':83,'x':689,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':64,'width':83,'height':83,'x':791,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':65,'width':83,'height':83,'x':893,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':66,'width':83,'height':83,'x':995,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':67,'width':83,'height':83,'x':1097,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':68,'width':83,'height':83,'x':1199,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':69,'width':83,'height':83,'x':1301,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':70,'width':83,'height':83,'x':1403,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':71,'width':83,'height':83,'x':0,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':72,'width':83,'height':83,'x':102,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':73,'width':83,'height':83,'x':204,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':74,'width':83,'height':83,'x':306,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':75,'width':525,'height':83,'x':408,'y':530,'isDarkMode':1,'actions':[{'display':'PPT','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':76,'width':83,'height':83,'x':952,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':77,'width':83,'height':83,'x':1054,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':78,'width':83,'height':83,'x':1199,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':79,'width':83,'height':83,'x':1301,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':80,'width':83,'height':83,'x':1403,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]}],'linkApplicationPath':'','linkApplicationEnable':false, 'associatedId':''}";
            var jsonContent = CreateJsonContent(fakeLayoutJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var response = await _client.PutAsync(url, jsonContent);

            updateLayoutCalled.Should().BeFalse();
            _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiPutLayoutIdNotFound);
        }

        [Test]
        public async Task LayoutController_PutLayout_WithWrongAssociatedLayoutId_ReturnErrorCode()
        {
            base.Setup();

            var targetHid = _layoutLibrary.Layouts.First(x => x.LayoutInfo.Hid);
            var targetCustom = _layoutLibrary.Layouts.First(x => !x.LayoutInfo.Hid);
            _layoutLibrary.Layouts.Remove(targetHid);
            targetCustom.AssociatedLayoutId = targetHid.LayoutId;
            var url = $"{GetServerUrl()}/api/layouts/"+targetCustom.LayoutId;
            var fakeLayoutJson = "{'index':0,'title':'Test','categoryId':123,'isEnable':true,'width':1496,'height':624,'isDarkMode':0,'keys':[{'index':0,'width':75,'height':75,'x':5,'y':0,'isDarkMode':1,'actions':[{'display':'New','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'n','type':1}]}]},{'index':1,'width':75,'height':75,'x':99,'y':0,'isDarkMode':1,'actions':[{'display':'Open','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'o','type':1}]}]},{'index':2,'width':75,'height':75,'x':193,'y':0,'isDarkMode':1,'actions':[{'display':'Diapo+','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'Ctrl','type':2},{'data':'m','type':1}]}]},{'index':3,'width':75,'height':75,'x':287,'y':0,'isDarkMode':1,'actions':[{'display':'Help','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'F1','type':2}]}]},{'index':4,'width':75,'height':75,'x':381,'y':0,'isDarkMode':1,'actions':[{'display':'Start','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[{'data':'F5','type':2}]}]},{'index':5,'width':75,'height':75,'x':475,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':6,'width':75,'height':75,'x':569,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':7,'width':75,'height':75,'x':663,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':8,'width':75,'height':75,'x':757,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':9,'width':75,'height':75,'x':851,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':10,'width':75,'height':75,'x':945,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':11,'width':75,'height':75,'x':1039,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':12,'width':75,'height':75,'x':1133,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':13,'width':75,'height':75,'x':1227,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':14,'width':75,'height':75,'x':1321,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':15,'width':75,'height':75,'x':1415,'y':0,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':16,'width':83,'height':83,'x':9,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':17,'width':83,'height':83,'x':111,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':18,'width':83,'height':83,'x':213,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':19,'width':83,'height':83,'x':315,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':20,'width':83,'height':83,'x':417,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':21,'width':83,'height':83,'x':519,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':22,'width':83,'height':83,'x':621,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':23,'width':83,'height':83,'x':723,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':24,'width':83,'height':83,'x':825,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':25,'width':83,'height':83,'x':927,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':26,'width':83,'height':83,'x':1029,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':27,'width':83,'height':83,'x':1131,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':28,'width':83,'height':83,'x':1233,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':29,'width':166,'height':83,'x':1335,'y':122,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':30,'width':166,'height':83,'x':0,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':31,'width':83,'height':83,'x':179,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':32,'width':83,'height':83,'x':281,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':33,'width':83,'height':83,'x':383,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':34,'width':83,'height':83,'x':485,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':35,'width':83,'height':83,'x':587,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':36,'width':83,'height':83,'x':689,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':37,'width':83,'height':83,'x':791,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':38,'width':83,'height':83,'x':893,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':39,'width':83,'height':83,'x':995,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':40,'width':83,'height':83,'x':1097,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':41,'width':83,'height':83,'x':1199,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':42,'width':83,'height':83,'x':1301,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':43,'width':83,'height':83,'x':1403,'y':224,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':44,'width':176,'height':83,'x':0,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':45,'width':83,'height':83,'x':190,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':46,'width':83,'height':83,'x':292,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':47,'width':83,'height':83,'x':394,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':48,'width':83,'height':83,'x':496,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':49,'width':83,'height':83,'x':598,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':50,'width':83,'height':83,'x':700,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':51,'width':83,'height':83,'x':802,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':52,'width':83,'height':83,'x':904,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':53,'width':83,'height':83,'x':1006,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':54,'width':83,'height':83,'x':1108,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':55,'width':83,'height':83,'x':1210,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':56,'width':176,'height':83,'x':1312,'y':326,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':57,'width':166,'height':83,'x':0,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':58,'width':83,'height':83,'x':179,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':59,'width':83,'height':83,'x':281,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':60,'width':83,'height':83,'x':383,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':61,'width':83,'height':83,'x':485,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':62,'width':83,'height':83,'x':587,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':63,'width':83,'height':83,'x':689,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':64,'width':83,'height':83,'x':791,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':65,'width':83,'height':83,'x':893,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':66,'width':83,'height':83,'x':995,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':67,'width':83,'height':83,'x':1097,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':68,'width':83,'height':83,'x':1199,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':69,'width':83,'height':83,'x':1301,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':70,'width':83,'height':83,'x':1403,'y':428,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':71,'width':83,'height':83,'x':0,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':72,'width':83,'height':83,'x':102,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':73,'width':83,'height':83,'x':204,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':74,'width':83,'height':83,'x':306,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':75,'width':525,'height':83,'x':408,'y':530,'isDarkMode':1,'actions':[{'display':'PPT','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':76,'width':83,'height':83,'x':952,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':77,'width':83,'height':83,'x':1054,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':78,'width':83,'height':83,'x':1199,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':79,'width':83,'height':83,'x':1301,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]},{'index':80,'width':83,'height':83,'x':1403,'y':530,'isDarkMode':1,'actions':[{'display':'z','modifier':0,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'y','modifier':1,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'x','modifier':2,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]},{'display':'w','modifier':3,'font':{'name':'Arial','size':16,'bold':false,'italic':false},'subactions':[]}]}],'linkApplicationPath':'','linkApplicationEnable':false, 'associatedId':'" + targetHid.LayoutId + "'}";
            var jsonContent = CreateJsonContent(fakeLayoutJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var response = await _client.PutAsync(url, jsonContent);

            updateLayoutCalled.Should().BeFalse();
            _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiPutAssociatedIdInvalid);
        }

        [Test]
        public async Task LayoutController_PutLayout_WithUnknownCategoryId_ReturnErrorCode()
        {
            base.Setup();

            const string layoutId = "8667DF98-1529-4A76-B846-78F5BFE57B2F";
            const int unknownCategoryId = 9685;
            const string newLayoutTitle = "My_New_Layout_Title";

            var url = $"{GetServerUrl()}/api/layouts/{layoutId}";
            var fakeLayoutJson = "{ 'title':'" + newLayoutTitle + "','categoryId':" + unknownCategoryId + "}";
            var jsonContent = CreateJsonContent(fakeLayoutJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var layoutBeforeUpdate = _layoutLibrary.Layouts.First(x => x.LayoutId.Equals(new LayoutId(layoutId)));
            layoutBeforeUpdate.Should().NotBeNull();
            layoutBeforeUpdate.Title.Should().NotBe(newLayoutTitle);

            var response = await _client.PutAsync(url, jsonContent);

            updateLayoutCalled.Should().BeFalse();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiPutLayoutCategoryIdInvalid);

            var layoutAfterUpdate = _layoutLibrary.Layouts.First(x => x.LayoutId.Equals(new LayoutId(layoutId)));
            layoutAfterUpdate.Should().NotBeNull();
            layoutAfterUpdate.Title.Should().NotBe(newLayoutTitle);
            layoutAfterUpdate.CategoryId.Should().NotBe(unknownCategoryId);
        }

        [Test]
        public async Task LayoutController_PutLayout_WithWrongData_ReturnHttpMethodNotAllowed()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts";
            var fakeLayoutJson = "{'fakeKey':'8667DF98-1529-4A76-B846-78F5B3257B2F'}";

            using (var stream = GetStream(fakeLayoutJson))
            {
                var streamContent = new StreamContent(stream);

                var resp = await _client.PutAsync(url, streamContent);

                _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

                resp.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
            }
        }

        [Test]
        public async Task LayoutController_DeleteLayout_Works()
        {
            base.Setup();

            var newLayout = Mock.Of<ILayout>();
            Mock.Get(newLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId("8667DF98-1529-4A76-B846-78F5BFE57B2F"));
            Mock.Get(newLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        false
                    )
                );

            _layoutLibrary.Layouts.Add(newLayout);

            var removeLayoutAsyncCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.RemoveLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => removeLayoutAsyncCalled = true)
                .Returns(Task.Delay(0));
            Mock.Get(_layoutFacade)
                .Setup(x => x.RemoveLayoutAsync(It.IsAny<LayoutId>()))
                .Callback(() => removeLayoutAsyncCalled = true)
                .Returns(Task.Delay(0));

            var url = $"{GetServerUrl()}/api/layouts/8667DF98-1529-4A76-B846-78F5BFE57B2F";

            var response = await _client.DeleteAsync(url);

            removeLayoutAsyncCalled.Should().BeTrue();
            _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

            await CheckRequestIsSuccess<BaseOutDto>(response);
        }

        [Test]
        public async Task LayoutController_DeleteLayout_WhenLayoutIsHid_ReturnForbidden()
        {
            base.Setup();

            var forbiddenLayout = Mock.Of<ILayout>();
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId("6357DD98-7859-5574-B846-78F5B1E57B2F"));
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        true
                    )
                );

            _layoutLibrary.Layouts.Add(forbiddenLayout);

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-78F5B1E57B2F";

            var response = await _client.DeleteAsync(url);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiDeleteHidLayoutForbidden);
        }

        [Test]
        public async Task LayoutController_DeleteLayout_WithoutId_ReturnMethodNotAllowed()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/";

            var resp = await _client.DeleteAsync(url);

            _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

            resp.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task LayoutController_DeleteLayout_WhenBadId_ReturnNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8457DF98-1529-4A76-B846-78F5BFE54B2F";

            var response = await _client.DeleteAsync(url);

            _fakeLayoutGenService.RenderLayoutFromJsonCalled.Should().BeFalse();

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiDeleteLayoutIdNotFound);
        }

        [Test]
        public async Task LayoutController_GetTemplates_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/templates";

            var httpResponse = await _client.GetAsync(url);
            var response = await CheckRequestIsSuccess<TemplatesOutDto>(httpResponse);

            response.Result.Templates.Count().Should().Be(2);
        }

        [Test]
        public async Task LayoutController_PostLayout_WhenPostUnknownLayoutId_ReturnNotFound()
        {
            base.Setup();

            var newName = "newOne";
            var layoutId = "6357DD98-7859-5574-B846-85F5B1E57B4F".ToLower();

            var url = $"{GetServerUrl()}/api/layouts";
            var data = "{ 'templateId': '" + layoutId + "', 'title': '" + newName + "' }";
            var jsonContent = CreateJsonContent(data);

            var response = await _client.PostAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiPostLayoutTemplateIdNotFound);
        }

        [Test]
        public async Task LayoutController_GetHids_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/hids";
            var resp = await _client.GetAsync(url);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LayoutController_GetCustoms_WorksOk()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/customs";
            var resp = await _client.GetAsync(url);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [TestCase("windows.svg")]
        [TestCase("backspace.svg")]
        [TestCase("mic.svg")]
        [TestCase("return.svg")]
        [TestCase("tab.svg")]
        [TestCase("search.svg")]
        [TestCase("arrowUp.svg")]
        [TestCase("arrowDown.svg")]
        [TestCase("cmd.svg")]
        public async Task LayoutController_GetHidImage_WorksOk(string imageName)
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/hids/image/{imageName}";
            var resp = await _client.GetAsync(url);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [TestCase("this_is_not_an_image_name.svg")]
        [TestCase("00359.svg")]
        [TestCase("windows.png")]
        public async Task LayoutController_GetHidImage_WhenImageNameIsInvalid_ReturnNotFound(string imageName)
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/hids/image/{imageName}";
            var response = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiGetHidImageNotFound);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WorksOk()
        {
            base.Setup();

            const string layoutId = "8667DF98-1529-4A76-B846-78F5BFE57B2F";
            const int keyIndex = 52;

            var url = $"{GetServerUrl()}/api/layouts/{layoutId}/keys/{keyIndex}";
            var fakeJson = "{ 'actions': [], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var resp = await _client.PutAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            updateLayoutCalled.Should().BeFalse();
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WithBadValue_RollbackValue()
        {
            base.Setup();

            const string newDisplayValue = "B";
            const string layoutId = "8667DF98-1529-4A76-B846-78F5BFE57B2F";
            const int keyIndex = 52;
            const string newBadUrl = "blob://this_is_not_an_http_url";

            var url = $"{GetServerUrl()}/api/layouts/{layoutId}/keys/{keyIndex}";
            var fakeJson = "{ 'actions': [{ 'display': '" + newDisplayValue + "', 'subactions': [{ 'data':'" + newBadUrl + "', 'type':4 }] }], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var layoutBeforeUpdate = _layoutLibrary.Layouts.First(x => x.LayoutId.Equals(new LayoutId(layoutId)));
            var layoutKeyBeforeUpdate = layoutBeforeUpdate.Keys.ElementAt(keyIndex);
            layoutKeyBeforeUpdate.Actions.Count().Should().BeGreaterThan(0);
            layoutKeyBeforeUpdate.Disposition.Should().NotBe(KeyDisposition.Single);

            var resp = await _client.PutAsync(url, jsonContent);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var layoutAfterUpdate = _layoutLibrary.Layouts.First(x => x.LayoutId.Equals(new LayoutId(layoutId)));

            var layoutKeyAfterUpdate = layoutAfterUpdate.Keys.ElementAt(keyIndex);
            layoutKeyAfterUpdate.Actions.Count().Should().Be(1);
            layoutKeyAfterUpdate.Disposition.Should().NotBe(KeyDisposition.Single);

            var layoutKeyFirstAction = layoutKeyAfterUpdate.Actions.First();
            layoutKeyFirstAction.Should().NotBeNull();
            layoutKeyFirstAction.Display.Should().NotBe(newDisplayValue);
            layoutKeyFirstAction.Subactions.First().Data.Should().NotBe(newBadUrl);
            layoutKeyFirstAction.Subactions.First().Type.Should().NotBe(4);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WithEmptyLayoutId_ReturnBadRequest()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/ /keys/52";
            var fakeJson = "{ 'actions': [] }";
            var jsonContent = CreateJsonContent(fakeJson);

            var resp = await _client.PutAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WithKeyIdLessThanZero_ReturnBadRequest()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-85F5B1E57B4F/keys/-2";
            var fakeJson = "{ 'actions': [] }";
            var jsonContent = CreateJsonContent(fakeJson);

            var resp = await _client.PutAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WithKeyIdGreaterThanMaxKeyboardKeys_ReturnBadRequest()
        {
            base.Setup();

            var maxNumberofKeys = 81 + 2;

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-85F5B1E57B4F/keys/{maxNumberofKeys}";
            var fakeJson = "{ 'actions': [] }";
            var jsonContent = CreateJsonContent(fakeJson);

            var resp = await _client.PutAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WithUnknownLayoutId_ReturnNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8645DD98-3597-5574-1248-85F5B1E34B4F/keys/0";
            var fakeJson = "{ 'actions': [], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyLayoutNotFound);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_WhenLayoutIsHid_ReturnForbidden()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-78F5B1E57B2F/keys/0";
            var fakeJson = "{ 'actions': [], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyHidLayoutForbidden);
        }

        [Test]
        public async Task LayoutController_ResetLayoutKey_WithEmptyLayoutId_ReturnBadRequest()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/ /reset/52";
            var fakeJson = string.Empty;
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task LayoutController_ResetLayoutKey_WithKeyIdLessThanZero_ReturnNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-85F5B1E57B4F/reset/-2/";
            var fakeJson = string.Empty;
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyLayoutNotFound);
        }

        [Test]
        public async Task LayoutController_ResetLayoutKey_WithKeyIdGreaterThanMaxKeyboardKeys_ReturnNotFound()
        {
            base.Setup();

            var maxNumberofKeys = 81 + 2;

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-85F5B1E57B4F/reset/{maxNumberofKeys}";
            var fakeJson = string.Empty;
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyLayoutNotFound);
        }

        [Test]
        public async Task LayoutController_ResetLayoutKey_WithUnknownLayoutId_ReturnNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8645DD98-3597-5574-1248-85F5B1E34B4F/reset/0";
            var fakeJson = string.Empty;
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyLayoutNotFound);
        }

        [Test]
        public async Task LayoutController_ResetLayoutKey_WhenLayoutIsHid_ReturnForbidden()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-78F5B1E57B2F/reset/0/";
            var fakeJson = "{ 'actions': [], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var response = await _client.PutAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiUpdateLayoutKeyHidLayoutForbidden);
        }

        [Test]
        public async Task LayoutController_CommitUIChanges_WhenIdIsEmpty_ReturnBadRequest()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/ /commit";
            var response = await _client.PostAsync(url, null);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiCommitUiIdInvalid);
        }

        [Test]
        public async Task LayoutController_CommitUIChanges_WhenLayoutIsUnknown_ReturnNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8645DD98-3597-8634-6752-85F5B1E34B4F/commit";
            var response = await _client.PostAsync(url, null);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiCommitUiLayoutNotFound);
        }

        [Test]
        public async Task LayoutController_CommitUIChanges_WhenLayoutIsHid_ReturnFobidden()
        {
            base.Setup();

            var forbiddenLayout = Mock.Of<ILayout>();
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId("6357DD98-7859-5574-B846-78F5B1E57B2F"));
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        true
                    )
                );

            _layoutLibrary.Layouts.Add(forbiddenLayout);

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-78F5B1E57B2F/commit";
            var response = await _client.PostAsync(url, null);

            await CheckRequestIsError<BaseOutDto>(response, ErrorCode.ApiCommitUiHidLayoutForbidden);
        }

        [Test]
        public async Task LayoutController_CommitUIChanges_WorksOk()
        {
            base.Setup();

            var forbiddenLayout = Mock.Of<ILayout>();
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(FakeLayoutDbRepository.FakeLayoutId1));
            Mock.Get(forbiddenLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        false
                    )
                );

            _layoutLibrary.Layouts.Add(forbiddenLayout);

            var url = $"{GetServerUrl()}/api/layouts/{FakeLayoutDbRepository.FakeLayoutId1}/commit";
            var resp = await _client.PostAsync(url, null);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LayoutController_DuplicateLayout_01_01_WithUnknowLayoutId_ReturnApiDuplicateLayoutIsNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/duplicate";
            var inDto = new DuplicateLayoutInDto()
            {
                LayoutId = Guid.NewGuid().ToString(),
                Title = null
            };
            var json = JsonConvert.SerializeObject(inDto);
            var jsonContent = CreateJsonContent(json);
            var resp = await _client.PostAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(resp, ErrorCode.ApiDuplicateLayoutIsNotFound);
        }

        [Test]
        public async Task LayoutController_DuplicateLayout_01_02_WithHidLayout_ReturnApiDuplicateLayoutHidForbidden()
        {
            base.Setup();

            const string layoutId = "7DCB0207-6563-4AD3-91B3-55A02F23306A";

            var newLayout = Mock.Of<ILayout>();
            Mock.Get(newLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(layoutId));
            Mock.Get(newLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        true
                    )
                );

            _layoutLibrary.Layouts.Add(newLayout);

            Mock.Get(_layoutFacade)
                .Setup(x => x.DuplicateLayoutAsync(It.IsAny<ILayout>(), It.IsAny<string>()))
                .Throws(new ForbiddenActionException());

            var url = $"{GetServerUrl()}/api/layouts/duplicate";
            var inDto = new DuplicateLayoutInDto()
            {
                LayoutId = layoutId,
                Title = null
            };
            var json = JsonConvert.SerializeObject(inDto);
            var jsonContent = CreateJsonContent(json);
            var resp = await _client.PostAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(resp, ErrorCode.ApiDuplicateLayoutHidForbidden);
        }

        [Test]
        public async Task LayoutController_DuplicateLayout_01_03_WithAlreadyUsedName_ReturnApiDuplicateLayoutInvalidName()
        {
            base.Setup();

            const string layoutId = "7DCB0207-6563-4AD3-91B3-55A02F23306A";

            var newLayout = Mock.Of<ILayout>();
            Mock.Get(newLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(layoutId));
            Mock.Get(newLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(
                    new LayoutInfo(
                        new OsLayoutId("fr"),
                        false,
                        true
                    )
                );

            _layoutLibrary.Layouts.Add(newLayout);

            Mock.Get(_layoutFacade)
                .Setup(x => x.DuplicateLayoutAsync(It.IsAny<ILayout>(), It.IsAny<string>()))
                .Throws(new InvalidDataException());

            var url = $"{GetServerUrl()}/api/layouts/duplicate";
            var inDto = new DuplicateLayoutInDto()
            {
                LayoutId = layoutId,
                Title = FakeLayoutDbRepository.FakeLayoutTitle3
            };
            var json = JsonConvert.SerializeObject(inDto);
            var jsonContent = CreateJsonContent(json);
            var resp = await _client.PostAsync(url, jsonContent);

            await CheckRequestIsError<BaseOutDto>(resp, ErrorCode.ApiDuplicationLayoutNameAlreadyUsed);
        }

        [Test]
        public async Task LayoutController_ExportLayout_Success()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/8667DF98-1529-4A76-B846-78F5BFE57B2F/export";

            var httpResponse = await _client.GetAsync(url);

            httpResponse.Should().NotBeNull();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponse.Content.Should().NotBeNull();
        }

        [Test]
        public async Task LayoutController_ExportLayout_ErrorHidForbidden()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/6357DD98-7859-5574-B846-78F5B1E57B2F/export";

            var httpResponse = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiExportLayoutHidForbidden);
        }

        [Test]
        public async Task LayoutController_ExportLayout_ErrorIdInvalid()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/1/export";

            var httpResponse = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiExportLayoutInvalidId);
        }

        [Test]
        public async Task LayoutController_ExportLayout_ErrorIsNotFound()
        {
            base.Setup();

            var url = $"{GetServerUrl()}/api/layouts/00000000-0000-0000-0000-000000000000/export";

            var httpResponse = await _client.GetAsync(url);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiExportLayoutIsNotFound);
        }

        [Test]
        public async Task LayoutController_OpenExportedLayout_Success()
        {
            base.Setup();

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);

            var httpResponse = await PostFileAsync("api/layouts/export/open", layoutExportApiOutDto, "test.nemeio");

            httpResponse.Should().NotBeNull();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponse.Content.Should().NotBeNull();

            var response = await CheckRequestIsSuccess<LayoutExportApiOutDto>(httpResponse);

            response?.Result.Should().NotBeNull();

            response?.Result.Title.Should().Be("Layout1");
        }

        [Test]
        public async Task LayoutController_OpenExportedLayout_InvalidVersion()
        {
            base.Setup();

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.MajorVersion = 0;
            layoutExportApiOutDto.Version = "0.1";

            var httpResponse = await PostFileAsync("api/layouts/export/open", layoutExportApiOutDto, "test.nemeio");

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutInvalidExportVersion);
        }

        [Test]
        public async Task LayoutController_ImportLayout_Success()
        {
            base.Setup();

            var importCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.ImportLayoutAsync(It.IsAny<LayoutExport>()))
                .Callback(() => importCalled = true)
                .ReturnsAsync(fakeLayout1);

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.Title = layoutExportApiOutDto + " (copy)";
            layoutExportApiOutDto.AssociatedLayoutId = fakeLayout1.LayoutId.ToString();

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            var response = await CheckRequestIsSuccess<LayoutApiOutDto>(httpResponse);

            response.Should().NotBeNull();
            importCalled.Should().BeTrue();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("1")]
        public async Task LayoutController_ImportLayout_InvalidAssociatedLayoutId(string associatedLayoutId)
        {
            base.Setup();

            Mock.Get(_layoutFacade)
                .Setup(x => x.ImportLayoutAsync(It.IsAny<LayoutExport>()))
                .Throws(new CoreException(ErrorCode.CoreImportLayoutInvalidAssociatedLayoutId));

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.Title = layoutExportApiOutDto + " (copy)";
            layoutExportApiOutDto.AssociatedLayoutId = associatedLayoutId;

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutInvalidAssociatedLayoutId);
        }

        [Test]
        public async Task LayoutController_ImportLayout_MissingAssociatedLayout()
        {
            base.Setup();

            Mock.Get(_layoutFacade)
                .Setup(x => x.ImportLayoutAsync(It.IsAny<LayoutExport>()))
                .Throws(new CoreException(ErrorCode.CoreImportLayoutMissingAssociatedLayout));

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.Title = layoutExportApiOutDto + " (copy)";
            layoutExportApiOutDto.AssociatedLayoutId = "A667DF98-1529-4A76-B846-78F5BFE57B2F";

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutMissingAssociatedLayout);
        }

        [Test]
        public async Task LayoutController_ImportLayout_InvalidExportVersion()
        {
            base.Setup();

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.MajorVersion = 0;
            layoutExportApiOutDto.Version = "0.1";

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutInvalidExportVersion);
        }

        [Test]
        public async Task LayoutController_ImportLayout_TitleAlreadyUsed()
        {
            base.Setup();

            Mock.Get(_layoutFacade)
                .Setup(x => x.ImportLayoutAsync(It.IsAny<LayoutExport>()))
                .Throws(new CoreException(ErrorCode.CoreImportLayoutTitleAlreadyUsed));

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutTitleAlreadyUsed);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task LayoutController_ImportLayout_TitleEmpty(string title)
        {
            base.Setup();

            Mock.Get(_layoutFacade)
                .Setup(x => x.ImportLayoutAsync(It.IsAny<LayoutExport>()))
                .Throws(new CoreException(ErrorCode.CoreImportLayoutTitleEmpty));

            var layoutExportApiOutDto = await GenerateExportAsync(fakeLayout1.LayoutId);
            layoutExportApiOutDto.Title = title;

            var httpResponse = await PostRequestAsync("api/layouts/import", layoutExportApiOutDto);

            await CheckRequestIsError<BaseOutDto>(httpResponse, ErrorCode.ApiImportLayoutTitleEmpty);
        }

        #region Update Modifier Keys

        [Test]
        public async Task LayoutController_UpdateLayoutKey_OnModifierKey_ChangeDisplay_WorksOk()
        {
            base.Setup();

            const string layoutId = "8667DF98-1529-4A76-B846-78F5BFE57B2F";
            const string newKeyDisplay = "MyKey";
            const int keyIndex = 52;

            _fakeLayoutGenService.ModifierKeys.Add(keyIndex);

            var url = $"{GetServerUrl()}/api/layouts/{layoutId}/keys/{keyIndex}";
            var fakeJson = "{ 'actions': [{ 'display': '" + newKeyDisplay + "', 'modifier': 0, 'subactions': [{ 'data': 'Fake', 'type': 1 }] }], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var resp = await _client.PutAsync(url, jsonContent);

            updateLayoutCalled.Should().BeFalse();

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LayoutController_UpdateLayoutKey_OnModifierKey_ChangeSubactions_WorksOk()
        {
            base.Setup();

            const int keyIndex = 52;
            const string subActionData = "Fake";

            _fakeLayoutGenService.ModifierKeys.Add(keyIndex);

            var url = $"{GetServerUrl()}/api/layouts/{fakeLayout1.LayoutId}/keys/{keyIndex}";
            var fakeJson = "{ 'actions': [{ 'display': 'MyKey', 'modifier': 0, 'subactions': [{ 'data': '" + subActionData + "', 'type': 1 }] }], 'disposition': 0 }";
            var jsonContent = CreateJsonContent(fakeJson);

            var updateLayoutCalled = false;
            Mock.Get(_layoutFacade)
                .Setup(x => x.UpdateLayoutAsync(It.IsAny<ILayout>()))
                .Callback(() => updateLayoutCalled = true)
                .Returns(Task.Delay(0));

            var resp = await _client.PutAsync(url, jsonContent);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            updateLayoutCalled.Should().BeFalse();
        }

        #endregion

        private async Task<LayoutExportApiOutDto> GenerateExportAsync(string forId)
        {
            var httpResponse = await GetRequestAsync($"api/layouts/{forId}/export");
            var content = await httpResponse.Content.ReadAsStringAsync();
            var layoutExportApiOutDto = JsonConvert.DeserializeObject<LayoutExportApiOutDto>(content);
            return layoutExportApiOutDto;
        }
    }
}
