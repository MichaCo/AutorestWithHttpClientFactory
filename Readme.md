# Testing Autorest with HttpClientFactory

See http://michaco.net/blog/IntegratingAutorestWithHttpClientFactoryAndDI

In this solution I'm trying to integrate `IHttpClientFactory` from `Microsoft.Extensions.Http` with auto generated autorest clients.

### Main problems are:
* Initialization of the generated client: 
  The generated client doesn't come with overloads which accept an `HttpClient` (only protected ctor)
* In addition, we have to pass in the base uri and probably credentials

### Possible Solutions:
* Create a custom partial client class for a custom ctor to set the `HttpClient`
* Change autorest runtime library to work with `IHttpClientFactory` => bigger change and potentially too many dependencies added
* Change `IHttpClientFactory` to expose/create `HttpClientHandler`s => would be the optiomal solution.


