using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using fmp_prototype_3.DataStore.Configuration;

namespace fmp_prototype_3.DataStore.Services
{
    /// <summary>
    /// Service to handle Cosmos DB initialization
    /// </summary>
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly CosmosDbOptions _options;

        /// <summary>
        /// Initializes a new instance of the CosmosDbService
        /// </summary>
        public CosmosDbService(IOptions<CosmosDbOptions> options)
        {
            _options = options.Value;
            _cosmosClient = new CosmosClient(_options.Endpoint, _options.Key);
        }

        /// <summary>
        /// Initialize Cosmos DB database and container
        /// </summary>
        public async Task InitializeAsync()
        {
            // Create database if it doesn't exist
            DatabaseResponse database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_options.DatabaseName);

            // Create container if it doesn't exist, with /key as partition key path
            await database.Database.CreateContainerIfNotExistsAsync(
                _options.ContainerName, 
                "/key",
                throughput: 400); // Minimum throughput
        }
    }
}