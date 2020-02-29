namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public interface IProductionLineFactory
    {
        Line Create(PartIo target, IEnumerable<PartIo> existingInputs, IEnumerable<Recipe> constraints);
    }
}