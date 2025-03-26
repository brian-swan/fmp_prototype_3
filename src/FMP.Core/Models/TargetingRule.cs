using System;
using System.Collections.Generic;

namespace FMP.Core.Models
{
    /// <summary>
    /// Represents a targeting rule for a feature flag
    /// </summary>
    public class TargetingRule
    {
        /// <summary>
        /// Unique identifier for this targeting rule
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Reference to the feature flag this rule belongs to
        /// </summary>
        public Guid FeatureFlagId { get; set; }
        
        /// <summary>
        /// The type of targeting (User, Group, etc.)
        /// </summary>
        public TargetingType Type { get; set; }
        
        /// <summary>
        /// The values to target (user IDs, group names, etc.)
        /// </summary>
        public List<string> Values { get; set; } = new List<string>();
        
        /// <summary>
        /// Whether to include or exclude the targeted values
        /// </summary>
        public bool IsInclude { get; set; } = true;
    }
    
    /// <summary>
    /// Types of targeting available for feature flags
    /// </summary>
    public enum TargetingType
    {
        /// <summary>
        /// Target specific users
        /// </summary>
        User,
        
        /// <summary>
        /// Target specific groups
        /// </summary>
        Group,
        
        /// <summary>
        /// Target based on IP address
        /// </summary>
        IpAddress,
        
        /// <summary>
        /// Target based on device or browser
        /// </summary>
        Device,
        
        /// <summary>
        /// Target based on custom attributes
        /// </summary>
        Custom
    }
}