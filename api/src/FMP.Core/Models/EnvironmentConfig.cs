using System;

namespace FMP.Core.Models
{
    /// <summary>
    /// Represents environment-specific configuration for a feature flag
    /// </summary>
    public class EnvironmentConfig
    {
        /// <summary>
        /// Unique identifier for this environment configuration
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Reference to the feature flag this configuration belongs to
        /// </summary>
        public Guid FeatureFlagId { get; set; }
        
        /// <summary>
        /// Name of the environment (e.g., Development, Staging, Production)
        /// </summary>
        public string Environment { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the feature flag is enabled in this environment
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Rollout percentage (0-100) if doing a percentage-based rollout
        /// </summary>
        public int RolloutPercentage { get; set; } = 0;
        
        /// <summary>
        /// When this environment configuration was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}