using ETCD.V3;
using Microsoft.Extensions.Configuration;
using System;

namespace Pandv.Config.Test
{
    public class ClientFixture : IDisposable
    {
        public const string Endpoints = "127.0.0.1:2379";
        public const string User = "root";
        public const string Pwd = "123";

        public ClientFixture()
        {
            Client = new Client(Endpoints);
            Client.NewAuthToken(User, Pwd);
        }

        public IConfiguration BuildConfig(string systemName, bool reloadOnChange)
        {
            return new ConfigurationBuilder()
                .UsePandv(systemName, reloadOnChange, Endpoints, User, Pwd)
                .Build();
        }

        public void Dispose()
        {
            Client.Close();
        }

        public Client Client { get; private set; }
    }
}