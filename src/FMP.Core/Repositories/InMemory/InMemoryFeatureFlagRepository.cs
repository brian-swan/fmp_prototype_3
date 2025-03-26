using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMP.Core.Models;

namespace FMP.Core.Repositories.InMemory
{
    /// <summary>
    /// In-memory implementation of the feature flag repository
    /// </summary>
    public class InMemoryFeatureFlagRepository : IFeatureFlagRepository
    {
        private readonly ConcurrentDictionary<Guid, FeatureFlag> _featureFlags = new();

        /// <summary>
        /// Creates a new instance of the in-memory feature flag repository
        /// </summary>
        public InMemoryFeatureFlagRepository()
        {
            // Initialize with sample data
            SeedSampleData();
        }

        /// <inheritdoc />
        public Task<IEnumerable<FeatureFlag>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<FeatureFlag>>(_featureFlags.Values.ToList());
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> GetByIdAsync(Guid id)
        {
            _featureFlags.TryGetValue(id, out var featureFlag);
            return Task.FromResult(featureFlag);
        }

        /// <inheritdoc />
        public Task<FeatureFlag?> GetByKeyAsync(string key)
        {
            var featureFlag = _featureFlags.Values.FirstOrDefault(f => 
                f.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(featureFlag);
        }

        /// <inheritdoc />
        public Task<FeatureFlag> CreateAsync(FeatureFlag featureFlag)
        {
            if (featureFlag.Id == Guid.Empty)
            {
                featureFlag.Id = Guid.NewGuid();
            }

            featureFlag.CreatedAt = DateTime.UtcNow;
            featureFlag.UpdatedAt = DateTime.UtcNow;

            _featureFlags[featureFlag.Id] = featureFlag;
            return Task.FromResult(featureFlag);
        }

        /// <inheritdoc />
        public Task<FeatureFlag> UpdateAsync(FeatureFlag featureFlag)
        {
            if (!_featureFlags.ContainsKey(featureFlag.Id))
            {
                throw new KeyNotFoundException($"Feature flag with ID {featureFlag.Id} not found");
            }

            featureFlag.UpdatedAt = DateTime.UtcNow;
            _featureFlags[featureFlag.Id] = featureFlag;
            
            return Task.FromResult(featureFlag);
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync(Guid id)
        {
            var result = _featureFlags.TryRemove(id, out _);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<IEnumerable<FeatureFlag>> GetByTagsAsync(IEnumerable<string> tags)
        {
            var tagsList = tags.ToList();
            var result = _featureFlags.Values
                .Where(f => f.Tags.Any(t => tagsList.Contains(t, StringComparer.OrdinalIgnoreCase)))
                .ToList();
            
            return Task.FromResult<IEnumerable<FeatureFlag>>(result);
        }

        private void SeedSampleData()
        {
            // Sample feature flags for development
            var flags = new List<FeatureFlag>
            {
                new FeatureFlag
                {
                    Id = Guid.NewGuid(),
                    Name = "New Dashboard",
                    Description = "Enables the new dashboard UI for users",
                    Key = "new-dashboard",
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Tags = new List<string> { "ui", "dashboard" },
                    EnvironmentConfigs = new List<EnvironmentConfig>
                    {
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Development",
                            Enabled = true,
                            RolloutPercentage = 100,
                            UpdatedAt = DateTime.UtcNow.AddDays(-5)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Staging",
                            Enabled = true,
                            RolloutPercentage = 50,
                            UpdatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Production",
                            Enabled = false,
                            RolloutPercentage = 0,
                            UpdatedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    }
                },
                new FeatureFlag
                {
                    Id = Guid.NewGuid(),
                    Name = "API Rate Limiting",
                    Description = "Enables rate limiting for API endpoints",
                    Key = "api-rate-limiting",
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    Tags = new List<string> { "api", "performance" },
                    EnvironmentConfigs = new List<EnvironmentConfig>
                    {
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Development",
                            Enabled = false,
                            UpdatedAt = DateTime.UtcNow.AddDays(-3)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Staging",
                            Enabled = true,
                            UpdatedAt = DateTime.UtcNow.AddDays(-3)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Production",
                            Enabled = true,
                            UpdatedAt = DateTime.UtcNow.AddDays(-3)
                        }
                    }
                },
                new FeatureFlag
                {
                    Id = Guid.NewGuid(),
                    Name = "Dark Mode",
                    Description = "Enables dark mode UI theme",
                    Key = "dark-mode",
                    Enabled = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    Tags = new List<string> { "ui", "theme" },
                    TargetingRules = new List<TargetingRule>
                    {
                        new TargetingRule
                        {
                            Id = Guid.NewGuid(),
                            Type = TargetingType.User,
                            Values = new List<string> { "user123", "admin456" },
                            IsInclude = true
                        }
                    },
                    EnvironmentConfigs = new List<EnvironmentConfig>
                    {
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Development",
                            Enabled = true,
                            UpdatedAt = DateTime.UtcNow.AddDays(-10)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Staging",
                            Enabled = true,
                            UpdatedAt = DateTime.UtcNow.AddDays(-10)
                        },
                        new EnvironmentConfig
                        {
                            Id = Guid.NewGuid(),
                            Environment = "Production",
                            Enabled = true,
                            RolloutPercentage = 20,
                            UpdatedAt = DateTime.UtcNow.AddDays(-10)
                        }
                    }
                }
            };

            // Add sample data to the dictionary
            foreach (var flag in flags)
            {
                _featureFlags[flag.Id] = flag;
                
                // Link environment configs and targeting rules to their feature flag
                foreach (var config in flag.EnvironmentConfigs)
                {
                    config.FeatureFlagId = flag.Id;
                }
                
                foreach (var rule in flag.TargetingRules)
                {
                    rule.FeatureFlagId = flag.Id;
                }
            }
        }
    }
}