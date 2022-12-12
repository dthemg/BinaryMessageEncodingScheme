using SinchMessageEncoder.Models;

namespace SinchMessageEncoder.Interfaces;

public interface IEncoderService
{
    /// <summary>
    /// Encodes a message into a byte sequence. 
    /// The message has the following constraints to be valid:
    /// 	* The header names and values must be <1024 in length ASCII characters
    /// 	* There can be a maximum of 63 headers
    /// 	* The data payload must be <256 KiB
    /// </summary>
    /// <param name="messageToEncode">The message to encode</param>
    /// <returns>Encoded byte sequence</returns>
    /// <exception cref="ArgumentException">Thrown if the byte sequence do not conform to the specification</exception>
    public byte[] EncodeMessage(Message messageToEncode);
}
