# OCSPResponder
A .NET facility to create an OCSP Responder

## What is this?

OCSPResponder is a library that enables you to easily create an OCSP Responder in .NET. All you need is to implement an interface for the CA/Authorized Responder. It follows the OCSP protocol defined in [RFC 6960](https://tools.ietf.org/html/rfc6960).

## How do I use it?

0. Install [OCSPResponder.Core](http://nuget.org/List/Packages/OSCPResponder.Core) via [NuGet](http://nuget.org).
1. Implement the **IOcspResponderRepository** interface for your CA.
2. Create a WebAPI Controller or a HttpHandler and call **OcspResponder.Respond()** passing the **HttpRequestMessage**.
3. Done! You've been configured your OCSP Responder.

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
