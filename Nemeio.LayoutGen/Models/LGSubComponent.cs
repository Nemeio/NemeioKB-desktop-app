using System;

namespace Nemeio.LayoutGen.Models
{
    internal abstract class LGSubComponent<T> : LGComponent where T : LGComponent
    {
        internal T Parent { get; private set; }

        internal LGSubComponent(T parent, LGPosition position, LGSize size) 
            : base(ComputePosition(parent, position), size) 
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        private static LGPosition ComputePosition(T parent, LGPosition position)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var x = parent.Position.X;
            var y = parent.Position.Y;

            if (position != null)
            {
                x += position.X;
                y += position.Y;
            }

            var calculatedPosition = new LGPosition(x, y);

            return calculatedPosition;
        }
    }
}
