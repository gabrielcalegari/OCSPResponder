using System;

namespace OcspResponder.Core
{
    public class OcspHttpRequest
    {
        public string HttpMethod { get; set; }

        public Uri RequestUri { get; set; }

        public string MediaType { get; set; }

        public byte[] Content { get; set; }
    }
}
