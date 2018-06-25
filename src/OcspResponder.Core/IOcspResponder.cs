using System.Threading.Tasks;

namespace OcspResponder.Core
{
    public interface IOcspResponder
    {
        Task<OcspHttpResponse> Respond(OcspHttpRequest httpRequest);
    }
}