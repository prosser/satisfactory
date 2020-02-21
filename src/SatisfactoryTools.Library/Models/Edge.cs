namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Edge
    {
        public double Capacity { get; set; }

        public Node Consumer { get; set; }

        public double Length { get; set; }

        public Part Part { get; set; }

        public Node Producer { get; set; }

        public double Rate { get; set; }

        public Edge Clone(CloneFilters filters)
        {
            Edge clone = new Edge
            {
                Capacity = this.Capacity,
                Rate = this.Rate,
                Part = this.Part,
                Consumer = filters.HasFlag(CloneFilters.Backward) ? this.Consumer.Clone(filters) : this.Consumer,
                Producer = filters.HasFlag(CloneFilters.Forward) ? this.Producer.Clone(filters) : this.Producer
            };
            return clone;
        }
    }
}