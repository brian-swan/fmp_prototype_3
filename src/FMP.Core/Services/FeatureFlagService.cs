using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMP.Core.Models;
using FMP.Core.Repositories;

namespace FMP.Core.Services
{
    /// <summary>
    /// Service for managing feature flags
    /// </summary>
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly IFeatureFlagRepository _repository;

        /// <summary>
        /// Creates a new instance of the feature flag service
        /// </summary>
        public FeatureFlagService(IFeatureFlagRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <inheritdoc />
        public Task<IEnumerable<FeatureFlag>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> GetByKeyAsync(string key)
        {
            return _repository.GetByKeyAsync(key);
        }

        /// <inheritdoc />
        public async Task<bool> IsEnabledAsync(string key, string environment = "Production")
        {
            var featureFlag = await _repository.GetByKeyAsync(key);
            
            if (featureFlag == null || !featureFlag.Enabled)
            {
                return false;
            }

            var envConfig = featureFlag.EnvironmentConfigs?.FirstOrDefault(e => 
                e.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));

            return envConfig?.Enabled ?? false;
        }

        /// <inheritdoc />
        public Task<FeatureFlag> CreateAsync(FeatureFlag featureFlag)
        {
            // Validate feature flag
            ValidateFeatureFlag(featureFlag);
            
            return _repository.CreateAsync(featureFlag);
        }

        /// <inheritdoc />
        public async Task<FeatureFlag> UpdateAsync(FeatureFlag featureFlag)
        {
            // Validate feature flag exists
            var existingFlag = await _repository.GetByIdAsync(featureFlag.Id);
            if (existingFlag == null)
            {
                throw new KeyNotFoundException($"Feature flag with ID {featureFlag.Id} not found");
            }
            
            // Validate feature flag
            ValidateFeatureFlag(featureFlag);
            
            return await _repository.UpdateAsync(featureFlag);
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync(Guid id)
        {
            return _repository.DeleteAsync(id);
        }

        /// <inheritdoc />
        public Task<IEnumerable<FeatureFlag>> GetByTagsAsync(IEnumerable<string> tags)
        {
            return _repository.GetByTagsAsync(tags);
        }

        private void ValidateFeatureFlag(FeatureFlag featureFlag)
        {
            if (string.IsNullOrWhiteSpace(featureFlag.Name))
            {
                throw new ArgumentException("Feature flag name is required");
            }
            
            if (string.IsNullOrWhiteSpace(featureFlag.Key))
            {
                throw new ArgumentException("Feature flag key is required");
            }
        }
    }
}