# OCSPResponder
A .NET facility to create an OCSP Responder

[![Build status](https://ci.appveyor.com/api/projects/status/lhqukqop1eh385wt?svg=true)](https://ci.appveyor.com/project/gabrielcalegari/ocspresponder)
[![NuGet Version](http://img.shields.io/nuget/v/OcspResponder.Core.svg?style=flat)](https://www.nuget.org/packages/OcspResponder.Core)
[![License](https://img.shields.io/badge/license-apache-blue.svg)](http://www.apache.org/licenses/LICENSE-2.0)

## What is this?

OCSPResponder is a library that enables you to easily create an OCSP Responder in .NET. All you need is to implement an interface for the CA/Authorized Responder. It follows the OCSP protocol defined in [RFC 6960](https://tools.ietf.org/html/rfc6960).

## How do I use it?

0. Install [OCSPResponder.Core](http://nuget.org/List/Packages/OSCPResponder.Core) via [NuGet](http://nuget.org).
1. Implement the **IOcspResponderRepository** interface for your CA.
2. Configure in your Dependency Injector to use the class **OcspResponder** for the interface **IOcspResponder**.
3. Configure in your Dependency Injector to use the class that you implemented for the interface **IOcspResponderRepository**.
4. Create a WebAPI Controller or a HttpHandler and call **OcspResponder.Respond()** passing the **HttpRequestMessage**.
5. Done! You've been configured your OCSP Responder.

## Example

**/Controllers/OcspController.cs**

```csharp
[RoutePrefix("api/ocsp")]
public class OcspController : Controller
{
    [Route("{encoded}"]
    public Task<HttpResponseMessage> Get(string encoded)
    {
        return await OcspResponder.Respond(Request);
    }
    
    [Route("")]
    public Task<HttpResponseMessage> Post()
    {
        return await OcspResponder.Respond(Request);
    }
    
    private IOcspResponder OcspResponder { get; }
    
    public OcspController(IOcspResponder ocspResponder)
    {
        OcspResponder = ocspResponder;
    }
}
```

## License

Code by Gabriel Calegari. Copyright 2018 Gabriel Calegari.

This library is intended to be used in both open-source and commercial environments. It is under Apache 2.0 license.

Refer to the [LICENSE](https://github.com/gabrielcalegari/OCSPResponder/blob/master/LICENSE) for detailed information. 

## Any questions, comments or additions?
If you have a feature request or bug report, leave an issue on the [issues page](https://github.com/gabrielcalegari/OCSPResponder/issues) or send a [pull request](https://github.com/gabrielcalegari/OCSPResponder/pulls). For general questions and comments, use the [StackOverflow](https://stackoverflow.com/) forum.
