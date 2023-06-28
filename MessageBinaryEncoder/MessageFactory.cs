using MessageBinaryEncoder.Core.MessageEncoder;

namespace MessageBinaryEncoder
{
    public static class MessageFactory
    {
        public static Message CreateSampleMessage()
            => new()
            {
                Headers = new Dictionary<string, string>()
                {
                    { "headerName", "headerValue" },
                    { "name", "value" },
                    { "fruit", "apple" },
                    { "", "empty" },
                    { "abc", "def" },
                    { "111", "222" },
                    { "zzz", "xxx" }
                },
                Payload = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8 }
            };
    }
}
