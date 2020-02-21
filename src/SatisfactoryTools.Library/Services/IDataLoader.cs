namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IDataLoader
    {
        Task LoadDataAsync();
    }
}