using AppKit;

namespace Nemeio.Mac.Extensions
{
    public static class NSViewExtensions
    {
        public static NSLayoutConstraint GetHeightConstraint(this NSView view)
        {
            if (view == null)
            {
                return null;
            }

            NSLayoutConstraint heightConstraint = null;

            foreach (var constraint in view.Constraints)
            {
                if (constraint.FirstAttribute == NSLayoutAttribute.Height)
                {
                    heightConstraint = constraint;
                    break;
                }
            }

            return heightConstraint;
        }

        public static void Minimize(this NSView view)
        {
            var heightConstraint = GetHeightConstraint(view);
            if (heightConstraint != null)
            {
                heightConstraint.Constant = 0;

                view.NeedsUpdateConstraints = true;
                view.UpdateConstraints();
            }
        }

        public static void Maximize(this NSView view, float value)
        {
            var heightConstraint = GetHeightConstraint(view);
            if (heightConstraint != null)
            {
                heightConstraint.Constant = value;

                view.NeedsUpdateConstraints = true;
                view.UpdateConstraints();
            }
        }
    }
}
