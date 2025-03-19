using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;
using Nemeio.Models.Fonts;
using Newtonsoft.Json;

namespace Nemeio.Infrastructure.DbModels
{
    [Table("Layouts")]
    public class LayoutDbModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public ScreenType Screen { get; set; }

        /// <summary>
        /// HID layouts are linked to a system layout. This field contains the id of this system layout. In particular, it allows to set the right system layout when the user selects it. Is only useful for HID layouts.
        /// </summary>
        [Required]
        public string OsId { get; set; }

        /// <summary>
        /// This id is only used for custom layouts. Each custom layout is linked to a HID layout for the management of dead keys and the system layout to be set up.
        /// </summary>
        public string AssociatedId { get; set; }

        /// <summary>
        /// Image sent to the keyboard representing the current layout. Stored to avoid image calculation at each startup.
        /// </summary>
        [Required]
        public byte[] Image { get; set; }

        public CategoryDbModel Category { get; set; }

        /// <summary>
        /// Each layout is part of a category to classify it.
        /// </summary>
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Display index in the configurator layout list.
        /// </summary>
        public int ConfiguratorIndex { get; set; }

        /// <summary>
        /// Layout title. Is used in particular in the menu and the configurator.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Layout subtitle. Is used to identify the Associated Layout language.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Date of creation of the layout. Defined only at the time of its creation.
        /// </summary>
        public DateTime DateCreation { get; set; }

        /// <summary>
        /// Layout update date. Defined during creation and each time it is modified.
        /// </summary>
        public DateTime DateUpdate { get; set; }

        /// <summary>
        /// A layout can have an active / inactive state. This state allows to have or not to have access to the layout in the menu and to obtain its synchronization with the keyboard.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Some layouts are implemented from the factory and present only on the keyboard. Not in use at the moment.
        /// </summary>
        public bool IsFactory { get; set; }

        /// <summary>
        /// Two types of layouts exist: hid (created from the operating system) or custom (created by the user from a hid layout)
        /// </summary>
        public bool IsHid { get; set; }

        /// <summary>
        /// There is a default layout that is set when the landing is shut down or in special cases. There can only be one default layout.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// A set of keys on a keyboard. Here we store directly the JSON representing these keys. In particular, they are used to recreate the image of a layout.
        /// </summary>
        public string Keys { get; set; }

        /// <summary>
        /// Set of application links automatically switching layout. In this case, it may be a path or simply the name of a program.
        /// </summary>
        public string LinkAppPath { get; set; }

        /// <summary>
        /// Switch status. If this one is a false, the automatic switch is disabled for this layout.
        /// </summary>
        public bool LinkAppEnable { get; set; }

        /// <summary>
        /// Layout display status. Black background with white writing or vice versa.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Some layouts are annotated as templates (here they are for the moment HIDs only). They are the ones that allow the creation of a custom layout.
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// Name of the font used globally for the current layout. Necessary to draw the image of the layout.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Size of the font used for the layout.
        /// </summary>
        public FontSize FontSize { get; set; }

        /// <summary>
        /// Default bold status for the layout.
        /// </summary>
        public bool FontIsBold { get; set; }

        /// <summary>
        /// Default italic status for the layout.
        /// </summary>
        public bool FontIsItalic { get; set; }

        /// <summary>
        /// Layout image can have different look and feel (classic, hide, gray, ...)
        /// </summary>
        public LayoutImageType ImageType { get; set; }

        /// <summary>
        /// Status of HID+ image
        /// </summary>
        public bool AugmentedImageEnabled { get; set; }

        /// <summary>
        /// Value to adapt layout X position on image
        /// </summary>
        public float XPositionAdjustment { get; set; }

        /// <summary>
        /// Value to adapt layout Y position on image
        /// </summary>
        public float YPositionAdjustment { get; set; }

        /// <summary>
        /// Value to define order of the Layouts in lists
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// This id is only used for custom layouts to allow the automatic restore of layouts on lnaguage re install.
        /// </summary>
        public string OriginalAssociatedId { get; set; }

        public ILayout ToDomainModel(IScreenFactory screenFactory)
        {
            var links = LinkAppPath.Split(LayoutDbRepository.LinkDelimiter)
                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                                    .Select(x => x.ToLower())
                                    .ToList();

            var layoutInfo = new LayoutInfo(
                new OsLayoutId(OsId),
                IsFactory,
                IsHid,
                links,
                LinkAppEnable,
                IsTemplate,
                AugmentedImageEnabled
            );

            var layoutImageInfo = new LayoutImageInfo(
                new HexColor(Color),
                new Font(FontName, FontSize, FontIsBold, FontIsItalic),
                screenFactory.CreateScreen(Screen),
                ImageType,
                XPositionAdjustment,
                YPositionAdjustment
            );

            return new Layout(
                layoutInfo,
                layoutImageInfo,
                Image,
                CategoryId,
                ConfiguratorIndex,
                Title,
                Subtitle,
                DateCreation,
                DateUpdate,
                Keys != null ? JsonConvert.DeserializeObject<List<Key>>(Keys) : new List<Key>(),
                new LayoutId(Id),
                string.IsNullOrEmpty(AssociatedId) ? null : new LayoutId(AssociatedId),
                IsDefault,
                Enable,
                Order,
                string.IsNullOrEmpty(OriginalAssociatedId) ? null : new LayoutId(OriginalAssociatedId)
            );
        }
    }
}
