using System;
using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace GrpcServer.Services
{
    public class PrimeNumberCheckService : PrimeNumberService.PrimeNumberServiceBase
    {
        private static ConcurrentDictionary<Int64, Int64> requestedNumbers = new();
        private static int totalMessagesReceived = 0;

        public override Task<PrimeNumberResponse> CheckPrime(PrimeNumber request, ServerCallContext context)
        {
            var number = request.Number;
            bool isPrime = IsPrime(number);
            totalMessagesReceived++;

            if (isPrime)
                requestedNumbers.TryAdd(totalMessagesReceived, number); ;

            return Task.FromResult(new PrimeNumberResponse { IsPrime = isPrime });
        }

        private bool IsPrime(Int64 number)
        {
            if (number <= 1)
                return false;
            if (number <= 3)
                return true;

            for (int i = 2; i <= number/2; i++)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }

        public void DisplayTopValidPrimeNumbers()
        {
            // Count occurrences of numeric values in the dictionary
            var valueCounts = requestedNumbers.Values.GroupBy(value => value)
                                             .ToDictionary(group => group.Key, group => group.Count());

            // Get the top 10 most common values
            var topValues = valueCounts.OrderByDescending(kv => kv.Value)
                                       .ThenByDescending(x => x.Key)
                                       .Take(10)
                                       .Select(x => x.Key)
                                       .ToList();

            Console.WriteLine("Top 10 Highest Requested/Validated Prime Numbers:");
            Console.WriteLine(string.Join(", ", topValues));
        }

        public void GetTotalMessagesReceived()
        {
            Console.WriteLine($"Total Number of messages received: {totalMessagesReceived}");
        }
    }
}