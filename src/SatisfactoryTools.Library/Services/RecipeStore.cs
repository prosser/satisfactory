namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Models.Dto;

    public class RecipeStore : IRecipeStore
    {
        private readonly IObjectLookupService lookup;

        private readonly IPartStore partStore;

        private readonly ConcurrentDictionary<int, Recipe> recipes = new ConcurrentDictionary<int, Recipe>();

        private readonly ConcurrentDictionary<Part, ConcurrentBag<Recipe>> recipesByInput =
            new ConcurrentDictionary<Part, ConcurrentBag<Recipe>>();

        private readonly ConcurrentDictionary<Part, ConcurrentBag<Recipe>> recipesByOutput =
            new ConcurrentDictionary<Part, ConcurrentBag<Recipe>>();

        private readonly HashSet<Recipe> unlocked = new HashSet<Recipe>();

        public RecipeStore(IPartStore partStore, IObjectLookupService lookup)
        {
            this.partStore = partStore;
            this.lookup = lookup;
        }

        public int Count => this.recipes.Count;

        public IEnumerator<Recipe> GetEnumerator()
        {
            return this.GetUnlockedRecipes().GetEnumerator();
        }

        public IEnumerable<Recipe> AllRecipes =>
            this.recipes.Values.OrderBy(x => x.IsUnlockable ? 1 : 0).ThenBy(x => x.Name);

        public IEnumerable<Recipe> GetRecipesForInput(Part part)
        {
            return this.recipesByInput[part].Where(this.unlocked.Contains);
        }

        public IEnumerable<Recipe> AlternateRecipes =>
            this.recipes.Values.Where(x => x.IsUnlockable).OrderBy(x => x.Name);

        public IEnumerable<Recipe> GetRecipesForOutput(Part part)
        {
            return this.recipesByOutput[part].Where(this.unlocked.Contains);
        }

        public void Load(ItemsDto data)
        {
            int id = 1;

            foreach (RecipeDto dto in data.Recipes)
            {
                this.Add(new Recipe(this.partStore, id++, dto));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void Add(Recipe recipe)
        {
            if (!this.recipes.TryAdd(recipe.Id, recipe))
            {
                throw new InvalidOperationException("Duplicate recipe id");
            }

            if (!recipe.IsUnlockable)
            {
                this.unlocked.Add(recipe);
            }

            foreach (PartIo output in recipe.Outputs)
            {
                ConcurrentBag<Recipe> byOutput =
                    this.recipesByOutput.GetOrAdd(output.Part, _ => new ConcurrentBag<Recipe>());
                byOutput.Add(recipe);
            }

            foreach (PartIo input in recipe.Inputs)
            {
                ConcurrentBag<Recipe> byInput =
                    this.recipesByInput.GetOrAdd(input.Part, _ => new ConcurrentBag<Recipe>());
                byInput.Add(recipe);
            }

            this.lookup.Add(recipe.Id, recipe);
        }

        public IEnumerable<Recipe> GetUnlockedRecipes()
        {
            return this.unlocked.OrderBy(x => x.IsUnlockable ? 1 : 0).ThenBy(x => x.Name);
        }
    }
}