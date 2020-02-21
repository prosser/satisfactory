namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Recipe.Name} -> {string.Join(\", \", ConsumerRecipeNames)}")]
    public class RecipeNode
    {
        public HashSet<RecipeNode> Consumers { get; } = new HashSet<RecipeNode>();

        public HashSet<RecipeNode> Producers { get; } = new HashSet<RecipeNode>();

        public Recipe Recipe { get; set; }

        public bool TryConnect(RecipeNode node)
        {
            if (node.Recipe.Outputs.Any(x => this.Recipe.Inputs.Any(a => a.Part == x.Part)))
            {
                this.Producers.Add(node);
                node.Consumers.Add(this);
                return true;
            }

            if (node.Recipe.Inputs.Any(x => this.Recipe.Outputs.Any(a => a.Part == x.Part)))
            {
                this.Consumers.Add(node);
                node.Producers.Add(this);
                return true;
            }

            return false;
        }

        public IEnumerable<RecipeNode> FindConsumer(Part part)
        {
            if (this.Recipe.Outputs.Any(x => x.Part == part))
            {
                yield return this;
            }

            foreach (IEnumerable<RecipeNode> nodes in this.Consumers.Select(x => x.FindConsumer(part)))
            {
                foreach (RecipeNode node in nodes)
                {
                    yield return node;
                }
            }
        }

        public string[] ConsumerRecipeNames => this.Consumers.Select(x => x.Recipe.Name).OrderBy(x => x).ToArray();
        public string[] ProducerRecipeNames => this.Producers.Select(x => x.Recipe.Name).OrderBy(x => x).ToArray();
    }

    public class RecipeTree
    {
        public RecipeNode Root { get; } = new RecipeNode {Recipe = Recipe.None};

        public static RecipeTree Build(IEnumerable<Recipe> recipes)
        {
            var allNodes = new HashSet<RecipeNode>();

            var recipeQueue = new Queue<Recipe>(recipes);
            var skipped = new List<Recipe>();
            var tree = new RecipeTree();

            while (recipeQueue.Count > 0 && skipped.Count < recipeQueue.Count)
            {
                Recipe recipe = recipeQueue.Dequeue();

                if (tree.TryAdd(recipe, allNodes))
                {
                    skipped.Clear();
                }
                else
                {
                    skipped.Add(recipe);
                    recipeQueue.Enqueue(recipe);
                }
            }

            foreach (var node in allNodes)
            {
                foreach (var other in allNodes.Except(new[] {node}))
                {
                    node.TryConnect(other);
                }
            }

            if (skipped.Count > 0)
            {
                throw new InvalidOperationException(
                    $"There were {skipped.Count} recipes that could not be linked to the tree");
            }

            return tree;
        }

        public bool TryAdd(Recipe recipe, HashSet<RecipeNode> allNodes)
        {
            var node = new RecipeNode {Recipe = recipe};
            allNodes.Add(node);

            if (recipe.Inputs.Count == 0)
            {
                this.Root.Consumers.Add(node);
                return true;
            }

            return allNodes.Select(node.TryConnect).Any();
            //foreach (PartIo input in recipe.Inputs)
            //{
            //    Part part = input.Part;


            //    foreach (var candidate in allNodes)
            //    {
            //        if (candidate.Recipe.Inputs.Any(x => recipe.Outputs.Contains(x.Part))
            //    }

            //    bool added = false;



            //    foreach (RecipeNode producer in this.Root.FindConsumer(part))
            //    {
            //        added = true;
            //        producer.ConnectConsumer(node);
            //    }

            //    if (!added)
            //    {
            //        return false;
            //    }
            //}

            //// lazy connect for already added consumers of this recipe's outputs
            //foreach (PartIo output in recipe.Outputs)
            //{
            //    foreach (RecipeNode consumer in this.Root.FindConsumer(output.Part))
            //    {
            //        consumer.Producers.Add(node);
            //    }
            //}


            //return true;
        }
    }
}