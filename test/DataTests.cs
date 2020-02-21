namespace SatisfactoryTools.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;

    using SatisfactoryTools.Models.Dto;
    using SatisfactoryTools.Services;
    using SatisfactoryTools.Test.Services;

    using Xunit;

    public class DataTests
    {
        private readonly string json;

        public DataTests()
        {
            this.json = File.ReadAllText(@"Content\items.json");
        }

        [Fact]
        public async Task ItemsDtoDeserializes()
        {
            var partStoreMock = new Mock<IPartStore>();
            partStoreMock.Setup(x => x.Load(It.IsAny<ItemsDto>()));

            var recipeStoreMock = new Mock<IRecipeStore>();
            recipeStoreMock.Setup(x => x.Load(It.IsAny<ItemsDto>()));

            TestDataLoader loader = new TestDataLoader(this.json, partStoreMock.Object, recipeStoreMock.Object);
            await loader.LoadDataAsync().ConfigureAwait(false);

            partStoreMock.Verify(x => x.Load(It.IsAny<ItemsDto>()));
            recipeStoreMock.Verify(x => x.Load(It.IsAny<ItemsDto>()));
            partStoreMock.VerifyNoOtherCalls();
            recipeStoreMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PartStoreLoads()
        {
            var recipeStoreMock = new Mock<IRecipeStore>();
            recipeStoreMock.Setup(x => x.Load(It.IsAny<ItemsDto>()));

            IPartStore partStore = new PartStore();

            TestDataLoader loader = new TestDataLoader(this.json, partStore, recipeStoreMock.Object);
            await loader.LoadDataAsync().ConfigureAwait(false);

            Assert.NotEmpty(partStore);
        }

        [Fact]
        public async Task<(IPartStore Parts, IRecipeStore Recipes)> RecipeStoreLoads()
        {
            IObjectLookupService objectLookupMock = Mock.Of<IObjectLookupService>();
            IPartStore partStore = new PartStore();
            IRecipeStore recipeStore = new RecipeStore(partStore, objectLookupMock);

            TestDataLoader loader = new TestDataLoader(this.json, partStore, recipeStore);
            await loader.LoadDataAsync().ConfigureAwait(false);

            Assert.NotEmpty(recipeStore);

            return (partStore, recipeStore);
        }
    }
}