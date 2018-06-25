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
            var contentResult = new FileContentResult(OcspHttpResponse.Content, OcspHttpResponse.MediaType);
            await contentResult.ExecuteResultAsync(context);
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
