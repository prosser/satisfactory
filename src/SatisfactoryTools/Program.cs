namespace SatisfactoryTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazored.LocalStorage;
    using Microsoft.AspNetCore.Blazor.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Storage;

    public class Program
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddSingleton<IStorageProvider, LocalStorageProvider>();
            services.AddScoped<IDataLoader, WasmDataLoader>();
            Startup.ConfigureServices(services);
        }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            ConfigureServices(builder.Services);

            await builder.Build().RunAsync().ConfigureAwait(true);
        }
    }
}