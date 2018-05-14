using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Shared;

namespace Swagger
{
    public partial class PetStoreClient
    {
        public PetStoreClient(IOptionsSnapshot<ClientSettings> options, HttpClient httpClient) : base(httpClient, false)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var settings = options.Get(nameof(PetStoreClient));
            if (settings?.BaseUri == null)
            {
                throw new ArgumentNullException("baseUri");
            }
            if (settings?.Credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }

            BaseUri = settings.BaseUri;
            Credentials = settings.Credentials;
        }
    }
}