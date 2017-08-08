using ETCD.V3;
using Microsoft.Extensions.Configuration;

namespace Pandv.Config
{
    public class PandvConfigurationSource : IConfigurationSource
    {
        public string SystemName { get; set; }
        public bool ReloadOnChange { get; set; }
        public Client Client { get; set; }
        public string RootPath { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new PandvConfigurationProvider(this);
        }
    }
}