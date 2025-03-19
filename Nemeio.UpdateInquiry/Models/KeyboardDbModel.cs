using Microsoft.WindowsAzure.Storage.Table;

namespace Nemeio.UpdateInquiry.Models
{
    public class KeyboardDbModel : TableEntity
    {
        public string Partition { get; set; }

        public string Environment { get; set; }
    }
}
