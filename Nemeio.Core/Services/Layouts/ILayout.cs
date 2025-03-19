using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.LayoutWarning;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.Services.Layouts
{
    public interface ILayout : IBackupable<ILayout>
    {
        LayoutId LayoutId { get; set; }
        LayoutHash Hash { get; set; }
        LayoutId AssociatedLayoutId { get; set; }
        LayoutInfo LayoutInfo { get; set; }
        LayoutImageInfo LayoutImageInfo { get; set; }
        int Index { get; set; }
        byte[] Image { get; set; }
        string Title { get; set; }
        string Subtitle { get; set; }
        int CategoryId { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        bool Enable { get; set; }
        SpecialSequences SpecialSequences { get; }
        bool IsDefault { get; set; }
        List<Key> Keys { get; set; }
        Font Font { get; set; }
        IEnumerable<LayoutWarning> Warnings { get; }
        int Order { get; set; }
        LayoutId OriginalAssociatedLayoutId { get; set; }

        void CalculateImageHash();
    }

    public class Layout : ILayout
    {
        public const int WinSASLength = 3;

        private List<Key> _keys;
        private byte[] _layoutImage;

        public LayoutId LayoutId { get; set; }
        public LayoutHash Hash { get; set; }
        public LayoutId AssociatedLayoutId { get; set; }
        public LayoutInfo LayoutInfo { get; set; }
        public LayoutImageInfo LayoutImageInfo { get; set; }
        public int Index { get; set; }
        public byte[] Image
        {
            get => _layoutImage;
            set
            {
                _layoutImage = value;
                CalculateImageHash();
            }
        }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Enable { get; set; }
        public SpecialSequences SpecialSequences { get; private set; }
        public bool IsDefault { get; set; }
        public List<Key> Keys
        {
            get => _keys;
            set
            {
                _keys = value;

                CalculateSecureAttentionSequence();
                CalculateWarnings();
            }
        }
        public Font Font { get; set; }

        public IEnumerable<LayoutWarning> Warnings { get; private set; }
        public int Order { get; set; }
        public LayoutId OriginalAssociatedLayoutId { get; set; }

        public Layout(LayoutInfo layoutInfo, LayoutImageInfo layoutImageInfo, byte[] image, int categoryId, int index, string title, string subtitle, DateTime creation, DateTime update,
            List<Key> keys, LayoutId layoutId = null, LayoutId associatedLayoutId = null, bool isDefault = false, bool enable = true, int order = 0, LayoutId originalLayoutId = null)
        {
            LayoutInfo = layoutInfo;
            LayoutId = layoutId ?? LayoutId.Compute(layoutInfo);
            AssociatedLayoutId = associatedLayoutId;
            CategoryId = categoryId;
            Index = index;
            Title = title;
            Subtitle = subtitle;
            DateCreated = creation;
            DateUpdated = update;
            Enable = enable;
            IsDefault = isDefault;
            Keys = keys;
            LayoutImageInfo = layoutImageInfo;
            Image = image;
            Order = order;
            OriginalAssociatedLayoutId = originalLayoutId;
            CalculateImageHash();
        }

        public ILayout CreateBackup()
        {
            var imageCopy = new byte[Image.Length];

            Array.Copy(Image, 0, imageCopy, 0, Image.Length);

            return new Layout(
                new LayoutInfo(
                    new OsLayoutId(LayoutInfo.OsLayoutId),
                    LayoutInfo.Factory,
                    LayoutInfo.Hid,
                    LayoutInfo.LinkApplicationPaths == null ? null : new List<string>(LayoutInfo.LinkApplicationPaths),
                    LayoutInfo.LinkApplicationEnable,
                    LayoutInfo.IsTemplate,
                    LayoutInfo.AugmentedHidEnable
                ),
                new LayoutImageInfo(
                    LayoutImageInfo.Color.CreateBackup(),
                    LayoutImageInfo.Font.CreateBackup(),
                    LayoutImageInfo.Screen,
                    LayoutImageInfo.ImageType,
                    LayoutImageInfo.XPositionAdjustment,
                    LayoutImageInfo.YPositionAdjustement
                ),
                imageCopy,
                CategoryId,
                Index,
                Title,
                Subtitle,
                new DateTime(DateCreated.Ticks),
                new DateTime(DateUpdated.Ticks),
                CreateKeysBackup(),
                new LayoutId(LayoutId),
                AssociatedLayoutId == null ? null : new LayoutId(AssociatedLayoutId),
                IsDefault,
                Enable,
                Order
            );
        }

        private void CalculateSecureAttentionSequence() => SpecialSequences = LayoutSecureAttentionSequenceBuilder.Build(this);

        private void CalculateWarnings() => Warnings = new LayoutAnalyser(this).Analyse();

        public void CalculateImageHash() => Hash = LayoutHash.Compute(LayoutId, Image);

        private List<Key> CreateKeysBackup()
        {
            var backupList = new List<Key>();

            foreach (var key in Keys)
            {
                var backupKey = key.CreateBackup();

                backupList.Add(backupKey);
            }

            return backupList;
        }
    }
}
