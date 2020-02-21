using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SatisfactoryTools.Test")]

namespace SatisfactoryTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;

    using SatisfactoryTools.Services;

    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IObjectLookupService, ObjectLookupService>();
            services.AddSingleton<IPartStore, PartStore>();
            //services.AddScoped<IProductionLineFactory, ProductionLineFactory>();
            services.AddScoped<IRecipeStore, RecipeStore>();

            services.AddSingleton<ApplicationState>();
        }
    }
}