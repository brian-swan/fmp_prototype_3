using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using fmp_prototype_3.Models;
using fmp_prototype_3.DataStore.Configuration;

namespace fmp_prototype_3.DataStore.Repositories
{
    /// <summary>
    /// Implementation of the feature flag repository using Azure Cosmos DB
    /// </summary>
    public class CosmosDbFeatureFlagRepository : IFeatureFlagRepository
    {
        private readonly CosmosClient? _cosmosClient;
        private readonly Container? _container;
        private readonly bool _isConfigured;
        private readonly IList<FeatureFlag> _fallbackData = new List<FeatureFlag>();

        /// <summary>
        /// Initializes a new instance of the CosmosDbFeatureFlagRepository
        /// </summary>
        public CosmosDbFeatureFlagRepository(IOptions<CosmosDbOptions> options)
        {
            var cosmosOptions = options.Value;
            
            // Check if required configuration is present
            if (string.IsNullOrEmpty(cosmosOptions.Endpoint) || 
                string.IsNullOrEmpty(cosmosOptions.Key) || 
                string.IsNullOrEmpty(cosmosOptions.DatabaseName) || 
                string.IsNullOrEmpty(cosmosOptions.ContainerName))
            {
                _isConfigured = false;
                return;
            }

            try
            {
                // Configure Cosmos DB client options
                CosmosClientOptions clientOptions = new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    },
                    ConnectionMode = ConnectionMode.Gateway,
                    RequestTimeout = TimeSpan.FromSeconds(10)
                };

                _cosmosClient = new CosmosClient(cosmosOptions.Endpoint, cosmosOptions.Key, clientOptions);
                _container = _cosmosClient.GetContainer(cosmosOptions.DatabaseName, cosmosOptions.ContainerName);
                _isConfigured = true;
            }
            catch
            {
                // If we failed to create the client, mark as not configured and use fallback
                _isConfigured = false;
            }
        }

        /// <inheritdoc />
        public async Task<List<FeatureFlag>> GetAllFeatureFlagsAsync()
        {
            if (!_isConfigured || _container == null)
            {
                return _fallbackData.ToList();
            }

            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var results = new List<FeatureFlag>();

                using (var iterator = _container.GetItemQueryIterator<FeatureFlag>(query))
                {
                    while (iterator.HasMoreResults)
                    {
                        var response = await iterator.ReadNextAsync();
                        results.AddRange(response.ToList());
                    }
                }

                return results;
            }
            catch
            {
                // Fall back to in-memory if Cosmos operation fails
                return _fallbackData.ToList();
            }
        }

        /// <inheritdoc />
        public async Task<FeatureFlag?> GetFeatureFlagAsync(string key)
        {
            if (!_isConfigured || _container == null)
            {
                return _fallbackData.FirstOrDefault(f => f.key == key);
            }

            try
            {
                var response = await _container.ReadItemAsync<FeatureFlag>(
                    key, new PartitionKey(key));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                // Fall back to in-memory if Cosmos operation fails
                return _fallbackData.FirstOrDefault(f => f.key == key);
            }
        }

        /// <inheritdoc />
        public async Task<FeatureFlag> CreateFeatureFlagAsync(FeatureFlag featureFlag)
        {
            // In Cosmos DB, we use the key as the id
            featureFlag.id = featureFlag.key;
            
            if (!_isConfigured || _container == null)
            {
                _fallbackData.Add(featureFlag);
                return featureFlag;
            }

            try
            {
                var response = await _container.CreateItemAsync(
                    featureFlag, new PartitionKey(featureFlag.key));
                return response.Resource;
            }
            catch
            {
                // Fall back to in-memory if Cosmos operation fails
                _fallbackData.Add(featureFlag);
                return featureFlag;
            }
        }

        /// <inheritdoc />
        public async Task<FeatureFlag?> UpdateFeatureFlagAsync(string key, FeatureFlag featureFlag)
        {
            // Ensure the ID matches the key for Cosmos DB
            featureFlag.id = key;
            featureFlag.key = key;
            
            if (!_isConfigured || _container == null)
            {
                var index = _fallbackData.ToList().FindIndex(f => f.key == key);
                if (index == -1) return null;
                
                _fallbackData[index] = featureFlag;
                return featureFlag;
            }

            try
            {
                var response = await _container.ReplaceItemAsync(
                    featureFlag, key, new PartitionKey(key));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                // Fall back to in-memory if Cosmos operation fails
                var index = _fallbackData.ToList().FindIndex(f => f.key == key);
                if (index == -1) return null;
                
                _fallbackData[index] = featureFlag;
                return featureFlag;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteFeatureFlagAsync(string key)
        {
            if (!_isConfigured || _container == null)
            {
                var removed = _fallbackData.Remove(_fallbackData.FirstOrDefault(f => f.key == key));
                return removed;
            }

            try
            {
                await _container.DeleteItemAsync<FeatureFlag>(
                    key, new PartitionKey(key));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            catch
            {
                // Fall back to in-memory if Cosmos operation fails
                var removed = _fallbackData.Remove(_fallbackData.FirstOrDefault(f => f.key == key));
                return removed;
            }
        }
    }
}