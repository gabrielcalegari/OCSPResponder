using System.Net.Http;
using System.Threading.Tasks;

namespace OcspResponder.Core
{
    public interface IOcspResponder
    {
        Task<HttpResponseMessage> Respond(HttpRequestMessage httpRequest);
    }
}