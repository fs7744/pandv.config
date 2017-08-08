using ETCD.V3;
using Microsoft.Extensions.Configuration;
using System;

namespace Pandv.Config
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder UsePandv(this IConfigurationBuilder builder, string systemName,
            bool reloadOnChange, string target, string user = null, string pwd = null, string rootPath = "config")
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
                s.Client = new Client(target);
                s.Client.NewAuthToken(user, pwd);
                s.SystemName = systemName;
                s.RootPath = rootPath;
                s.ReloadOnChange = reloadOnChange;
            });
        }

        public static IConfigurationBuilder UsePandv(this IConfigurationBuilder builder, Action<PandvConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}