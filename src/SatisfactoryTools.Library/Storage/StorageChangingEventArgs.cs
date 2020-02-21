namespace SatisfactoryTools.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StorageChangingEventArgs : StorageChangedEventArgs
    {
        public bool Cancel { get; set; }
    }
}