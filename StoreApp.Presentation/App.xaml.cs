using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BLL.AutoMapper;
using BLL.Services;
using DAL;
using DAL.Infrastructure;
using DAL.Repositories;

namespace StoreApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Register Repositories
            services.AddSingleton<IStoreRepository>(new FileStoreRepository("stores.csv"));
            services.AddSingleton<IProductRepository>(new FileProductRepository("products.csv"));

            // Register Mapper
            services.AddSingleton<IMapper>(provider =>
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>(); // Add your mapping profile

                });
                return mapperConfig.CreateMapper();
            });

            // Register Services
            services.AddSingleton<StoreService>();
            services.AddSingleton<ProductService>();

            // Register MainWindow
            services.AddTransient<MainWindow>();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Start MainWindow
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _serviceProvider.Dispose();
        }
    }
}
