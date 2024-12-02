using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BLL.AutoMapper;
using BLL.Services;
using DAL;
using DAL.Infrastructure;
using DAL.Repositories;
using DAL.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace StoreApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            // Load configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var services = new ServiceCollection();

            // Add AutoMapper
            services.AddScoped<IMapper>(_ =>
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                });
                return mapperConfig.CreateMapper();
            });

            // Add repositories based on configuration
            string repositoryType = configuration["RepositoryType"];
            if (repositoryType == "FileSystem")
            {
                // Register file system repositories
                services.AddSingleton<IProductRepository>(new FileProductRepository(configuration["FileSystem:ProductFilePath"]));
                services.AddSingleton<IStoreRepository>(new FileStoreRepository(configuration["FileSystem:StoreFilePath"]));
            }
            else if (repositoryType == "Database")
            {
                // Add SQLite database context
                services.AddDbContext<StoreAppContext>(options =>
                    options.UseSqlite(configuration["Database:ConnectionString"]));

                // Register database repositories
                services.AddScoped<IProductRepository, DatabaseProductRepository>();
                services.AddScoped<IStoreRepository, DatabaseStoreRepository>();
            }
            else
            {
                throw new Exception("Invalid repository type in configuration.");
            }

            // Register services
            services.AddScoped<StoreService>();
            services.AddScoped<ProductService>();

            // Register MainWindow
            services.AddTransient<MainWindow>();

            // Build the service provider
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
