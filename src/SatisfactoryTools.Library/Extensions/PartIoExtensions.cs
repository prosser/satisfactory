namespace SatisfactoryTools.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public static class PartIoExtensions
    {
        public static string Description(this IEnumerable<PartIo> parts)
        {
            return string.Join(" + ", parts.Select(x => x.Description));
        }
    }
}