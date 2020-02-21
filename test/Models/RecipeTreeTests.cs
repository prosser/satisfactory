using SatisfactoryTools.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SatisfactoryTools.Test.Models
{
    using System.Threading.Tasks;
    using SatisfactoryTools.Models;

    public class RecipeTreeTests
    {
        private readonly DataTests dataTests;

        public RecipeTreeTests()
        {
            this.dataTests = new DataTests();
        }
        [Fact]
        public async Task CanCreateEntireRecipeTree()
        {
            (IPartStore parts, IRecipeStore recipes) = await this.dataTests.RecipeStoreLoads().ConfigureAwait(false);


            RecipeTree.Build(recipes);
        }
    }
}
