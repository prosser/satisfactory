namespace SatisfactoryTools.Test.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SatisfactoryTools.Models;
    using SatisfactoryTools.Services;
    using Xunit;
    using Xunit.Abstractions;

    public class RecipeTreeTests
    {
        public RecipeTreeTests(ITestOutputHelper output)
        {
            this.output = output;
            this.dataTests = new DataTests();
        }

        private readonly ITestOutputHelper output;

        private readonly DataTests dataTests;

        [Fact]
        public async Task AllPartsConsumed()
        {
            (IPartStore parts, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            var tree = RecipeTree.Build(recipes.AllRecipes);

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

        [Fact]
        public async Task AllPartsProduced()
        {
            (IPartStore parts, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            var tree = RecipeTree.Build(recipes.AllRecipes);

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
        public async Task AllRecipesInTree()
        {
            (IPartStore _, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            var tree = RecipeTree.Build(recipes.AllRecipes);

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
        public async Task<RecipeTree> CanCreateEntireRecipeTree()
        {
            (IPartStore _, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            return RecipeTree.Build(recipes.AllRecipes);
        }

        [Fact]
        public async Task<RecipeTree> CanCreateStandardRecipeTree()
        {
            (IPartStore _, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);
            return RecipeTree.Build(recipes);
        }
    }
}