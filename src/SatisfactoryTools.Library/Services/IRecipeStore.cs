namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Models.Dto;

    public interface IRecipeStore : IEnumerable<Recipe>
    {
        IEnumerable<Recipe> AllRecipes { get; }

        IEnumerable<Recipe> AlternateRecipes { get; }

        int Count { get; }

        IEnumerable<Recipe> GetRecipesForInput(Part part);

        IEnumerable<Recipe> GetRecipesForOutput(Part part);

        void Load(ItemsDto data);
    }
}