using FluentAssertions;
using MessageBinaryEncoder.Core.MessageEncoder;
using static MessageBinaryEncoder.Tests.MessageCodecTestsFactory;

namespace MessageBinaryEncoder.Tests
{
    public class MessageCodecTests
    {
        private MessageCodec _messageCodec;        

        public MessageCodecTests()
        {
            _messageCodec = new MessageCodec();
        }

        [Fact]
        public void ShouldEncodeEndDecodeMessageCorrectly()
        {
            var encodedMessage = _messageCodec.Encode(CreateCorrectMessage());
            var decodedMessage = _messageCodec.Decode(encodedMessage);


            decodedMessage.Headers.Count.Should().Be(2);
            var firstHeader = decodedMessage.Headers.First();
            var secondHeader = decodedMessage.Headers.Last();
            firstHeader.Key.Should().Be(HeaderFirstName);
            firstHeader.Value.Should().Be(HeaderFirstValue);
            secondHeader.Key.Should().Be(HeaderSecondName);
            secondHeader.Value.Should().Be(HeaderSecondValue);
            decodedMessage.Payload.First().Should().Be(PayloadFirstByte);
            decodedMessage.Payload.Last().Should().Be(PayloadSecondByte);
        }

        [Fact]
        public void ShouldEncodeEndDecodeMessageCorrectlyASci()
        {
            var encodedMessageNotAsciiHeaderName = () => _messageCodec.Encode(CreateMessage(new Dictionary<string, string>
                {
                   { HeaderNameNotAscii, HeaderFirstValue },
                }));

            var encodedMessageNotAsciiHeaderValue = () => _messageCodec.Encode(CreateMessage(new Dictionary<string, string>
                {
                   { HeaderFirstName, HeaderValueNotAscii },
                }));


            encodedMessageNotAsciiHeaderName.Should().Throw<ArgumentException>();
            encodedMessageNotAsciiHeaderValue.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(63)]

        public void ShouldEncodeEndDecodeMessageCorrectlyFor0And63HeadersAndPayload256KiB(int numberOfHeaders)
        {
            var encodedMessage = _messageCodec.Encode(CreateMessage(numberOfHeaders, 262144));
            var decodedMessage = _messageCodec.Decode(encodedMessage);

            decodedMessage.Headers.Count.Should().Be(numberOfHeaders);
        }

        [Fact]
        public void ShouldReturnArgumentExceptionForToManyHeaders()
        {
            var encodedMessage = () => _messageCodec.Encode(CreateMessage(64, 2));

            encodedMessage.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldReturnArgumentExceptionForLargePayload()
        {
            var encodedMessage = () => _messageCodec.Encode(CreateMessage(1, 262145));

            encodedMessage.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void ShouldThrowArgumentExceptionWhenHeaderNameSizeOrHeaderValueIsTooBig()
        {
            var encodedMessageTooBigHeaderSize = () => _messageCodec.Encode(CreateMessage(new Dictionary<string, string>
                {
                   { String1024Signs, HeaderFirstValue }
                }));

            var encodedMessageTooBigHeaderValueSize = () => _messageCodec.Encode(CreateMessage(new Dictionary<string, string>
                {
                   { HeaderFirstName, String1024Signs }
                }));

            encodedMessageTooBigHeaderSize.Should().Throw<ArgumentException>();
            encodedMessageTooBigHeaderValueSize.Should().Throw<ArgumentException>();
        }
    }
}