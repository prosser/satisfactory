namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SatisfactoryTools.Models;
    using SatisfactoryTools.Models.Dto;

    public interface IRecipeStore : IEnumerable<Recipe>
    {
        int Count { get; }

        IEnumerable<Recipe> GetRecipesForInput(Part part);

        IEnumerable<Recipe> GetRecipesForOutput(Part part);

        IEnumerable<Recipe> GetUnlockedRecipes();

        void Load(ItemsDto data);
    }
}