using SinchMessageEncoder.Interfaces;
using SinchMessageEncoder.Models;
using System.Text;

namespace SinchMessageEncoder.Services;

public sealed class DecoderService: IDecoderService
{
    public Message DecodeMessage(byte[] encodedMessage)
    {
        var message = new Message();

        var startOfHeaderIndex = 0;
        var keyValueDelimiterIndex = -1;

        for (var i = 0; i < encodedMessage.Length; i++)
        {
            if (encodedMessage[i] == Constants.EndOfHeaderValue)
            {
                var headerName = ReadHeaderName(ref encodedMessage, startOfHeaderIndex, keyValueDelimiterIndex);
                var headerValue = ReadHeaderValue(ref encodedMessage, keyValueDelimiterIndex, i);

                AddHeader(ref message, headerName, headerValue);
                startOfHeaderIndex = i + 1;
            } else if (encodedMessage[i] == Constants.EndOfHeaderName)
            {
                keyValueDelimiterIndex = i;
            }
            
            if (encodedMessage[i] == Constants.StartOfPayload)
            {
                message.Payload = ReadPayload(ref encodedMessage, i);
                break;
            }
        }

        return message;
    }

    private static string ReadHeaderName(ref byte[] encodedMessage, int startofHeader, int keyValueDelimiter)
    {
        return ReadStringFromMessage(ref encodedMessage, startofHeader, keyValueDelimiter);
    }

    private static string ReadHeaderValue(ref byte[] encodedMessage, int keyValueDelimiter, int stoppingIndex)
    {
        return ReadStringFromMessage(ref encodedMessage, keyValueDelimiter + 1, stoppingIndex);
    }

    private static string ReadStringFromMessage(ref byte[] encodedMessage, int startingIndex, int stoppingIndex)
    {
        if (startingIndex < 0 || stoppingIndex < 0 || stoppingIndex < startingIndex)
        {
            throw new ArgumentException("Message malformed, unable to parse message");
        }

        var headerSize = stoppingIndex - startingIndex;
        if (headerSize > Constants.MaxHeaderSize)
        {
            throw new ArgumentException($"Header size {headerSize} exceeds maximum " +
                $"size of {Constants.MaxHeaderSize}");
        }

         return Encoding.ASCII.GetString(
            encodedMessage,
            startingIndex,
            stoppingIndex - startingIndex);
    }

    private static void AddHeader(ref Message message, string headerName, string headerValue)
    {
        if (string.IsNullOrEmpty(headerName))
        {
            throw new ArgumentException("Header name cannot be null or empty");
        }
        
        if (message.Headers.ContainsKey(headerName))
        {
            throw new ArgumentException($"Duplicate header names encountered for {headerName}");
        }

        if (message.Headers.Count > Constants.MaxHeaderCount)
        {
            throw new ArgumentException($"Maximum number of headers {Constants.MaxHeaderCount} has been exceeded");
        }

        message.Headers.Add(headerName, headerValue);
    }

    private static byte[] ReadPayload(ref byte[] encodedMessage, int startingIndex)
    {
        var payloadSize = encodedMessage.Length - 1 - startingIndex;
        if (payloadSize > Constants.MaxPayloadSize)
        {
            throw new ArgumentException($"Payload size {payloadSize} exceeds maximum " +
                $"size of {Constants.MaxPayloadSize}");
        }

        return encodedMessage
            .Skip(startingIndex + 1)
            .Take(encodedMessage.Length - 1 - startingIndex)
            .ToArray();
    }
}
