using System.Collections.Generic;

namespace Nemeio.Core.JsonModels
{
    public class DeadKeyModel
    {
        public string Key { get; set; }

        public List<AssociationModel> Associations { get; set; }
    }
}
