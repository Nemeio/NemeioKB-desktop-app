using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Core.EventArguments
{
    public class RemovedByDeletionEventArgs : EventArgs
    {
        public string[] DeletedHidName { get; private set; }

        public string[] RemovedCustomNames { get; private set; }
        public RemovedByDeletionEventArgs(string[] deletedHidName, string[] removedCustomNames)
        {
            DeletedHidName = deletedHidName;
            RemovedCustomNames = removedCustomNames;
        }
    }
}
