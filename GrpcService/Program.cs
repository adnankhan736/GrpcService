using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

//Setup channel to communicate with server
var channel = Grpc.Net.Client.GrpcChannel.ForAddress("http://127.0.0.1:50051");
var client = new PrimeNumberService.PrimeNumberServiceClient(channel);



int totalRequests = 1000;
var stopwatch = new Stopwatch();


for (int i = 0; i < totalRequests; i++)
{
    int number = new Random().Next(1, 1001);
    var request = new PrimeNumber { Number = number };
    PrimeNumberResponse response = new PrimeNumberResponse();
    stopwatch.Start();
    try
    {
        response = await client.CheckPrimeAsync(request);
        stopwatch.Stop();
    }
    catch (Exception)
    {
        //Log unsuccessful requests
        Console.WriteLine($"Number: {request?.Id} request was not successful.");
        continue;
    }

    //Log successful requests 
    Console.WriteLine($"Number: {number}, IsPrime: {response.IsPrime}, RTT: {stopwatch.ElapsedMilliseconds} ms");

}


channel.ShutdownAsync().Wait();

app.Run();
