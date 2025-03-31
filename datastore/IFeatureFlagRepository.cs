using System.Collections.Generic;
using System.Threading.Tasks;
using fmp_prototype_3.Models;

namespace fmp_prototype_3.DataStore
{
    /// <summary>
    /// Interface for feature flag repository operations
    /// </summary>
    public interface IFeatureFlagRepository
    {
        /// <summary>
        /// Get all feature flags
        /// </summary>
        Task<List<FeatureFlag>> GetAllFeatureFlagsAsync();
        
        /// <summary>
        /// Get a feature flag by key
        /// </summary>
        Task<FeatureFlag?> GetFeatureFlagAsync(string key);
        
        /// <summary>
        /// Create a new feature flag
        /// </summary>
        Task<FeatureFlag> CreateFeatureFlagAsync(FeatureFlag featureFlag);
        
        /// <summary>
        /// Update an existing feature flag
        /// </summary>
        Task<FeatureFlag?> UpdateFeatureFlagAsync(string key, FeatureFlag featureFlag);
        
        /// <summary>
        /// Delete a feature flag
        /// </summary>
        Task<bool> DeleteFeatureFlagAsync(string key);
    }
}