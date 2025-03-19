using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nemeio.Core.Services.Blacklist;

namespace Nemeio.Infrastructure.DbModels
{
    [Table("Blacklists")]
    public class BlacklistDbModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Application name (with extension) blacklisted.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// IsSystem makes it possible in particular to know if this program is blacklisted by default. A blacklisted program and system cannot be destroyed.
        /// </summary>
        [DataType("BOOLEAN")]
        public bool IsSystem { get; set; }

        public Blacklist ToDomainModel() => new Blacklist(Id, Name, IsSystem);
    }
}
