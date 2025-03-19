using System.Collections.Generic;

namespace Nemeio.Core.JsonModels
{
    public class CustomActionModel
    {
        public List<ActionModel> Actions { get; set; }

        public override string ToString()
        {
            string result = "";
            foreach (var act in Actions)
            {
                result += $" - {act.Data}";
            }
            return result;
        }
    }
}
