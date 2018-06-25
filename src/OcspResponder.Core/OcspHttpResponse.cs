using System.Net;

namespace OcspResponder.Core
{
    public class OcspHttpResponse
    {
        public byte[] Content { get; }

        public string MediaType { get; }

        public HttpStatusCode Status { get; }

        public OcspHttpResponse(byte[] content, string mediaType, HttpStatusCode status)
        {
            Content = content;
            MediaType = mediaType;
            Status = status;
        }
    }
}