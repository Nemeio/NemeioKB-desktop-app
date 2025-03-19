using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;
using CategoryClass = Nemeio.Core.Services.Category.Category;

namespace Nemeio.Api.Dto.In.Category
{
    public class PutCategoryApiInDto
    {
        [JsonProperty(CategoryJsonKeys.Index, Required = Required.Always)]
        public int Index { get; set; }

        [JsonProperty(CategoryJsonKeys.Title, Required = Required.Always)]
        public string Title { get; set; }

        public CategoryClass ToDomainModel(int id) => new CategoryClass(id, Index, Title);
    }
}
