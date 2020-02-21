namespace SatisfactoryTools.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StorageChangedEventArgs
    {
        public string Key { get; set; }

        public object NewValue { get; set; }

        public object OldValue { get; set; }
    }
}