using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nemeio.Core.Services.Category;

namespace Nemeio.Infrastructure.DbModels
{
    [Table("Categories")]
    public class CategoryDbModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The display index of the category within the configurator list.
        /// </summary>
        public int ConfiguratorIndex { get; set; }

        /// <summary>
        /// Each category has a title for its display.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Each time the database is created, a default category is created (id 1). There can only be one default category.
        /// </summary>
        public bool IsDefault { get; set; }

        public Category ToDomainModel() => new Category(Id, ConfiguratorIndex, Title, isDefault: IsDefault);
    }
}
