namespace SatisfactoryTools.Models.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemsDto
    {
        public List<int> Fluids { get; set; }

        public List<string> Parts { get; set; }

        public List<RecipeDto> Recipes { get; set; }
    }
}