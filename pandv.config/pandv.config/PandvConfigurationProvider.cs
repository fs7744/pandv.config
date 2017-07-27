using Etcdserverpb;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using static Etcdserverpb.KV;
using static Etcdserverpb.Watch;

namespace pandv.config
{
    internal class PandvConfigurationProvider : ConfigurationProvider
    {
        private PandvConfigurationSource source;
        private KVClient kv;
        private AsyncDuplexStreamingCall<WatchRequest, WatchResponse> watch;

        public PandvConfigurationProvider(PandvConfigurationSource pandvConfigurationSource)
        {
            source = pandvConfigurationSource;
        }

        public override void Load()
        {
            kv = new KVClient(source.Channel);
            var result = kv.Range(new Etcdserverpb.RangeRequest()
            {
                Key = ByteString.CopyFromUtf8(source.SystemName),
                RangeEnd = ByteString.CopyFromUtf8("\0")
            });
            foreach (var item in result.Kvs)
            {
                Data.Add(item.Key.ToStringUtf8(), item.Value.ToStringUtf8());
            }
            watch = new WatchClient(source.Channel).Watch();
            watch.RequestStream.WriteAsync(new Etcdserverpb.WatchRequest()
            {
                CreateRequest = new Etcdserverpb.WatchCreateRequest()
                {
                    Key = ByteString.CopyFromUtf8(source.SystemName),
                    RangeEnd = ByteString.CopyFromUtf8("\0")
                }
            });
            while (await watch.ResponseStream.MoveNext())
            {
                var res = watch.ResponseStream.Current;
                foreach (var e in res.Events)
                {
                    switch (e.Type)
                    {
                        case Mvccpb.Event.Types.EventType.Put:
                            Data[e.Kv.Key.ToStringUtf8()] = e.Kv.Value.ToStringUtf8();
                            break;
                        case Mvccpb.Event.Types.EventType.Delete:
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
            kv.Put(new Etcdserverpb.PutRequest()
            {
                Key = ByteString.CopyFromUtf8(source.SystemName + key),
                Value = ByteString.CopyFromUtf8(value)
            });
        }
    }
}