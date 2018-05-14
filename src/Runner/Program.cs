using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using Polly;
using Swagger;

namespace Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IPetStoreClient, PetStoreClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(4, (t) => TimeSpan.FromSeconds(t)));

            //services.Configure<ClientSettings>(nameof(IPetStoreClient), settings =>
            //{
            //    // probably resolve base uri from service discovery...
            //    // doesn't work well inside the configure factory because we don't have access to IServiceProvider...
            //    settings.BaseUri = new Uri("http://petstore.swagger.io/v2");

            //    // probably use ITokenProvider to get to user's token or retrieve a client credentials token...
            //    // same here, how do we resolve a ITokenProvider for this client?
            //    settings.Credentials = new TokenCredentials("superToken");
            //});

            //var provider = services.BuildServiceProvider();

            //var client = provider.GetRequiredService<IPetStoreClient>();

            //var pets = client.FindPetsByStatus(new List<string>() { "available", "pending" });

            //Console.WriteLine($"There are {pets.Count} pets.");

            /*** more real implementation: ***/

            // no implementation here, just dummy here...
            services.AddSingleton<IServiceDiscovery, ServiceDiscovery>();

            // add token providers so it can get dependencies from DI, too
            services.AddScoped<UserAccessTokenProvider>();
            services.AddScoped<ClientCredentialsTokenProvider>();

            services.AddScoped<IPetStoreClient, PetStoreClient>(p =>
            {
                // create a named/configured HttpClient
                var httpClient = p.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(IPetStoreClient));

                // in this case user initiated flow is used
                var tokenProvider = p.GetRequiredService<UserAccessTokenProvider>();

                // get the base Uri from service disco (service name could come from configuration again...)
                var baseUri = p.GetRequiredService<IServiceDiscovery>().GetServiceBaseUri("petStore");

                return new PetStoreClient(baseUri, httpClient, new TokenCredentials(tokenProvider));
            });

            var provider = services.BuildServiceProvider();

            // will throw as we are not in ASP.NET and no IHttpContextAccessor has been injected
            var client = provider.GetRequiredService<IPetStoreClient>();
        }
    }

    public class UserAccessTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserAccessTokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            // getting the token from current http context. Doesn't work if no user is signed in of course
            var token = await _contextAccessor.HttpContext?.GetTokenAsync("acccess_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Could not get an access token from HttpContext.");
            }

            return new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public class ClientCredentialsTokenProvider : ITokenProvider
    {
        public Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            // do things to get a token from your authority for client secrets...
            throw new NotImplementedException();
        }
    }

    // dummy code here
    public interface IServiceDiscovery
    {
        Uri GetServiceBaseUri(string name);
    }

    public class ServiceDiscovery : IServiceDiscovery
    {
        public Uri GetServiceBaseUri(string name)
        {
            return new Uri("http://petstore.swagger.io/v2");
        }
    }
}