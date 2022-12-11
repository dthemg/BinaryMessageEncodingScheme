namespace SinchMessageEncoder.Models;

public sealed class Constants
{
    public const byte EndOfHeaderValue = 128;
    public const byte EndOfHeaderName = 129;
    public const byte StartOfPayload = 130;
    public const int MaxHeaderCount = 63;
    public const int MaxHeaderSize = 1023;
    public const int MaxPayloadSize = 256 * 1024;
}
