namespace MessageBinaryEncoder.Core.MessageEncoder;

using System.Text;

public class MessageCodec : IMessageCodec
{
    // I assumed that these ASCII signs cannot occur in header ASCII string
    private const byte EndOfHeaderName = 0x01; // ASCII SOH sign
    private const byte EndOfHeaderValue = 0x02; // ASCII STX sign
    private const byte StartPayload = 0x03; // ASCII ETX sign

    private const int MaxNumberOfHeaders = 63;
    private const int MaxHeaderNameSizeInBytes = 1023;
    private const int MaxHeaderValueSizeInBytes = 1023;
    private const int MaxPayloadSizeInBytes = 262_144;

    public byte[] Encode(Message message)
    {
        Validate(message);

        var buffer = new List<byte>();
        buffer.AddRange(GetHeaders(message));
        buffer.Add(StartPayload);
        buffer.AddRange(message.Payload);

        return buffer.ToArray();
    }

    private static IReadOnlyList<byte> GetHeaders(Message message)
    {
        var headersBuffer = new List<byte>();
        if (message.Headers is not null)
        {
            foreach (var header in message.Headers)
            {
                headersBuffer.AddRange(Encoding.ASCII.GetBytes(header.Key));
                headersBuffer.Add(EndOfHeaderName);

                headersBuffer.AddRange(Encoding.ASCII.GetBytes(header.Value));
                headersBuffer.Add(EndOfHeaderValue);
            }
        }
        return headersBuffer;
    }

    private static void Validate(Message message)
    {
        if (message is null)
        {
            throw new ArgumentNullException("Message cannot be null");
        }
        if (message.Headers is not null && message.Headers.Any())
        {
            if (message.Headers.Count > MaxNumberOfHeaders)
            {
                throw new ArgumentException($"Max number of headers is {MaxNumberOfHeaders}");
            }

            foreach (var header in message.Headers)
            {

                if(!header.Key.All(char.IsAscii))
                {
                    throw new ArgumentException($"Message header name has to contain only ascii signs {header.Key}");
                }
                if (!header.Value.All(char.IsAscii))
                {
                    throw new ArgumentException($"Message header value has to contain only ascii signs {header.Value}");
                }
                if (Encoding.ASCII.GetByteCount(header.Key) > MaxHeaderNameSizeInBytes)
                {
                    throw new ArgumentException($"Max header name size is {MaxHeaderNameSizeInBytes}");
                }
                if (Encoding.ASCII.GetByteCount(header.Value) > MaxHeaderValueSizeInBytes)
                {
                    throw new ArgumentException($"Max header value size is {MaxHeaderValueSizeInBytes}");
                }
            }

            if (message.Payload.Count() > MaxPayloadSizeInBytes)
            {
                throw new ArgumentException($"Max payload size is {MaxPayloadSizeInBytes}");
            }
        }
    }

    public Message Decode(byte[] message)
    {
        // I skip message validation because I assumed that comes from our application without any modification
        // In real-world application, I have to validate the binary message structure

        var fullMessageSpan = new Span<byte>(message);
        var startPayloadPosition = GetStartPayloadPosition(fullMessageSpan);

        return new Message
        {
            Headers = GetHeaders(message, startPayloadPosition),
            Payload = GetPayload(fullMessageSpan, startPayloadPosition)
        };
    }

    private static IReadOnlyDictionary<string, string> GetHeaders(byte[] message, int startPayloadPosition)
    {
        var headers = new Dictionary<string, string>();

        if (ContainsHeaders(message))
        {
            var numberOfHeaders = GetNumberOfHeaders(message, startPayloadPosition);
            var startHeaderPosition = 0;

            for (var i = 0; i < numberOfHeaders; i++)
            {
                var headerSpan = new Span<byte>(message, startHeaderPosition, startPayloadPosition - startHeaderPosition + 1);

                var headerNameEndOfPosition = headerSpan.IndexOf(EndOfHeaderName);
                var headerNameSpan = headerSpan.Slice(0, headerNameEndOfPosition);
                var headerValueEndOfPosition = headerSpan.IndexOf(EndOfHeaderValue);
                var headerValueSpan = headerSpan.Slice(headerNameEndOfPosition + 1, (headerValueEndOfPosition) - (headerNameEndOfPosition + 1));
                headers.Add(Encoding.ASCII.GetString(headerNameSpan), Encoding.ASCII.GetString(headerValueSpan));
                startHeaderPosition += headerValueEndOfPosition + 1;
            }
        }

        return headers;
    }

    private static byte[] GetPayload(Span<byte> fullMessageSpan, int startPayloadPosition)
        => fullMessageSpan.Slice(startPayloadPosition + 1).ToArray();

    private static bool ContainsHeaders(byte[] message)
        => message.Contains(EndOfHeaderName);

    private static int GetNumberOfHeaders(byte[] message, int startPayloadPosition)
    {
        var headers = message.AsSpan(0, startPayloadPosition).ToArray();
        return headers.Count(x => x.Equals(EndOfHeaderName));
    }
    private static int GetStartPayloadPosition(Span<byte> fullMessageSpan)
        => fullMessageSpan.IndexOf(StartPayload);
}