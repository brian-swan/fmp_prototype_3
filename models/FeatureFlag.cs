namespace fmp_prototype_3.Models
{
    public class FeatureFlag
    {
        public string id { get; set; } = string.Empty;
        public string key { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public bool enabled { get; set; }
        public List<TargetingRule>? targetingRules { get; set; }
        public Dictionary<string, EnvironmentConfig>? environmentConfigs { get; set; }
    }

    public class TargetingRule
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string condition { get; set; } = string.Empty;
        public bool value { get; set; }
    }

    public class EnvironmentConfig
    {
        public bool enabled { get; set; }
    }
}