using AiOperationsHub.Api.Authentication;
using AiOperationsHub.Api.Authorization;
using AiOperationsHub.Api.Infrastructure;
using AiOperationsHub.Application.DependencyInjection;
using AiOperationsHub.Infrastructure.DependencyInjection;
using AiOperationsHub.Persistence.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace AiOperationsHub.Api
{
    /// <summary>
    /// Entry point for the AI Operations Hub API.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Configures and runs the API host.
        /// </summary>
        /// <param name="args">The process command-line arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            builder.Services
                .AddOptions<JwtOptions>()
                .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>() ?? throw new InvalidOperationException(
                    $"Configuration section '{JwtOptions.SectionName}' is missing or invalid.");

            if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
            {
                throw new InvalidOperationException(
                    $"Configuration value '{JwtOptions.SectionName}:SigningKey' must be provided.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        NameClaimType = JwtClaimTypes.Subject,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(AuthorizationPolicies.CanReadProposals, policy =>
                {
                    policy.RequireRole(ApplicationRoles.Admin, ApplicationRoles.Operator, ApplicationRoles.Auditor);
                });

                options.AddPolicy(AuthorizationPolicies.CanCreateProposals, policy =>
                {
                    policy.RequireRole(ApplicationRoles.Admin, ApplicationRoles.Operator);
                });

                options.AddPolicy(AuthorizationPolicies.CanConfirmProposals, policy =>
                {
                    policy.RequireRole(ApplicationRoles.Admin, ApplicationRoles.Operator);
                });
            });

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
                .AllowAnonymous();

            await app.RunAsync();
        }
    }
}