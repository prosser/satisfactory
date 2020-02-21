namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SatisfactoryTools.Models;

    public class ProductionLineFactory : IProductionLineFactory
    {
        private readonly IPartStore parts;

        private readonly IRecipeStore recipes;

        public ProductionLineFactory(IRecipeStore recipes, IPartStore parts)
        {
            this.recipes = recipes;
            this.parts = parts;
        }

        public Line Create(PartIo target, IEnumerable<Part> existingInputs, IEnumerable<Recipe> constraints)
        {
            Line line = new Line();

            IEnumerable<Node> permutations = this.CreateAll(target, existingInputs, constraints);
            Node best = OptimizeForLowestLeafRate(permutations);

            line.Head = best;

            return line;
        }

        public IEnumerable<Node> CreateAll(PartIo target, IEnumerable<Part> existingInputs, IEnumerable<Recipe> constraints)
        {
            return this.recipes
                .GetRecipesForOutput(target.Part)
                .Select(recipe => new Node { Transformer = recipe })
                .SelectMany(node => this.Build(node, existingInputs, constraints));
        }

        private static void Connect(Node consumer, Node producer, PartIo io)
        {
            ValidateNewEdge(consumer, consumer.Inputs, io.Part);
            ValidateNewEdge(producer, producer.Outputs, io.Part);

            Edge edge = new Edge
            {
                Rate = io.Rate,
                Consumer = consumer,
                Producer = producer
            };
            consumer.Inputs.Add(edge);
            producer.Outputs.Add(edge);
        }

        private static Node OptimizeForLowestLeafRate(IEnumerable<Node> permutations)
        {
            return permutations.OrderBy(x => x.GetCombinedLeafRate()).First();
        }

        private static void ValidateNewEdge(Node node, IEnumerable<Edge> existingEdges, Part part)
        {
            if (existingEdges.Any(x => x.Part == part))
            {
                throw new InvalidOperationException($"Node {node.Transformer.Name} already connected to part {part.Name}");
            }
        }

        private IEnumerable<Node> Build(Node node, IEnumerable<Part> existingInputs, IEnumerable<Recipe> constraints)
        {
            bool isRoot = true;
            foreach (PartIo input in node.Transformer.Inputs
                // exclude existing inputs so we don't add buildings for them
                .Where(input => !existingInputs.Contains(input.Part))
            )
            {
                Node[] producers = this.recipes.GetRecipesForOutput(input.Part)
                    .Select(recipe => new Node { Transformer = recipe })
                    .ToArray();

                //if (producers.Length == 1)
                //{
                //    // no new branch for this side, just add to the root
                //    Connect(root, producers[0], input);
                //    continue;
                //}

                // create a new branch for each recipe that can produce the part on this input
                foreach (Node producer in producers)
                {
                    Node newRoot;
                    if (isRoot)
                    {
                        newRoot = node;
                        isRoot = false;
                    }
                    else
                    {
                        // clone in the direction of consumption, i.e. the root and all it's descendants
                        newRoot = node.Clone(CloneFilters.Backward);
                    }

                    // connect the consumer to the producer
                    Connect(newRoot, producer, input);

                    this.Build(producer, existingInputs, constraints);

                    yield return newRoot;
                }
            }
        }

        //solve(rate, depth)
        //{
        //    if (!depth)
        //    {
        //        depth = 0;
        //    }
        //    const multiplier = rate / this.rate;
        //    let indent = "";
        //    for (let i = 0; i < depth; i++)
        //        indent += "  ";

        //    const result = new SolveResult(this.name, rate, this.units, this.building, multiplier, depth);

        //    let bom = { };
        //    bom[this.name] = result;

        //    const node = new SolveNode(result, bom);

        //    for (var i = 0; i < this.inputs.length; i++)
        //    {
        //        let input = this.inputs[i];
        //        let rateNeeded = input.rate * multiplier;

        //        // child is SolveNode
        //        let child = input.part.solve(rateNeeded, depth + 1);
        //        node.children.unshift(child);
        //    }

        //    return node;
        //}
    }
}