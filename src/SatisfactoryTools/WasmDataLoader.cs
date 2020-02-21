namespace SatisfactoryTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;

    using SatisfactoryTools.Models.Dto;
    using SatisfactoryTools.Services;

    public class WasmDataLoader : IDataLoader
    {
        private readonly string baseUri;

        private readonly HttpClient client;

        private readonly IPartStore partStore;

        private readonly IRecipeStore recipeStore;

        public WasmDataLoader(HttpClient client, string baseUri, IPartStore partStore, IRecipeStore recipeStore)
        {
            this.client = client;
            this.baseUri = baseUri;
            this.partStore = partStore;
            this.recipeStore = recipeStore;
        }

        public async Task LoadDataAsync()
        {
            ItemsDto response = await this.client.GetJsonAsync<ItemsDto>(this.baseUri + "items.json").ConfigureAwait(false);

            this.partStore.Load(response);
            this.recipeStore.Load(response);
        }
    }
}