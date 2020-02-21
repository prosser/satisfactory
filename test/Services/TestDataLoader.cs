namespace SatisfactoryTools.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using SatisfactoryTools.Models.Dto;
    using SatisfactoryTools.Services;

    public class TestDataLoader : IDataLoader
    {
        private readonly string json;

        private readonly IPartStore partStore;

        private readonly IRecipeStore recipeStore;

        public TestDataLoader(string json, IPartStore partStore, IRecipeStore recipeStore)
        {
            this.json = json;
            this.partStore = partStore;
            this.recipeStore = recipeStore;
        }

        public Task LoadDataAsync()
        {
            ItemsDto data = JsonSerializer.Deserialize<ItemsDto>(this.json, SerializerOptions.JsonSerializerOptions);

            this.partStore.Load(data);
            this.recipeStore.Load(data);

            return Task.CompletedTask;
        }
    }
}