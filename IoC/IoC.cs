using importacionmasiva.api.net.DataBase;
using importacionmasiva.api.net.Filters;
using importacionmasiva.api.net.Helpers;
using importacionmasiva.api.net.Repositories;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Services;
using importacionmasiva.api.net.Services.Interface;
using importacionmasiva.api.net.Utils.Csv;
using importacionmasiva.api.net.Utils.Excel;
using importacionmasiva.api.net.Utils.Txt;

namespace importacionmasiva.api.net.IoC
{
    public static class IoC
    {
        public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
        {
            // DataBase Connection
            services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(sp.GetRequiredService<IConfiguration>()));
            services.AddScoped<IDbContext>((serviceProvider) =>
            {
                var dbConnectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
                string registryName = ""; // Esto es solo un placeholder, se configura en el controller.
                return new DbContext(dbConnectionFactory, registryName);
            });

            // Filters
            services.AddScoped<Autenticacion>();
            services.AddScoped<DecodeHeader>();

            // Repositories
            services.AddScoped<IHttpClienteRepository, HttpClienteRepository>();
            services.AddScoped<IImportacionRepository, ImportacionRepository>();
            services.AddScoped<IImportacionLogsRepository, ImportacionLogsRepository>();

            // Services
            services.AddScoped<IAutenticacionService, AutenticacionService>();
            services.AddScoped<IImportacionService, ImportacionService>();
            services.AddScoped<IImportacionLogsService, ImportacionLogService>();

            // Utils
            services.AddScoped<IExcelUtils, ExcelUtils>();
            services.AddScoped<ICsvUtils, CsvUtils>();
            services.AddScoped<ITxtUtils, TxtUtils>();

            //Validators
            services.AddScoped<TableValidator>();

            return services;
        }
    }
}
