namespace Nemeio.Mac.Models
{
    public class TableViewRow
    {
        public object AssociatedObject { get; set; }

        public MenuTableViewCellIdentifier CellIdentifier { get; set; }

        public bool Selected { get; set; } = false;
    }
}
