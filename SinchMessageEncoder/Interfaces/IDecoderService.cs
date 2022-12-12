using SinchMessageEncoder.Models;

namespace SinchMessageEncoder.Interfaces;

public interface IDecoderService
{
    /// <summary>
    /// Decodes a byte sequence into a message which constitutes of a number of 
    /// headers and a binary payload.
    /// The byte sequence has the following constraints to be valid:
    /// 	* The header names and values must be <1024 in length ASCII characters
    /// 	* There can be a maximum of 63 headers
    /// 	* The data payload must be <256 KiB
    /// </summary>
    /// <param name="encodedMessage">Message to decode</param>
    /// <returns><paramref name="encodedMessage"/> decoded into a message of headers and a binary payload</returns>
    /// <exception cref="ArgumentException">Thrown if the byte sequence do not conform to the specification</exception>
    public Message DecodeMessage(byte[] encodedMessage);
}
