namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Node : IEnumerable<Node>
    {
        public IList<Edge> Inputs { get; } = new List<Edge>();

        public IList<Edge> Outputs { get; } = new List<Edge>();

        public NodeTransformer Transformer { get; set; }

        public Node Clone(CloneFilters filters)
        {
            Node clone = new Node
            {
                Transformer = this.Transformer.Clone()
            };

            if (filters.HasFlag(CloneFilters.Backward))
            {
                foreach (Edge edge in this.Inputs)
                {
                    clone.Inputs.Add(edge.Clone(filters));
                }
            }

            if (filters.HasFlag(CloneFilters.Forward))
            {
                foreach (Edge edge in this.Outputs)
                {
                    clone.Outputs.Add(edge.Clone(filters));
                }
            }

            return clone;
        }

        public double GetCombinedLeafRate()
        {
            return this.Inputs.Count == 0
                ? this.Outputs.Sum(x => x.Rate)
                : this.Inputs.Sum(x => x.Consumer.GetCombinedLeafRate());
        }

        public IEnumerator<Node> GetEnumerator()
        {
            foreach (Edge edge in this.Inputs)
            {
                yield return edge.Producer;
                foreach (Node node in edge.Producer)
                {
                    yield return node;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}