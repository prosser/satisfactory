namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using SatisfactoryTools.Models;
    using SatisfactoryTools.Storage;

    public class ApplicationState
    {
        private readonly IObjectLookupService lookup;

        public ApplicationState(IObjectLookupService lookup)
        {
            this.lookup = lookup;
            this.Solver = new SolverState(lookup);
        }

        public SolverState Solver { get; set; }

        public async Task LoadAsync(IStorageProvider storage, CancellationToken ct)
        {
            await this.Solver.LoadAsync(storage, ct).ConfigureAwait(false);
        }
    }

    public class SolverState
    {
        private readonly IObjectLookupService lookup;

        public SolverState(IObjectLookupService lookup)
        {
            this.lookup = lookup;
        }

        public double Rate { get; set; }

        public Recipe SelectedRecipe => this.SelectedRecipeId == 0 ? null : this.lookup.Lookup<Recipe>(this.SelectedRecipeId);

        public int SelectedRecipeId { get; set; }

        /// <summary>
        /// Initializes from browser local storage
        /// </summary>
        public async Task LoadAsync(IStorageProvider storage, CancellationToken ct)
        {
            if (await storage.ContainKeyAsync(nameof(SolverState)).ConfigureAwait(false))
            {
                SolverState loaded = await storage.GetItemAsync<SolverState>(nameof(SolverState)).ConfigureAwait(false);
                this.Rate = loaded.Rate;
                this.SelectedRecipeId = loaded.SelectedRecipeId;
            }
        }
    }
}