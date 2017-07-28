using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace pandv.config
{
    public class PandvConfigurationSource : IConfigurationSource
    {
        public string SystemName { get; set; }
        public bool ReloadOnChange { get; set; }
        public Channel Channel { get; internal set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new PandvConfigurationProvider(this);
        }
    }
}