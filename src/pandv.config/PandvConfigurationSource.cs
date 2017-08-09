using ETCD.V3;
using Microsoft.Extensions.Configuration;

namespace Pandv.Config
{
    /// <summary>
    /// Pandv Configuration Source 
    /// It keep the config which about etcd v3 and key path
    /// </summary>
    public class PandvConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// key's group name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// if reloadOnChange == true, the all key/value will keep same with etcd v3 which's keys under $"{configName}:{systemName}:"
        /// else only once get value which's keys under $"{configName}:{systemName}:" 
        /// </summary>
        public bool ReloadOnChange { get; set; }
        /// <summary>
        /// etcd v3 client
        /// </summary>
        public Client Client { get; set; }
        /// <summary>
        /// key's first path name
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Build PandvConfigurationProvider
        /// </summary>
        /// <param name="builder">Configuration Builder</param>
        /// <returns>IConfigurationProvider</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new PandvConfigurationProvider(this);
        }
    }
}