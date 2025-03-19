using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Nemeio.Core.Services.AppSettings;

namespace Nemeio.Infrastructure.DbModels
{
    [Table("ApplicationSettings")]
    public class ApplicationSettingsDbModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Stores the code (e.g. en-EN, en-US, ...) of the language used for the graphical interface of the application (menu, configurator, ...).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// HID+ status for current user
        /// </summary>
        [Required]
        public bool AugmentedImageEnable { get; set; }

        /// <summary>
        /// Indicates if the grant privilege window should be shown every times.
        /// </summary>
        [Required]
        public bool ShowGrantPrivilegeWindow { get; set; }

        public string UpdateTo { get; set; }

        public string LastRollbackManifestString { get; set; }

        public ApplicationSettings ToDomainModel()
        {
            var settings = new ApplicationSettings(
                Language == null ? null : new CultureInfo(Language),
                AugmentedImageEnable,
                ShowGrantPrivilegeWindow,
                UpdateTo == null ? null : new Version(UpdateTo),
                LastRollbackManifestString
            );

            return settings;
        }
    }
}
