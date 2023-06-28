namespace MessageBinaryEncoder.Core.MessageEncoder;

public class Message
{
    public IReadOnlyDictionary<string, string> Headers { get; set; } = null!;
    public byte[] Payload { get; set; } = null!;
}
