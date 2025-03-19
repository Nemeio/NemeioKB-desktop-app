using System.Collections.Generic;
using System.Linq;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Api.Dto.Out.Layout;
using Newtonsoft.Json;
using CategoryClass = Nemeio.Core.Services.Category.Category;

namespace Nemeio.Api.Dto.Out.Category
{
    public class CategoryApiOutDto : BaseOutDto
    {
        [JsonProperty(CategoryJsonKeys.Id)]
        public long Id { get; set; }

        [JsonProperty(CategoryJsonKeys.Index)]
        public int Index { get; set; }

        [JsonProperty(CategoryJsonKeys.Title)]
        public string Title { get; set; }

        [JsonProperty(CategoryJsonKeys.Layouts)]
        public IEnumerable<BaseOutDto> Layouts { get; set; }

        public static CategoryApiOutDto FromModel(CategoryClass cat) => new CategoryApiOutDto()
        {
            Id = cat.Id,
            Index = cat.Index,
            Title = cat.Title,
            Layouts = cat.Layouts != null ? cat.Layouts.Select((x) => LayoutApiOutDto.FromModel(x)) : null
        };

        public static CategoryApiOutDto FromModelWithoutKeys(CategoryClass cat) => new CategoryApiOutDto()
        {
            Id = cat.Id,
            Index = cat.Index,
            Title = cat.Title,
            Layouts = cat.Layouts != null ? cat.Layouts.Select((x) => LightLayoutApiOutDto.FromModel(x)) : null
        };

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
