using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;
using CategoryClass = Nemeio.Core.Services.Category.Category;

namespace Nemeio.Api.Dto.Out.Category
{
    public class SimpleCategoryApiOutDto : BaseOutDto
    {
        [JsonProperty(CategoryJsonKeys.Id)]
        public long Id { get; set; }

        [JsonProperty(CategoryJsonKeys.Index)]
        public int Index { get; set; }

        [JsonProperty(CategoryJsonKeys.Title)]
        public string Title { get; set; }

        public static SimpleCategoryApiOutDto FromModel(CategoryClass cat) => new SimpleCategoryApiOutDto()
        {
            Id = cat.Id,
            Index = cat.Index,
            Title = cat.Title
        };
    }
}
