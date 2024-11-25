using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Core.Interfaces.Services;
using WidgetStore.Infrastructure.Repositories;
using WidgetStore.Infrastructure.Services;

namespace WidgetStore.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for configuring infrastructure services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds infrastructure services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register AutoMapper with all mapping profiles in this assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();

            // Configure and register queue storage
            services.Configure<QueueStorageConfig>(configuration.GetSection("QueueStorageConfig"));
            services.AddScoped<IQueueService, QueueService>();

            return services;
        }

        /// <summary>
        /// Configures JWT authentication
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="jwtConfig">JWT configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            JwtConfig jwtConfig)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // No expiration for POC
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                };
            });

            return services;
        }

        /// <summary>
        /// Adds CORS policy for development
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddDevCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DevCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        /// <summary>
        /// Adds caching services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            return services;
        }
    }
}