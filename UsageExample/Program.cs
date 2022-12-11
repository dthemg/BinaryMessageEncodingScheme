using Newtonsoft.Json;
using SinchMessageEncoder.Interfaces;
using SinchMessageEncoder.Models;
using SinchMessageEncoder.Services;

IDecoderService decoder = new DecoderService();
IEncoderService encoder = new EncoderService();

var random = new Random();

var payload = new byte[100];
random.NextBytes(payload);

var message = new Message()
{
    Headers = new()
    {
        { "firstHeader", "headerValue" },
        { "secondHeader", "secondValue" },
        { "thirdHeader", "" }
    },
    Payload = payload
};

var encodedMessage = encoder.EncodeMessage(message);
var decodedMessage = decoder.DecodeMessage(encodedMessage);

Console.WriteLine("Headers \n{");
foreach (var entry in decodedMessage.Headers)
{
    Console.WriteLine($"\t'{entry.Key}':\t '{entry.Value}'");
}
Console.WriteLine("{");

JsonConvert.SerializeObject(decodedMessage.Headers, Formatting.Indented);

Console.WriteLine($"Payload is equal: " +
    Enumerable.SequenceEqual(payload, decodedMessage.Payload));