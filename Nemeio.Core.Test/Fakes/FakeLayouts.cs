using System;
using System.Collections.Generic;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Test.Fakes
{
    public static class FakeLayouts
    {
        public static byte[] imgBytes = new byte[0];

        public static readonly LayoutImageInfo DefaultImageInfo = new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), Mock.Of<IScreen>());

        public static readonly ILayout FakeFR = new Layout(new LayoutInfo(new FakeOsLayoutId("fr-FR"), false, false), DefaultImageInfo, imgBytes, 0, 0, "fakeFR", "fakeSubtitleFR", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly ILayout FakeHidFR = new Layout(new LayoutInfo(new FakeOsLayoutId("fr-FR"), false, true), DefaultImageInfo, imgBytes, 0, 0, "fakeFRHid", "fakeSubtitleFRHid", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly ILayout FakeCA = new Layout(new LayoutInfo(new FakeOsLayoutId("fr-CA"), false, false), DefaultImageInfo, imgBytes, 0, 0, "fakeCA", "fakeSubtitleCA", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly ILayout FakeUS = new Layout(new LayoutInfo(new FakeOsLayoutId("en-US"), false, false), DefaultImageInfo, imgBytes, 0, 0, "fakeUS", "fakeSubtitleUS", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly List<ILayout> FakeList = new List<ILayout> { FakeFR, FakeCA, FakeUS, FakeHidFR };

        public static readonly ILayout UnHandledLayout1FromDb = new Layout(new LayoutInfo(new FakeOsLayoutId("ac-DC"), false, false), DefaultImageInfo, imgBytes, 0, 0, "Thunderstruck", "Subtitle Thunderstruck", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly ILayout UnHandledLayout2FromDb = new Layout(new LayoutInfo(new FakeOsLayoutId("AC-dc"), false, false), DefaultImageInfo, imgBytes, 0, 0, "Highway To Hell", "Subtitle Highway To Hell", DateTime.Now, DateTime.Now, new List<Key>());

        public static readonly ILayout CoreLayout1 = new Layout(new LayoutInfo(new FakeOsLayoutId("fr-FR"), false, false), DefaultImageInfo, imgBytes, 0, 0, "fakeFR", "fakeSubtitleFR", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly ILayout CoreLayout2 = new Layout(new LayoutInfo(new FakeOsLayoutId("ar-TN"), false, false), DefaultImageInfo, imgBytes, 0, 0, "fakeAR", "fakeSubtitleAR", DateTime.Now, DateTime.Now, new List<Key>());
        public static readonly List<ILayout> CoreSeedList = new List<ILayout> { CoreLayout1, CoreLayout2 };

}
}
