namespace Appart.Config
{
    public enum ConfigSearchType
    {
        Default,
        RegexpGroup
    }

    public class ConfigSearchItem
    {
        public string Key { get; set; }

        public string Begin { get; set; }

        public string End { get; set; }

        public ConfigSearchType SearchType { get; set; }
        public int RegexGroupId { get; internal set; }
        public string Regex { get; internal set; }

        public override string ToString()
        {
            return $"{Key}[{SearchType}]-{Begin}-{End}-{RegexGroupId}-{Regex}";
        }
    }
}
