namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Building
    {
        public Builder BuildingType { get; set; }

        public IEnumerable<Part> InputParts => this.Recipe.Inputs.Select(x => x.Part);

        public IEnumerable<Part> OutputParts => this.Recipe.Outputs.Select(x => x.Part);

        public Recipe Recipe { get; set; } = Recipe.None;
    }
}