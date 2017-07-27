using Microsoft.Extensions.Configuration;
using System;

namespace pandv.config
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddFrom(this IConfigurationBuilder builder, string etcdAddress, string systemName, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrWhiteSpace(systemName))
            {
                throw new ArgumentNullException(nameof(systemName));
            }
            return builder.AddFrom(s =>
            {
                s.ETCDAddress = etcdAddress;
                s.SystemName = systemName;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
            });
        }

        public static IConfigurationBuilder AddFrom(this IConfigurationBuilder builder, Action<PandvConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}