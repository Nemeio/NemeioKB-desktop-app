using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active.Historic
{
    public class ActiveLayoutHistoric : IActiveLayoutHistoric
    {
        private const int MaximumHistoricCount = 5;

        private readonly object _lockList = new object();

        private int _currentIndex = 0;
        private IList<IHistoricLog> _layouts;

        public int Index => _currentIndex;
        public uint MaxItemCount { get; set; }
        public IList<IHistoricLog> Historic => _layouts;

        public ActiveLayoutHistoric(uint maxItemCount = MaximumHistoricCount)
        {
            if (maxItemCount == 0)
            {
                throw new InvalidOperationException("Impossible to create historic with max count equal to zero.");
            }

            _layouts = new List<IHistoricLog>();

            MaxItemCount = maxItemCount;
        }

        public IHistoricLog Back()
        {
            lock (_lockList)
            {
                if (IsFirstIndex())
                {
                    //  Can't go back
                    //  Nothing to do

                    return null;
                }
                else
                {
                    _currentIndex -= 1;

                    return _layouts[_currentIndex];
                }
            }
        }

        public IHistoricLog Forward()
        {
            lock (_lockList)
            {
                if (IsMaximumIndex() || _currentIndex == _layouts.Count - 1)
                {
                    //  Can't go forward
                    //  Nothing to do

                    return null;
                }
                else
                {
                    _currentIndex += 1;

                    return _layouts[_currentIndex];
                }
            }
        }

        public void Insert(IHistoricLog historic)
        {
            lock (_lockList)
            {
                if (historic == null)
                {
                    throw new ArgumentNullException(nameof(historic));
                }

                if (_layouts.Count > 0 && historic.Layout.LayoutId == _layouts.Last().Layout.LayoutId)
                {
                    //  We don't stack same layout 2 times

                    return;
                }
                var lastRef = _layouts.FirstOrDefault(x => x.Layout.LayoutId == historic.Layout.LayoutId);
                if (lastRef == null)
                {
                    while(_layouts.Count>0 && _currentIndex<_layouts.Count-1)
                    {
                        _layouts.RemoveAt(_layouts.Count - 1);
                    }
                    _layouts.Add(historic);
                    if (_layouts.Count >= 1)
                        _currentIndex = _layouts.IndexOf(historic);
                }
                else
                {
                    _layouts.Remove(lastRef);
                    _layouts.Add(lastRef);
                }
                if (IsMaximumIndex())
                    _layouts.RemoveAt(0);
            }
        }

        public void Remove(ILayout layout)
        {
            Reset();
        }

        public void Reset()
        {
            lock (_lockList)
            {
                _layouts.Clear();
                _currentIndex = 0;
            }
        }

        public override string ToString()
        {
            var result = $"Current index : <{_currentIndex}>\n";

            foreach (var layout in _layouts)
            {
                result += $"- {layout.Layout.Title}\n";
            }

            return result;
        }

        private bool IsMaximumIndex() => _layouts.Count > MaximumHistoricCount;
        private bool IsFirstIndex() => _currentIndex == 0;
    }
}
