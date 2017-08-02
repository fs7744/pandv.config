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

        public override void Load()
        {
            var result = _Source.Client.KV.Range(new RangeRequest()
            {
                Key = ByteString.CopyFromUtf8(_Source.SystemName),
                RangeEnd = Constants.NullKey
            });
            foreach (var item in result.Kvs)
            {
                Data.Add(ConfigurationPath.GetSectionKey(item.Key.ToStringUtf8()), item.Value.ToStringUtf8());
            }
            if (_Source.ReloadOnChange)
            {
                Watch();
            }
        }

        private async void Watch()
        {
            _Watch = _Source.Client.Watch.Watch();
            await _Watch.RequestStream.WriteAsync(new WatchRequest()
            {
                CreateRequest = new WatchCreateRequest()
                {
                    Key = ByteString.CopyFromUtf8(_Source.SystemName),
                    RangeEnd = Constants.NullKey
                }
            });
            while (await _Watch.ResponseStream.MoveNext(_Cancellation))
            {
                var res = _Watch.ResponseStream.Current;
                foreach (var e in res.Events)
                {
                    switch (e.Type)
                    {
                        case EventType.Put:
                            Data[e.Kv.Key.ToStringUtf8()] = e.Kv.Value.ToStringUtf8();
                            break;

                        case EventType.Delete:
                            Data.Remove(e.Kv.Key.ToStringUtf8());
                            break;

                        default:
                            break;
                    }
                }
                OnReload();
            }
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            _Source.Client.KV.Put(new PutRequest()
            {
                Key = ByteString.CopyFromUtf8(ConfigurationPath.Combine(_Source.SystemName, key)),
                Value = ByteString.CopyFromUtf8(value)
            });
        }
    }
}