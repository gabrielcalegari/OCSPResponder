using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OcspResponder.Core;

namespace OcspResponder.AspNetCore
{
    /// <summary>
    /// An Ocsp response
    /// </summary>
    public class OcspActionResult : IActionResult
    {
        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(OcspHttpResponse.Content);
            objectResult.StatusCode = (int)OcspHttpResponse.Status;
            objectResult.ContentTypes.Add(OcspHttpResponse.MediaType);

            await objectResult.ExecuteResultAsync(context);
        }

        /// <summary>
        /// A <see cref="OcspHttpResponse"/> from OcspResponder.Core
        /// </summary>
        private OcspHttpResponse OcspHttpResponse { get; }

        /// <summary>
        /// Creates a <see cref="IActionResult"/> for Ocsp responses
        /// </summary>
        /// <param name="ocspHttpResponse"><see cref="OcspHttpResponse"/></param>
        public OcspActionResult(OcspHttpResponse ocspHttpResponse)
        {
            OcspHttpResponse = ocspHttpResponse;
        }
    }
}
