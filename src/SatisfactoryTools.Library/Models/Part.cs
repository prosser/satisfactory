namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Id} {Name} {PartClass}{Units}")]
    public class Part : IIdentifiable, INamed
    {
        internal const string M3Units = " m\u00b3";

        public static Part None { get; } = new Part { Id = 0, Name = "None" };

        public PartClass PartClass { get; set; } = PartClass.Item;

        public string Units => this.PartClass switch
        {
            PartClass.Fluid => M3Units,
            var _ => ""
        };

        public int Id { get; set; }

        public string Name { get; set; }
    }
}