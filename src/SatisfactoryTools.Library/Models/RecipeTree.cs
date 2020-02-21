namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PartNode
    {
        public HashSet<PartNode> Consumers { get; } = new HashSet<PartNode>();

        public Part Part { get; set; }

        public HashSet<PartNode> Producers { get; } = new HashSet<PartNode>();

        public HashSet<Recipe> Recipes { get; } = new HashSet<Recipe>();

        public void AddConsumer(PartNode node)
        {
            this.Consumers.Add(node);
            node.Producers.Add(this);
        }

        public PartNode Find(Part part)
        {
            if (part == this.Part)
            {
                return this;
            }

            return this.Consumers.Select(x => x.Find(part)).FirstOrDefault(x => x != null);
        }
    }

    public class PartTree
    {
        public PartNode Root { get; } = new PartNode { Part = Part.None };

        public bool TryAdd(Recipe recipe)
        {
            if (recipe.Inputs.Count == 0)
            {
                foreach (PartIo output in recipe.Outputs)
                {
                    var node = new PartNode { Part = output.Part };
                    node.Recipes.Add(recipe);
                    this.Root.AddConsumer(node);
                }

                return true;
            }

            foreach (PartIo input in recipe.Inputs)
            {
                Part part = input.Part;
                PartNode producer = this.Root.Find(part);
                if (producer == null)
                {
                    return false;
                }

                foreach (PartIo output in recipe.Outputs)
                {
                    var node = new PartNode { Part = output.Part };
                    node.Recipes.Add(recipe);
                    producer.AddConsumer(node);
                }

                return true;
            }
        }
    }
}