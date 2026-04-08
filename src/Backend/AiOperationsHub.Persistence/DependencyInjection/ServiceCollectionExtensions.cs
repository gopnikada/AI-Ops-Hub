using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Persistence.Db;
using AiOperationsHub.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiOperationsHub.Persistence.DependencyInjection
{
    /// <summary>
    /// Provides dependency injection registration helpers for the persistence layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers SQL Server persistence services, repositories, and EF Core context.
        /// </summary>
        /// <param name="services">The service collection being configured.</param>
        /// <param name="configuration">The application configuration used to resolve connection strings.</param>
        /// <returns>The same service collection for fluent registration chaining.</returns>
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IActionProposalRepository, ActionProposalRepository>();
            services.AddScoped<IAuditEventRepository, AuditEventRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}