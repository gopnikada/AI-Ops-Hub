using AiOperationsHub.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiOperationsHub.Application.DependencyInjection
{
    /// <summary>
    /// Provides dependency injection registration helpers for the application layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application-layer MediatR handlers, validators, and pipeline behaviors.
        /// </summary>
        /// <param name="services">The service collection being configured.</param>
        /// <returns>The same service collection for fluent registration chaining.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));

            return services;
        }
    }
}