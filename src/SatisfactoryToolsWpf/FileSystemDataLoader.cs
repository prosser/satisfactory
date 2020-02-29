namespace SatisfactoryTools.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Models.Dto;
    using Services;

    internal class FileSystemDataLoader : IDataLoader
    {
        private readonly IPartStore partStore;

        private readonly IRecipeStore recipeStore;

        private readonly JsonSerializerOptions serializerOptions;

        public FileSystemDataLoader(IPartStore partStore, IRecipeStore recipeStore,
            JsonSerializerOptions serializerOptions)
        {
            this.partStore = partStore;
            this.recipeStore = recipeStore;
            this.serializerOptions = serializerOptions;
        }

        public async Task LoadDataAsync()
        {
            string json = await File.ReadAllTextAsync(Path.Combine("Data", "items.json")).ConfigureAwait(false);
            ItemsDto response = JsonSerializer.Deserialize<ItemsDto>(json, this.serializerOptions);

            this.partStore.Load(response);
            this.recipeStore.Load(response);
        }
    }
}