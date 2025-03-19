using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;

namespace Nemeio.Core.Layouts.LinkedApplications
{
    public interface IApplicationLayoutManager
    {
        /// <summary>
        /// List if currently ignored applications (can be completed through configurator)
        /// </summary>
        IList<string> ExceptionNameList { get; }

        /// <summary>
        /// Method to get the latest layout possibly associated to current appplication
        /// </summary>
        /// <param name="information">Information object about inquired application</param>
        /// <returns>Return sthe found associated layout id or null if none found</returns>
        LayoutId FindLatestAssociatedLayoutId(Application information);

        /// <summary>
        /// Method to check whether an application is amongst the blacklist
        /// </summary>
        /// <param name="information">Information object about inquired application</param>
        /// <returns>Return sthe first foudn exception string or null if none found</returns>
        string FindException(Application information);

        /// <summary>
        /// Method to get layout associated with an uri
        /// </summary>
        /// <param name="applicationPathOrName">Application path or name</param>
        /// <returns>Layout or null value</returns>
        ILayout GetLayoutByLinkedApplicationPath(string applicationPathOrName);
    }
}
