using Grpc.Core;
using GrpcServer.Services;


const int Port = 50051;

try
{
    // Setup the Grpc Server
    Server server = new Server
    {
        Services = { PrimeNumberService.BindService(new PrimeNumberCheckService()) },
        Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
    };
    server.Start();

    PrimeNumberCheckService primeNumberServerService = new();
    // Display top 10 highest requested prime numbers
    while (true)
    {
        primeNumberServerService.DisplayTopValidPrimeNumbers();
        primeNumberServerService.GetTotalMessagesReceived();
        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception encountered: {ex}");
}

