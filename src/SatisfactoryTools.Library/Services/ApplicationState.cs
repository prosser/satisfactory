namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using Storage;

    public class ApplicationState
    {
        private readonly IStorageProvider storage;

        public ApplicationState(IObjectLookupService lookup, IStorageProvider storage)
        {
            this.storage = storage;
            this.Solver = new SolverState(lookup);
            Task.Run(() => this.Solver.LoadAsync(storage, CancellationToken.None)).Wait();
        }

        public SolverState Solver { get; set; }

        public async Task LoadAsync(CancellationToken ct = default)
        {
            await this.Solver.LoadAsync(this.storage, ct).ConfigureAwait(false);
        }

        public async Task SaveAsync(CancellationToken ct = default)
        {
            await this.Solver.SaveAsync(this.storage, ct).ConfigureAwait(false);
        }
    }

    public class SolverState
    {
        private readonly IObjectLookupService lookup;

        private string selectedRecipeName;

        public SolverState(IObjectLookupService lookup)
        {
            this.lookup = lookup;
        }

        public Dictionary<string, double> InputRates { get; } = new Dictionary<string, double>();

        public double Rate { get; set; }

        public Recipe SelectedRecipe { get; private set; }

        public string SelectedRecipeName
        {
            get => this.selectedRecipeName;
            set
            {
                if (this.selectedRecipeName != value)
                {
                    this.selectedRecipeName = value;
                    this.SelectedRecipe = this.lookup.Lookup<Recipe>(this.SelectedRecipeName);
                }
            }
        }

        /// <summary>
        /// Initializes from browser local storage
        /// </summary>
        public async Task LoadAsync(IStorageProvider storage, CancellationToken ct)
        {
            if (await storage.ContainKeyAsync(nameof(SolverState)).ConfigureAwait(false))
            {
                SolverState loaded = await storage.GetItemAsync<SolverState>(nameof(SolverState)).ConfigureAwait(false);
                this.Rate = loaded.Rate;
                this.SelectedRecipeName = loaded.SelectedRecipeName;
            }
        }

        public async Task SaveAsync(IStorageProvider storage, CancellationToken ct)
        {
            await storage.SetItemAsync(nameof(SolverState), ct).ConfigureAwait(false);
        }
    }
}