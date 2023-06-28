using MessageBinaryEncoder;
using MessageBinaryEncoder.Core.MessageEncoder;

//TODO: Implement Global Exception Handler
try
{
    IMessageCodec messageCodec = new MessageCodec();
    var encodedBinaryMessage = messageCodec.Encode(MessageFactory.CreateSampleMessage());
    var output = messageCodec.Decode(encodedBinaryMessage);

    Console.WriteLine($"Number of headers: {output.Headers.Count}");
    Console.WriteLine($"Payload size: {output.Payload.Length}");
}
catch (ArgumentException argumentException)
{
    Console.WriteLine($"Error. Invalid input. {argumentException.Message}");
}
catch (Exception exception)
{
    Console.WriteLine("Error. Something went wrong. Please see the application logs.");
    // TODO: Add logger and configure where logs should be stored   
    using StreamWriter sw = File.AppendText("logs.txt");
    sw.WriteLine(exception.Message);
    sw.WriteLine(exception.InnerException);
}


