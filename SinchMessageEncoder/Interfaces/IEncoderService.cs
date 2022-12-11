using SinchMessageEncoder.Models;

namespace SinchMessageEncoder.Interfaces;

public interface IEncoderService
{
    public byte[] EncodeMessage(Message messageToEncode);
}
