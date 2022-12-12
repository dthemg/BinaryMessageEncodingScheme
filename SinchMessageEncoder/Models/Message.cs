using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinchMessageEncoder.Models;

public sealed class Message
{
    public Dictionary<string, string> Headers { get; set; } = new();
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
