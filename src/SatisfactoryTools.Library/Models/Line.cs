﻿using System.Collections.Generic;

namespace SatisfactoryTools.Models
{
    public class Line
    {
        public Node Head { get; set; }

        public ISet<Node> Nodes { get; } = new HashSet<Node>();

        public Line Clone()
        {
            var clone = new Line
            {
                Head = this.Head.Clone(CloneFilters.Backward)
            };

            clone.Nodes.Add(clone.Head);
            foreach (Node node in clone.Head) clone.Nodes.Add(node);

            return clone;
        }
    }
}