using Etcdserverpb;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System.Threading;
using static Etcdserverpb.KV;
using static Etcdserverpb.Watch;
using static Mvccpb.Event.Types;

namespace pandv.config
{
    internal class PandvConfigurationProvider : ConfigurationProvider
    {
        private PandvConfigurationSource source;
        private KVClient kv;
        private AsyncDuplexStreamingCall<WatchRequest, WatchResponse> watch;
        private CancellationToken cancellation = new CancellationToken();

        public PandvConfigurationProvider(PandvConfigurationSource pandvConfigurationSource)
        {
            source = pandvConfigurationSource;
        }

        public override void Load()
        {
            kv = new KVClient(source.Channel);
            var result = kv.Range(new RangeRequest()
            {
                Key = ByteString.CopyFromUtf8(source.SystemName),
                RangeEnd = ByteString.CopyFromUtf8("\0")
            });
            foreach (var item in result.Kvs)
            {
                Data.Add(ConfigurationPath.GetSectionKey(item.Key.ToStringUtf8()), item.Value.ToStringUtf8());
            }
            if (source.ReloadOnChange)
            {
                Watch();
            }
        }

        private async void Watch()
        {
            watch = new WatchClient(source.Channel).Watch();
            await watch.RequestStream.WriteAsync(new WatchRequest()
            {
                CreateRequest = new WatchCreateRequest()
                {
                    Key = ByteString.CopyFromUtf8(source.SystemName),
                    RangeEnd = ByteString.CopyFromUtf8("\0")
                }
            });
            while (await watch.ResponseStream.MoveNext(cancellation))
            {
                var res = watch.ResponseStream.Current;
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
            kv.Put(new PutRequest()
            {
                Key = ByteString.CopyFromUtf8(ConfigurationPath.Combine(source.SystemName, key)),
                Value = ByteString.CopyFromUtf8(value)
            });
        }
    }
}