using ETCD.V3;
using Microsoft.Extensions.Configuration;
using System;

namespace Pandv.Config
{
    /// <summary>
    /// Configuration Extensions Methods
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// set configuration get key/value from etcd v3
        /// </summary>
        /// <param name="builder">Configuration Builder</param>
        /// <param name="systemName">key's group name</param>
        /// <param name="reloadOnChange">keep same with same key/value in etcd v3</param>
        /// <param name="target">etcd v3 host:port</param>
        /// <param name="user">user name</param>
        /// <param name="pwd">user password</param>
        /// <param name="rootPath">key's first path name</param>
        /// <returns>IConfigurationBuilder</returns>
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

        /// <summary>
        /// set configuration get key/value from custom PandvConfigurationSource
        /// </summary>
        /// <param name="builder">Configuration Builder</param>
        /// <param name="configureSource">custom PandvConfigurationSource</param>
        /// <returns>IConfigurationBuilder</returns>
        public static IConfigurationBuilder UsePandv(this IConfigurationBuilder builder, Action<PandvConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}