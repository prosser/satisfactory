namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Flags]
    public enum CloneFilters
    {
        Backward = 0x1,

        Forward = 0x2
    }
}