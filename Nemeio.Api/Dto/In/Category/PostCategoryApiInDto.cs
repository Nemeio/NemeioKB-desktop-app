using Nemeio.Api.Dto.JsonKeys;
using Newtonsoft.Json;
using CategoryClass = Nemeio.Core.Services.Category.Category;

namespace Nemeio.Api.Dto.In.Category
{
    /// <summary>
    /// Create new category information
    /// </summary>
    public class PostCategoryApiInDto
    {
        /// <summary>
        /// Category's index
        /// </summary>
        [JsonProperty(CategoryJsonKeys.Index, Required = Required.Always)]
        public int Index { get; set; }

        /// <summary>
        /// Category's name. Must be unique.
        /// </summary>
        [JsonProperty(CategoryJsonKeys.Title, Required = Required.Always)]
        public string Title { get; set; }

        public CategoryClass ToDomainModel() => new CategoryClass(Index, Title);
    }
}
