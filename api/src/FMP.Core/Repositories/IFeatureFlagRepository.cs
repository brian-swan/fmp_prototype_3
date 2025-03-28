using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMP.Core.Models;

namespace FMP.Core.Repositories
{
    /// <summary>
    /// Interface for feature flag repository operations
    /// </summary>
    public interface IFeatureFlagRepository
    {
        /// <summary>
        /// Gets all feature flags
        /// </summary>
        Task<IEnumerable<FeatureFlag>> GetAllAsync();
        
        /// <summary>
        /// Gets a feature flag by its ID
        /// </summary>
        Task<FeatureFlag?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Gets a feature flag by its key
        /// </summary>
        Task<FeatureFlag?> GetByKeyAsync(string key);
        
        /// <summary>
        /// Creates a new feature flag
        /// </summary>
        Task<FeatureFlag> CreateAsync(FeatureFlag featureFlag);
        
        /// <summary>
        /// Updates an existing feature flag
        /// </summary>
        Task<FeatureFlag> UpdateAsync(FeatureFlag featureFlag);
        
        /// <summary>
        /// Deletes a feature flag by its ID
        /// </summary>
        Task<bool> DeleteAsync(Guid id);
        
        /// <summary>
        /// Gets feature flags with specific tags
        /// </summary>
        Task<IEnumerable<FeatureFlag>> GetByTagsAsync(IEnumerable<string> tags);
    }
}