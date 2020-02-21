namespace SatisfactoryTools.Models.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RecipeDto
    {
        public string[] Buildings { get; set; }

        public PartIoDto[] Inputs { get; set; }

        public string Name { get; set; }

        public PartIoDto[] Outputs { get; set; }

        public int Time { get; set; }
    }
}