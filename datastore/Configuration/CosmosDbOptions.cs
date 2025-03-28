namespace fmp_prototype_3.DataStore.Configuration
{
    /// <summary>
    /// Configuration options for Cosmos DB
    /// </summary>
    public class CosmosDbOptions
    {
        /// <summary>
        /// The section name in the configuration file
        /// </summary>
        public const string SectionName = "CosmosDb";

        /// <summary>
        /// The Cosmos DB endpoint URL
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;
        
        /// <summary>
        /// The primary key for the Cosmos DB account
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// The database name
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// The container name
        /// </summary>
        public string ContainerName { get; set; } = string.Empty;
    }
}