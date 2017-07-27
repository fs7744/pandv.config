using Microsoft.Extensions.Configuration;

namespace pandv.config
{
    public class PandvConfigurationSource : IConfigurationSource
    {
        public bool Optional { get; set; }
        public string SystemName { get; set; }
        public bool ReloadOnChange { get; set; }
        public string ETCDAddress { get; internal set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new PandvConfigurationProvider(this);
        }
    }
}