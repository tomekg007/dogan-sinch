using MessageBinaryEncoder.Core.MessageEncoder;

namespace MessageBinaryEncoder.Tests
{
    public static class MessageCodecTestsFactory
    {
        public static string HeaderFirstName => "HeaderFirstName";
        public static string HeaderSecondName => "HeaderSecondName";
        public static string HeaderFirstValue => "HeaderFirstValue";
        public static string HeaderSecondValue => "HeaderSecondValue";
        public static string HeaderNameNotAscii => "HeaderNameNotAsciiΩ";
        public static string HeaderValueNotAscii => "HeaderValueNotAsciiΩ";
        public static byte PayloadFirstByte => 0x1;
        public static byte PayloadSecondByte => 0x2;
        public static string String1024Signs => new('A', 1024);


        public static Message CreateCorrectMessage()
            => new()
            {
                Headers = new Dictionary<string, string>
                {
                    { HeaderFirstName, HeaderFirstValue },
                    { HeaderSecondName, HeaderSecondValue }
                },
                Payload = new byte[] { PayloadFirstByte, PayloadSecondByte }
            };

        public static Message CreateMessage(Dictionary<string, string>? headers = null, byte[]? payload = null)
            => new()
            {
                Headers = headers ?? new Dictionary<string, string>
                {
                   { HeaderFirstName, HeaderFirstValue },
                },
                Payload = payload ?? new byte[] { PayloadFirstByte }
            };

        public static Message CreateMessage(int numberOfHeaders, int payloadSizeInBytes)
            => new()
            {
                Headers = GenerateHeaders(numberOfHeaders),
                Payload = GenerateByteArray(payloadSizeInBytes)
            };

        private static Dictionary<string, string> GenerateHeaders(int numberOfHeaders)
        {
            var result = new Dictionary<string, string>();
            for (int i = 1; i <= numberOfHeaders; i++)
            {
                result.Add($"Header {i}", $"Value {i}");
            }
            return result;
        }

        private static byte[] GenerateByteArray(int size)
        {
            var result = new byte[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = 0x1;
            }

            return result;
        }
    }
}
