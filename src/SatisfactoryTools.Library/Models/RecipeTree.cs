namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Recipe.Name} -> {string.Join(\", \", ChildRecipeNames)}")]
    public class RecipeNode
    {
        public string[] ChildRecipeNames => this.Children.Select(x => x.Recipe.Name).OrderBy(x => x).ToArray();

        public HashSet<RecipeNode> Children { get; } = new HashSet<RecipeNode>();

        public string[] ParentRecipeNames => this.Parents.Select(x => x.Recipe.Name).OrderBy(x => x).ToArray();

        public HashSet<RecipeNode> Parents { get; } = new HashSet<RecipeNode>();

        public Recipe Recipe { get; set; }

        public bool TryConnect(RecipeNode node)
        {
            if (node.Recipe.Outputs.Any(x => this.Recipe.Inputs.Any(a => a.Part == x.Part)))
            {
                this.Parents.Add(node);
                node.Children.Add(this);
                return true;
            }

            if (node.Recipe.Inputs.Any(x => this.Recipe.Outputs.Any(a => a.Part == x.Part)))
            {
                this.Children.Add(node);
                node.Parents.Add(this);
                return true;
            }

            return false;
        }
    }

    public class RecipeTree
    {
        public RecipeNode Root { get; } = new RecipeNode {Recipe = Recipe.None};

        public static RecipeTree Build(IEnumerable<Recipe> recipes)
        {
            var allNodes = new HashSet<RecipeNode>(recipes.Select(x => new RecipeNode { Recipe = x }));

            var recipeQueue = new Queue<RecipeNode>(allNodes);
            var skipped = new List<RecipeNode>();
            var tree = new RecipeTree();

            while (recipeQueue.Count > 0 && skipped.Count < recipeQueue.Count)
            {
                var node = recipeQueue.Dequeue();

                if (tree.TryAdd(node, allNodes))
                {
                    skipped.Clear();
                }
                else
                {
                    skipped.Add(node);
                    recipeQueue.Enqueue(node);
                }
            }

            if (skipped.Count > 0)
            {
                throw new InvalidOperationException(
                    $"There were {skipped.Count} recipes that could not be linked to the tree: {string.Join(", ", skipped.Select(x=> x.Recipe.Name))}");
            }
            return tree;
        }

        public IEnumerable<RecipeNode> FindRecipe(Recipe recipe)
        {
            var queue = new Queue<RecipeNode>(new[] {this.Root});
            var seen = new HashSet<RecipeNode>();

            while (queue.Count > 0)
            {
                RecipeNode node = queue.Dequeue();

                if (node.Recipe == recipe)
                {
                    yield return node;
                }
                else
                {
                    foreach (RecipeNode child in node.Children.Where(x => seen.Add(x)))
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        public IEnumerable<RecipeNode> FindRecipesThatConsume(Part part)
        {
            var queue = new Queue<RecipeNode>(new[] {this.Root});
            var seen = new HashSet<RecipeNode>();

            while (queue.Count > 0)
            {
                RecipeNode node = queue.Dequeue();

                if (node.Recipe.Inputs.Any(x => x.Part == part))
                {
                    yield return node;
                }
                else
                {
                    foreach (RecipeNode child in node.Children.Where(x => seen.Add(x)))
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        public IEnumerable<RecipeNode> FindRecipesThatProduce(Part part)
        {
            var queue = new Queue<RecipeNode>(new[] {this.Root});
            var seen = new HashSet<RecipeNode>();

            while (queue.Count > 0)
            {
                RecipeNode node = queue.Dequeue();

                if (node.Recipe.Outputs.Any(x => x.Part == part))
                {
                    yield return node;
                }
                else
                {
                    foreach (RecipeNode child in node.Children.Where(x => seen.Add(x)))
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        public bool TryAdd(RecipeNode node, HashSet<RecipeNode> allNodes)
        {
            if (node.Recipe.Inputs.Count == 0)
            {
                this.Root.Children.Add(node);
                return true;
            }

            RecipeNode[] except = new[] {node};
            bool connected = false;
            foreach (var other in allNodes.Except(except))
            {
                connected |= other.TryConnect(node);
            }

            return connected;
        }
    }
}