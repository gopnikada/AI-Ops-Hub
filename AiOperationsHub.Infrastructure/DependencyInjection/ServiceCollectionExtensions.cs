using AiOperationsHub.Application.Abstractions;
using AiOperationsHub.Application.Abstractions.Audit;
using AiOperationsHub.Application.Abstractions.Jira;
using AiOperationsHub.Application.Abstractions.Providers;
using AiOperationsHub.Application.Abstractions.Security;
using AiOperationsHub.Infrastructure.Options;
using AiOperationsHub.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiOperationsHub.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Provides dependency injection registration helpers for the infrastructure layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers infrastructure services and binds configuration-backed options.
        /// </summary>
        /// <param name="services">The service collection being configured.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The same service collection for fluent registration chaining.</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<JiraOptions>()
                .Bind(configuration.GetSection(JiraOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<OpenAiOptions>()
                .Bind(configuration.GetSection(OpenAiOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<AnonymizationOptions>()
                .Bind(configuration.GetSection(AnonymizationOptions.SectionName))
                .ValidateOnStart();

            services.AddSingleton<PlaceholderGenerator>();

            services.AddScoped<IAuditTrailWriter, AuditTrailWriter>();
            services.AddScoped<IAnonymizationService, StructuredAnonymizationService>();

            services.AddHttpClient<IJiraConnector, JiraConnector>((serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<JiraOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddHttpClient<IAiProvider, OpenAiProvider>((serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenAiOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            return services;
        }
    }
}