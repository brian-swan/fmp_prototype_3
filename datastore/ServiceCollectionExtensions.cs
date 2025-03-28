using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using fmp_prototype_3.DataStore.Configuration;
using fmp_prototype_3.DataStore.Repositories;
using fmp_prototype_3.DataStore.Services;

namespace fmp_prototype_3.DataStore
{
    /// <summary>
    /// Extension methods for setting up data store related services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the in-memory feature flag repository to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInMemoryFeatureFlagRepository(this IServiceCollection services)
        {
            services.AddSingleton<IFeatureFlagRepository, InMemoryFeatureFlagRepository>();
            return services;
        }

        /// <summary>
        /// Adds the Cosmos DB feature flag repository to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddCosmosDbFeatureFlagRepository(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure Cosmos DB options
            services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.SectionName));

            // Register Cosmos DB service
            services.AddSingleton<CosmosDbService>();

            // Register repository
            services.AddSingleton<IFeatureFlagRepository, CosmosDbFeatureFlagRepository>();

            return services;
        }

        /// <summary>
        /// Adds the feature flag repository to the service collection based on configuration
        /// </summary>
        public static IServiceCollection AddFeatureFlagRepository(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Check if "UseInMemoryRepository" is explicitly set to "true"
            var useInMemoryValue = configuration.GetSection("FeatureManagement")["UseInMemoryRepository"];
            bool useInMemory = string.Equals(useInMemoryValue, "true", StringComparison.OrdinalIgnoreCase);

            if (useInMemory)
            {
                services.AddInMemoryFeatureFlagRepository();
            }
            else
            {
                // Always register the CosmosDB options even if we're not using the repository directly
                // This ensures the service is available if needed
                services.Configure<CosmosDbOptions>(options =>
                {
                    var section = configuration.GetSection(CosmosDbOptions.SectionName);
                    options.Endpoint = section["Endpoint"] ?? string.Empty;
                    options.Key = section["Key"] ?? string.Empty;
                    options.DatabaseName = section["DatabaseName"] ?? string.Empty;
                    options.ContainerName = section["ContainerName"] ?? string.Empty;
                });

                services.AddSingleton<CosmosDbService>();
                services.AddSingleton<IFeatureFlagRepository, CosmosDbFeatureFlagRepository>();
            }

            return services;
        }
    }
}