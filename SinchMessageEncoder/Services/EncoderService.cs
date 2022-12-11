using SinchMessageEncoder.Interfaces;
using SinchMessageEncoder.Models;
using System.Text;

namespace SinchMessageEncoder.Services;

public sealed class EncoderService: IEncoderService
{
    private const int MaxHeaderCount = 63;
    private const int MaxHeaderSize = 1023;
    private const int MaxPayloadSize = 256 * 1024;

    public byte[] EncodeMessage(Message messageToEncode)
    {
        ValidateHeaders(messageToEncode);

        var encodedMessage = new List<byte>();
        foreach (var entry in messageToEncode.Headers)
        {
            AppendHeaderKeyToMessage(ref encodedMessage, entry.Key);
            AppendHeaderValueToMessage(ref encodedMessage, entry.Value);
        }

        AppendPayloadToMessage(ref encodedMessage, messageToEncode.Payload);

        return encodedMessage.ToArray();
    }


    private static void ValidateHeaders(Message messageToEncode)
    {
        if (messageToEncode.Headers.Count > MaxHeaderCount)
        {
            throw new ArgumentException($"Number of message headers {messageToEncode.Headers.Count} " +
                $"exceeds the maximum number of headers {MaxHeaderCount}");
        }

        foreach (var headerEntry in messageToEncode.Headers)
        {
            if (Encoding.UTF8.GetByteCount(headerEntry.Key) != headerEntry.Key.Length ||
                Encoding.UTF8.GetByteCount(headerEntry.Value) != headerEntry.Value.Length)
            {
                throw new ArgumentException("Header keys and values must contain ASCII-only characters");
            }
        }
    }

    private static void AppendHeaderKeyToMessage(ref List<byte> encodedMessage, string headerNameToAppend)
    {
        if (string.IsNullOrEmpty(headerNameToAppend))
        {
            throw new ArgumentException("Header key is empty");
        }

        AddStringToMessage(ref encodedMessage, headerNameToAppend);
        encodedMessage.Add(Constants.EndOfHeaderName);
    }

    private static void AppendHeaderValueToMessage(ref List<byte> encodedMessage, string headerValueToAppend)
    {
        AddStringToMessage(ref encodedMessage, headerValueToAppend);
        encodedMessage.Add(Constants.EndOfHeaderValue);
    }

    private static void AppendPayloadToMessage(ref List<byte> encodedMessage, byte[] payload)
    {
        if (payload.Length > MaxPayloadSize)
        {
            throw new ArgumentException($"Payload too large; size {payload.Length} " +
                $"exceeds maximum size {MaxPayloadSize}");
        }

        encodedMessage.Add(Constants.StartOfPayload);
        encodedMessage.AddRange(payload);
    }

    private static void AddStringToMessage(ref List<byte> encodedMessage, string headerToAppend)
    {
        var encodedHeader = Encoding.ASCII.GetBytes(headerToAppend);
        if (encodedHeader.Length > MaxHeaderSize)
        {
            throw new ArgumentException($"Message header key has size {encodedHeader.Length}, " +
                $"exceeds maximum size {MaxHeaderSize}");
        }
        encodedMessage.AddRange(encodedHeader);
    }
}
