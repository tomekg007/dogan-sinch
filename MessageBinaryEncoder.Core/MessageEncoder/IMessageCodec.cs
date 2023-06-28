namespace MessageBinaryEncoder.Core.MessageEncoder;

public interface IMessageCodec
{
    byte[] Encode(Message message);
    Message Decode(byte[] message);
}
