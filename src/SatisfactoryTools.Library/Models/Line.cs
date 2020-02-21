namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Line
    {
        public Node Head { get; set; }

        public ISet<Node> Nodes { get; } = new HashSet<Node>();

        public static Line Clone(Line line)
        {
            Line clone = new Line
            {
                Head = Node.Clone(line.Head, CloneFilters.Backward)
            };

            clone.Nodes.Add(clone.Head);
            foreach (Node node in clone.Head)
            {
                clone.Nodes.Add(node);
            }

            return clone;
        }
    }
}