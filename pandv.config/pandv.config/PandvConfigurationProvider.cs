using ETCD.V3;
using Etcdserverpb;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System.Threading;
using static Mvccpb.Event.Types;

namespace Pandv.Config
{
    public class PandvConfigurationProvider : ConfigurationProvider
    {
        private PandvConfigurationSource _Source;
        private AsyncDuplexStreamingCall<WatchRequest, WatchResponse> _Watch;
        private CancellationToken _Cancellation = new CancellationToken();

        public PandvConfigurationProvider(PandvConfigurationSource source)
        {
            _Source = source;
        }

        private string GenerateKey(string key)
        {
            return ConfigurationPath.Combine(_Source.RootPath, _Source.SystemName, key);
        }

        private string GetUseKey(string key)
        {
            return ConfigurationPath.GetSectionKey(key);
        }

        public override void Load()
        {
            var result = _Source.Client.GetAll(_Source.SystemName);
            foreach (var item in result.Kvs)
            {
                Data.Add(GetUseKey(item.Key.ToStringUtf8()), item.Value.ToStringUtf8());
            }
            if (_Source.ReloadOnChange)
            {
                Watch();
            }
        }

        private async void Watch()
        {
            _Watch = _Source.Client.Watch();
            await _Watch.RequestStream.WriteAsync(new WatchRequest()
            {
                CreateRequest = new WatchCreateRequest()
                {
                    Key = ByteString.CopyFromUtf8(GenerateKey("")),
                    RangeEnd = Constants.NullKey
                }
            });
            while (await _Watch.ResponseStream.MoveNext(_Cancellation))
            {
                var res = _Watch.ResponseStream.Current;
                foreach (var e in res.Events)
                {
                    var key = GetUseKey(e.Kv.Key.ToStringUtf8());
                    if (e.Type == EventType.Put)
                    {
                        Data[key] = e.Kv.Value.ToStringUtf8();
                    }
                    else
                    {
                        Data.Remove(GetUseKey(e.Kv.Key.ToStringUtf8()));
                    }
                }
                OnReload();
            }
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            _Source.Client.Put(GenerateKey(key), value);
        }
    }
}