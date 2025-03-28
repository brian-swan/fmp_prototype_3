using System;
using System.Collections.Generic;

namespace FMP.Core.Models
{
    /// <summary>
    /// Represents a feature flag in the system
    /// </summary>
    public class FeatureFlag
    {
        /// <summary>
        /// Unique identifier for the feature flag
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of the feature flag
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Description of what the feature flag controls
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Key used in code to check the flag status
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the feature flag is enabled globally
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// The environment-specific settings for this flag
        /// </summary>
        public List<EnvironmentConfig> EnvironmentConfigs { get; set; } = new List<EnvironmentConfig>();
        
        /// <summary>
        /// Tags associated with this feature flag
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// When the feature flag was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the feature flag was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Flag targeting rules for specific users, groups, etc.
        /// </summary>
        public List<TargetingRule> TargetingRules { get; set; } = new List<TargetingRule>();
    }
}