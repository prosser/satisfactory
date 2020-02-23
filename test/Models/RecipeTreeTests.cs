using SatisfactoryTools.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SatisfactoryTools.Test.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using SatisfactoryTools.Models;
    using Xunit.Abstractions;

    public class RecipeTreeTests
    {
        private readonly ITestOutputHelper output;

        private readonly DataTests dataTests;

        public RecipeTreeTests(ITestOutputHelper output)
        {
            this.output = output;
            this.dataTests = new DataTests();
        }
        [Fact]
        public async Task<RecipeTree> CanCreateEntireRecipeTree()
        {
            (IPartStore _, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);


            return RecipeTree.Build(recipes);
        }

        [Fact]
        public async Task AllRecipesInTree()
        {
            (IPartStore _, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            RecipeTree tree = RecipeTree.Build(recipes);

            foreach (Recipe recipe in recipes)
            {
                RecipeNode[] nodes = tree.FindRecipe(recipe).ToArray();

                if (nodes.Length == 0)
                {
                    throw new InvalidOperationException($"Recipe {recipe.Name} is not in the tree");
                }
                Assert.NotEmpty(tree.FindRecipe(recipe));
            }
        }

        [Fact]
        public async Task AllPartsProduced()
        {
            (IPartStore parts, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            RecipeTree tree = RecipeTree.Build(recipes);

            IEnumerable<Part> usedParts = parts.Where(part => recipes.Any(r => r.Outputs.Any(o => o.Part == part)));

            int errors = 0;
            foreach (Part part in usedParts)
            {
                RecipeNode[] producers = tree.FindRecipesThatProduce(part).ToArray();

                if (producers.Length == 0)
                {
                    this.output.WriteLine($"Part {part.Name} has no producers");
                    ++errors;
                }
            }

            Assert.Equal(0, errors);
        }

        [Fact]
        public async Task AllPartsConsumed()
        {
            (IPartStore parts, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            RecipeTree tree = RecipeTree.Build(recipes);

            IEnumerable<Part> usedParts = parts.Where(part => recipes.Any(r => r.Inputs.Any(o => o.Part == part)));

            int errors = 0;
            foreach (Part part in usedParts)
            {
                RecipeNode[] consumers = tree.FindRecipesThatConsume(part).ToArray();

                if (consumers.Length == 0)
                {
                    this.output.WriteLine($"Part {part.Name} has no consumers");
                    ++errors;
                }
            }

            Assert.Equal(0, errors);
        }
    }
}
