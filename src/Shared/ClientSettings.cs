using System;
using Microsoft.Rest;

namespace Shared
{
    public class ClientSettings
    {
        public Uri BaseUri { get; set; }

        public ServiceClientCredentials Credentials { get; set; }
    }
}