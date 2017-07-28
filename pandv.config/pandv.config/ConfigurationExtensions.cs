using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System;

namespace pandv.config
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder UsePandv(this IConfigurationBuilder builder, Channel channel, string systemName, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrWhiteSpace(systemName))
            {
                throw new ArgumentNullException(nameof(systemName));
            }
            return builder.UsePandv(s =>
            {
                s.Channel = channel;
                s.SystemName = systemName;
                s.ReloadOnChange = reloadOnChange;
            });
        }

        public static IConfigurationBuilder UsePandv(this IConfigurationBuilder builder, Action<PandvConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}