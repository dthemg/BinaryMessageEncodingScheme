using FluentAssertions;
using NUnit.Framework;
using SinchMessageEncoder.Interfaces;
using SinchMessageEncoder.Models;
using SinchMessageEncoder.Services;
using System.Text;

namespace SinchMessageEncoderTests;

[TestFixture]
public class DecoderTests
{
    private IDecoderService _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new DecoderService();
    }

    [Test]
    public void DecodeMessage_should_decode_message()
    {
        var inputBytes = new byte[]
        {
            AsAscii("a"), Constants.EndOfHeaderName,
            AsAscii("b"), Constants.EndOfHeaderValue,
            AsAscii("c"), Constants.EndOfHeaderName,
            AsAscii("d"), Constants.EndOfHeaderValue,
            Constants.StartOfPayload, AsAscii("e")
        };

        var decodedMessage = _sut.DecodeMessage(inputBytes);

        decodedMessage.Headers.Should().HaveCount(2);
        decodedMessage.Headers["a"].Should().Be("b");
        decodedMessage.Headers["c"].Should().Be("d");
        decodedMessage.Payload.Should().Equal(new byte[] { AsAscii("e") });
    }

    [Test]
    public void DecodeMessage_should_handle_empty_payload()
    {
        var inputBytes = new byte[]
        {
            AsAscii("a"), Constants.EndOfHeaderName,
            AsAscii("b"), Constants.EndOfHeaderValue,
            AsAscii("c"), Constants.EndOfHeaderName,
            AsAscii("d"), Constants.EndOfHeaderValue,
            Constants.StartOfPayload
        };

        var decodedMessage = _sut.DecodeMessage(inputBytes);

        decodedMessage.Payload.Should().BeEmpty();
    }

    [Test]
    public void DecodeMessage_should_handle_zero_headers()
    {
        var inputBytes = new byte[]
        {
            Constants.StartOfPayload, AsAscii("a")
        };

        var decodedMessage = _sut.DecodeMessage(inputBytes);

        decodedMessage.Headers.Should().BeEmpty();
    }

    [Test]
    public void DecodeMessage_should_throw_on_too_long_header_name()
    {
        var encodedWithTooLongHeaderName = Enumerable.Repeat((byte)42, 1024).ToList();
        encodedWithTooLongHeaderName
            .AddRange(new List<byte>()
                { Constants.EndOfHeaderName, 42, Constants.EndOfHeaderValue, Constants.StartOfPayload, 42 });

        var decodeMessage = () => _sut.DecodeMessage(encodedWithTooLongHeaderName.ToArray());

        decodeMessage.Should().Throw<ArgumentException>();
    }

    [Test]
    public void DecodeMessage_should_handle_message_with_no_headers_or_data()
    {
        var shortestPayload = new byte[] { Constants.StartOfPayload };

        var decodedMessage = _sut.DecodeMessage(shortestPayload);

        decodedMessage.Headers.Should().BeEmpty();
        decodedMessage.Payload.Should().BeEmpty();
    }

    private static byte AsAscii(string s)
    {
        var bytes = Encoding.ASCII.GetBytes(s);
        return bytes[0];
    }
}
