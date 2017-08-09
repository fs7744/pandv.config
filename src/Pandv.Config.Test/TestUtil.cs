using Google.Protobuf;
using System;

namespace Pandv.Config.Test
{
    public static class TestUtil
    {
        public static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public static ByteString RandomByteString()
        {
            return ByteString.CopyFromUtf8(RandomString());
        }
    }
}