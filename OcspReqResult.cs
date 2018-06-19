using System.Net.Http;
using Org.BouncyCastle.Ocsp;

namespace OcspResponder.Core
{
    /// <summary>
    /// Result of the attempt to retrieve the <see cref="OcspReq"/> from the <see cref="HttpRequestMessage"/>
    /// </summary>
    public class OcspReqResult
    {
        /// <summary>
        /// Status of the request according to <see cref="OcspRespStatus"/>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// The <see cref="OcspReq"/> extracted from the <see cref="HttpRequestMessage"/>
        /// </summary>
        public OcspReq OcspRequest { get; set; }

        /// <summary>
        /// Error message if the <see cref="OcspReq"/> could not be retrieved
        /// </summary>
        public string Error { get; set; }
    }
}
