# Testing Autorest with HttpClientFactory

In this solution I'm trying to integrate `IHttpClientFactory` from `Microsoft.Extensions.Http` with auto generated autorest clients.

### Main problems are:
* Initialization of the generated client: 
  The generated client doesn't come with overloads which accept an `HttpClient` (only protected ctor)
* In addition, we have to pass in the base uri and probably credentials

Needs testing:
* Do poly policies actually work with the generated client? Should work on the httpclient level, but how does it behave with the generated client code/custom error handling...?

### Current Solution:
* Created named http client
* Create named options to set base URI and credentials and probably other things
* Create a custom partial client class for a custom ctor to get the `HttpClient` and named options injected

### Optimal Solution:

This is a lot of custom code one has to write for each client. 
It would be much better if I'd be able to inject the client with the generated constructors in a simple way.

HttpClientFactory doesn't provide a way to get a `HttpClientHandler` though, and generated autorest clients do not take `HttpClient`.
Not sure what other options there are...?!

