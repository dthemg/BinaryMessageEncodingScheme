using FluentAssertions;
using NUnit.Framework;
using SinchMessageEncoder.Interfaces;
using SinchMessageEncoder.Models;
using SinchMessageEncoder.Services;
using System.Text;

namespace SinchMessageEncoderTests;

[TestFixture]
public class TestSinchMessageEncoder
{
    private IEncoderService _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new EncoderService();
    }

    [Test]
    public void EncodeMessage_should_create_a_message_in_the_correct_format()
    {
        var message = new Message()
        {
            Headers = new()
            {
                { "a", "b" },
                { "c", "d" }
            },
            Payload = Encoding.ASCII.GetBytes("e")
        };
        var encodedMessage = _sut.EncodeMessage(message).ToArray();

        var expectedEncodedMessage = new byte[] {
            AsAscii("a"), Constants.EndOfHeaderName, AsAscii("b"), Constants.EndOfHeaderValue,
            AsAscii("c"), Constants.EndOfHeaderName, AsAscii("d"), Constants.EndOfHeaderValue,
            Constants.StartOfPayload, AsAscii("e")
        };

        encodedMessage.Should().HaveCount(10);
        encodedMessage.Should().Equal(expectedEncodedMessage);
    }

    [Test]
    public void EncodeMessage_should_throw_on_non_ascii_headers()
    {
        var message = new Message()
        {
            Headers = new() { { "a", "åäö" } }
        };

        var encodeMessage = () => _sut.EncodeMessage(message);

        encodeMessage.Should().Throw<ArgumentException>();
    }

    [Test]
    public void EncodeMessage_should_throw_on_too_many_headers()
    {
        var message = new Message();
        for (int i = 0; i < 64; i++)
        {
            message.Headers.Add(i.ToString(), "value");
        }

        Action encodeMessage = () => _sut.EncodeMessage(message);

        encodeMessage.Should().Throw<ArgumentException>();
    }

    [Test]
    public void EncodeMessage_should_handle_empty_payload()
    {
        var message = new Message()
        {
            Headers = new()
            {
                { "a", "b" }
            }
        };
        var encodedMessage = _sut.EncodeMessage(message).ToArray();

        var expectedEncodedMessage = new byte[] {
            AsAscii("a"), Constants.EndOfHeaderName, AsAscii("b"), Constants.EndOfHeaderValue,
            Constants.StartOfPayload
        };

        encodedMessage.Should().Equal(expectedEncodedMessage);
    }

    [Test]
    public void EncodeMessage_should_handle_no_headers_and_empty_payload()
    {
        var message = new Message();

        var encodedMessage = _sut.EncodeMessage(message).ToArray();

        var expecteEncodedMessage = new byte[]
        {
            Constants.StartOfPayload
        };

        encodedMessage.Should().Equal(expecteEncodedMessage);
    }

    private static byte AsAscii(string s)
    {
        var bytes = Encoding.ASCII.GetBytes(s);
        return bytes[0];
    }
}
