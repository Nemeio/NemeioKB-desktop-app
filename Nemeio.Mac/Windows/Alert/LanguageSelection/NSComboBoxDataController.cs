using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows.Alert.LanguageSelection
{
    public class NSComboBoxDataController : NSObject, INSComboBoxDelegate, INSComboBoxDataSource
    {
        private IEnumerable<string> _items;
        private Action<int> _selectionChanged;

        public NSComboBoxDataController(IEnumerable<string> items, Action<int> selectionChanged)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = items;
            _selectionChanged = selectionChanged;
        }

        #region Delegate

        [Export("comboBox:objectValueForItemAtIndex:")]
        public NSObject ObjectValueForItem(NSComboBox comboBox, System.nint index)
        {
            var currentItem = _items.ElementAt((int)index);

            return new NSString(currentItem);
        }

        [Export("comboBoxSelectionDidChange:")]
        public void SelectionChanged(NSNotification notification)
        {
            var comboBox = notification.Object as NSComboBox;
            if (comboBox != null)
            {
                _selectionChanged?.Invoke((int)comboBox.SelectedIndex);
            }
        }

        #endregion

        #region DataSource

        [Export("numberOfItemsInComboBox:")]
        public System.nint ItemCount(NSComboBox comboBox)
        {
            return _items.Count();
        }

        #endregion
    }
}
