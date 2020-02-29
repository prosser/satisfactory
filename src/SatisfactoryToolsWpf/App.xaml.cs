namespace SatisfactoryTools.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Services;
    using Storage;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost host;

        private static void ConfigureServices(IServiceCollection services)
        {
            SatisfactoryTools.Startup.ConfigureServices(services);

            services.AddSingleton<IStorageProvider, FileSystemStorageProvider>();
            services.AddScoped<IDataLoader, FileSystemDataLoader>();
            services.AddTransient<MainWindow>();
            services.AddScoped<MainWindowViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Task.Run(() =>
            {
                this.host.StopAsync(TimeSpan.FromSeconds(1.0));
                this.host.Dispose();
            }).Wait();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new HostBuilder();
            builder.ConfigureLogging(x => x.AddDebug());

            builder.ConfigureServices(ConfigureServices);

            this.host = builder.Build();
            this.host.Start();

            IDataLoader dataLoader = this.host.Services.GetRequiredService<IDataLoader>();

            Task.Run(dataLoader.LoadDataAsync).Wait();

            MainWindow mainWindow = this.host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}