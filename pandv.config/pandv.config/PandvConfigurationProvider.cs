using Microsoft.Extensions.Configuration;

namespace pandv.config
{
    internal class PandvConfigurationProvider : ConfigurationProvider
    {
        private PandvConfigurationSource source;

        public PandvConfigurationProvider(PandvConfigurationSource pandvConfigurationSource)
        {
            source = pandvConfigurationSource;
        }

        public override void Load()
        {
            base.Load();
        }
    }
}