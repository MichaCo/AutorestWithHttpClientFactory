using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Shared;

namespace Swagger
{
    public partial class PetStoreClient
    {
        public PetStoreClient(IOptionsSnapshot<ClientSettings> options, HttpClient httpClient)
            : base(httpClient, true)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = options.Get(nameof(IPetStoreClient));
            if (settings?.BaseUri == null)
            {
                throw new ArgumentNullException(nameof(settings.BaseUri));
            }
            if (settings?.Credentials == null)
            {
                throw new ArgumentNullException(nameof(settings.Credentials));
            }

            BaseUri = settings.BaseUri;
            Credentials = settings.Credentials;
        }

        // disposeHttpClient can be set to true here, HttpClientFactory sets dispose handle to false to the client will not dispose the HttpClientHandle
        public PetStoreClient(Uri baseUri, HttpClient httpClient, ServiceClientCredentials credentials)
            : base(httpClient, disposeHttpClient: true)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }
    }
}