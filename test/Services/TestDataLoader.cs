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
        private readonly JsonSerializerOptions serializerOptions;

        public TestDataLoader(string json, IPartStore partStore, IRecipeStore recipeStore, JsonSerializerOptions serializerOptions)
        {
            this.json = json;
            this.partStore = partStore;
            this.recipeStore = recipeStore;
            this.serializerOptions = serializerOptions;
        }

        public Task LoadDataAsync()
        {
            ItemsDto data = JsonSerializer.Deserialize<ItemsDto>(this.json, this.serializerOptions);

            this.partStore.Load(data);
            this.recipeStore.Load(data);

            return Task.CompletedTask;
        }
    }
}