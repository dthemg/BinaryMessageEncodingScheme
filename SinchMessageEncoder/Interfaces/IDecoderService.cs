using SinchMessageEncoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinchMessageEncoder.Interfaces;

public interface IDecoderService
{
    public Message DecodeMessage(byte[] encodedMessage);
}
