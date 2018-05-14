using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using Polly;
using Shared;
using Swagger;

namespace Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IPetStoreClient, PetStoreClient>(h =>
            {
                // hard timeout doesn't cause retries?!
                h.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(4, (t) => TimeSpan.FromSeconds(t)));

            services.Configure<ClientSettings>(nameof(PetStoreClient), settings =>
            {
                // probably resolve base uri from service discovery...
                settings.BaseUri = new Uri("http://petstore.swagger.io/v2");

                // probably use ITokenProvider to get to user's token or retrieve a client credentials token...
                settings.Credentials = new TokenCredentials("superToken");
            });

            var provider = services.BuildServiceProvider();

            var client = provider.GetRequiredService<IPetStoreClient>();

            var pets = client.FindPetsByStatus(new List<string>() { "available", "pending" });

            Console.WriteLine($"There are {pets.Count} pets.");
        }
    }
}