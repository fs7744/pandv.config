using ETCD.V3;
using System.Threading;
using Xunit;

namespace Pandv.Config.Test
{
    public class ConfigTest : Xunit.IClassFixture<ClientFixture>
    {
        private ClientFixture Fixture;

        public ConfigTest(ClientFixture clientFixture)
        {
            Fixture = clientFixture;
            Fixture.Client.DeleteAll("config");
        }

        [Fact]
        public void LoadConfiguration()
        {
            Fixture.Client.Put("config:test:test", "info");
            var config = Fixture.BuildConfig("test", false);
            Assert.Equal("info", config["test"]);
            Assert.Null(config["test1"]);
            Fixture.Client.Put("config:test:test", "info32");
            Assert.Equal("info", config["test"]);
            Assert.Null(config["test1"]);
        }

        [Fact]
        public void SetConfiguration()
        {
            var info = TestUtil.RandomString();
            var config = Fixture.BuildConfig("test", false);
            config["test4"] = info;
            Assert.Equal(0, Fixture.Client.Range("config:test:test4").Count);
        }

        [Fact]
        public void LoadReloadConfiguration()
        {
            var info = TestUtil.RandomString();
            var config = Fixture.BuildConfig("test", true);
            Assert.Null(config["test3"]);
            Fixture.Client.Put("config:test:test3", info);
            Thread.Sleep(5000);
            Assert.Equal(info, config["test3"]);
            
        }

        [Fact]
        public void SetReloadConfiguration()
        {
            var info = TestUtil.RandomString();
            var config = Fixture.BuildConfig("test", true);
            config["test4"] = info;
            Thread.Sleep(500);
            var result = Fixture.Client.Range("config:test:test4");
            Assert.Equal(1, result.Count);
            Assert.Equal(info, result.Kvs[0].Value.ToStringUtf8());
        }
    }
}