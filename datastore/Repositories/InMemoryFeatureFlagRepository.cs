using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmp_prototype_3.Models;

namespace fmp_prototype_3.DataStore.Repositories
{
    /// <summary>
    /// In-memory implementation of the feature flag repository for local development
    /// </summary>
    public class InMemoryFeatureFlagRepository : IFeatureFlagRepository
    {
        private readonly List<FeatureFlag> _featureFlags;

        /// <summary>
        /// Initializes a new instance of the InMemoryFeatureFlagRepository
        /// </summary>
        public InMemoryFeatureFlagRepository()
        {
            // Initialize with some sample feature flags
            _featureFlags = new List<FeatureFlag>
            {
                new FeatureFlag
                {
                    id = "dark-mode",
                    key = "dark-mode",
                    name = "Dark Mode",
                    description = "Enables dark mode in the UI",
                    enabled = true,
                    targetingRules = new List<TargetingRule>
                    {
                        new TargetingRule
                        {
                            id = "beta-users",
                            name = "Beta Users",
                            condition = "user.isBetaTester == true",
                            value = true
                        }
                    },
                    environmentConfigs = new Dictionary<string, EnvironmentConfig>
                    {
                        { "development", new EnvironmentConfig { enabled = true } },
                        { "staging", new EnvironmentConfig { enabled = true } },
                        { "production", new EnvironmentConfig { enabled = false } }
                    }
                },
                new FeatureFlag
                {
                    id = "new-dashboard",
                    key = "new-dashboard",
                    name = "New Dashboard",
                    description = "Enables the new dashboard experience",
                    enabled = false,
                    targetingRules = new List<TargetingRule>
                    {
                        new TargetingRule
                        {
                            id = "internal-users",
                            name = "Internal Users",
                            condition = "user.email.endsWith('@company.com')",
                            value = true
                        }
                    },
                    environmentConfigs = new Dictionary<string, EnvironmentConfig>
                    {
                        { "development", new EnvironmentConfig { enabled = true } },
                        { "staging", new EnvironmentConfig { enabled = false } },
                        { "production", new EnvironmentConfig { enabled = false } }
                    }
                },
                new FeatureFlag
                {
                    id = "search-improvements",
                    key = "search-improvements",
                    name = "Search Improvements",
                    description = "Enables improved search algorithm",
                    enabled = true,
                    environmentConfigs = new Dictionary<string, EnvironmentConfig>
                    {
                        { "development", new EnvironmentConfig { enabled = true } },
                        { "staging", new EnvironmentConfig { enabled = true } },
                        { "production", new EnvironmentConfig { enabled = true } }
                    }
                }
            };
        }

        /// <inheritdoc />
        public Task<List<FeatureFlag>> GetAllFeatureFlagsAsync()
        {
            return Task.FromResult(_featureFlags.ToList());
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> GetFeatureFlagAsync(string key)
        {
            var flag = _featureFlags.FirstOrDefault(f => f.key == key);
            return Task.FromResult(flag);
        }

        /// <inheritdoc />
        public Task<FeatureFlag> CreateFeatureFlagAsync(FeatureFlag featureFlag)
        {
            // Generate an ID if not specified
            if (string.IsNullOrEmpty(featureFlag.id))
            {
                featureFlag.id = Guid.NewGuid().ToString();
            }
            
            _featureFlags.Add(featureFlag);
            return Task.FromResult(featureFlag);
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> UpdateFeatureFlagAsync(string key, FeatureFlag featureFlag)
        {
            var existingIndex = _featureFlags.FindIndex(f => f.key == key);
            if (existingIndex == -1)
            {
                return Task.FromResult<FeatureFlag?>(null);
            }

            featureFlag.key = key;
            _featureFlags[existingIndex] = featureFlag;
            return Task.FromResult<FeatureFlag?>(featureFlag);
        }

        /// <inheritdoc />
        public Task<bool> DeleteFeatureFlagAsync(string key)
        {
            var removed = _featureFlags.RemoveAll(f => f.key == key) > 0;
            return Task.FromResult(removed);
        }
    }
}