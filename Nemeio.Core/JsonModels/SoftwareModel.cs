using System.Collections.Generic;

namespace Nemeio.Core.JsonModels
{
    public class SoftwareModel
    {
        public IEnumerable<SoftwareVersionModel> Softwares { get; set; }
    }
}
