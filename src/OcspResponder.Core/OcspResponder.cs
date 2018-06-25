using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Common.Logging;
using OcspResponder.Core.Internal;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;

namespace OcspResponder.Core
{
    /// <summary>
    /// Implementation of an OCSP responder as defined in RFC 6960
    /// <remarks>https://tools.ietf.org/html/rfc6960</remarks>
    /// </summary>
    public class OcspResponder : IOcspResponder
    {
        public async Task<OcspHttpResponse> Respond(OcspHttpRequest httpRequest)
        {
            try
            {
                OcspReqResult ocspReqResult = await GetOcspRequest(httpRequest);
                if (ocspReqResult.Status != OcspRespStatus.Successful)
                {
                    Log?.Warn(ocspReqResult.Error);
                    return CreateResponse(OcspResponseGenerator.Generate(ocspReqResult.Status, null).GetEncoded());
                }

                OcspResp ocspResponse = await GetOcspDefinitiveResponse(ocspReqResult.OcspRequest);
                return CreateResponse(ocspResponse.GetEncoded());
            }
            catch (Exception e)
            {
                Log?.Error(e);
                return CreateResponse(OcspResponseGenerator.Generate(OcspRespStatus.InternalError, null).GetEncoded());
            }
        }

        /// <summary>
        /// Gets the <see cref="OcspResp"/> for the <see cref="OcspReq"/>
        /// </summary>
        /// <param name="ocspRequest"></param>
        /// <returns></returns>
        private async Task<OcspResp> GetOcspDefinitiveResponse(OcspReq ocspRequest)
        {
            var basicResponseGenerator = new BasicOcspRespGenerator(new RespID(await OcspResponderRepository.GetResponderSubjectDn()));
            var extensionsGenerator = new X509ExtensionsGenerator();

            foreach (var request in ocspRequest.GetRequestList())
            {
                var certificateId = request.GetCertID();
                var serialNumber = certificateId.SerialNumber;

                CertificateStatus certificateStatus;
                CaCompromisedStatus caCompromisedStatus = await OcspResponderRepository.IsCaCompromised();
                if (caCompromisedStatus.IsCompromised)
                {
                    // See section 2.7 of RFC 6960
                    certificateStatus = new RevokedStatus(caCompromisedStatus.CompromisedDate.Value, (int)RevocationReason.CACompromise);
                }
                else
                {
                    // Se section 2.2 of RFC 6960
                    if (await OcspResponderRepository.SerialExists(serialNumber))
                    {
                        var status = await OcspResponderRepository.SerialIsRevoked(serialNumber);
                        certificateStatus = status.IsRevoked
                            ? new RevokedStatus(status.RevokedInfo.Date, (int)status.RevokedInfo.Reason)
                            :  CertificateStatus.Good;
                    }
                    else
                    {

                        certificateStatus = new RevokedStatus(new DateTime(1970, 1, 1), CrlReason.CertificateHold);
                        extensionsGenerator.AddExtension(OcspObjectIdentifierExtensions.PkixOcspExtendedRevoke, false, (byte[])null);
                    }
                }

                basicResponseGenerator.AddResponse(certificateId, certificateStatus, DateTime.UtcNow, await OcspResponderRepository.GetNextUpdate(), null);
            }

            SetNonceExtension(ocspRequest, extensionsGenerator);

            basicResponseGenerator.SetResponseExtensions(extensionsGenerator.Generate());

            // Algorithm that all clients shall accept as defined in section 4.3 of RFC 6960
            const string signatureAlgorithm = "sha256WithRSAEncryption";
            var basicOcspResponse = basicResponseGenerator.Generate(
                signatureAlgorithm,
                await OcspResponderRepository.GetResponderPrivateKey(),
                await OcspResponderRepository.GetChain(),
                await OcspResponderRepository.GetNextUpdate());

            var ocspResponse = OcspResponseGenerator.Generate(OcspRespStatus.Successful, basicOcspResponse);
            return ocspResponse;
        }

        /// <summary>
        /// Add nonce extension if it exists in the request
        /// </summary>
        /// <param name="ocspRequest">ocsp request</param>
        /// <param name="extensionsGenerator">extensions generator</param>
        private void SetNonceExtension(OcspReq ocspRequest, X509ExtensionsGenerator extensionsGenerator)
        {
            var nonce = ocspRequest.GetExtensionValue(OcspObjectIdentifiers.PkixOcspNonce);
            if (nonce != null)
            {
                extensionsGenerator.AddExtension(OcspObjectIdentifiers.PkixOcspNonce, false, nonce);
            }
        }

        /// <summary>
        /// Retrieves the <see cref="OcspReq"/> from the request
        /// </summary>
        /// <param name="httpRequest"><see cref="OcspHttpRequest"/></param>
        /// <returns><see cref="OcspReqResult"/> containing the <see cref="OcspReq"/></returns>
        private async Task<OcspReqResult> GetOcspRequest(OcspHttpRequest httpRequest)
        {
            // Validates the header of the request
            if (httpRequest.MediaType != "application/ocsp-request")
            {
                return new OcspReqResult
                {
                    Status = OcspRespStatus.MalformedRequest,
                    Error = "OCSP requests requires 'application/ocsp-request' media's type on header"
                };
            }

            // Try to create the ocsp from the http request
            OcspReq ocspRequest;
            try
            {
                ocspRequest = CreateOcspReqFromHttpRequest(httpRequest);
            }
            catch(Exception e)
            {
                return new OcspReqResult
                {
                    Status = OcspRespStatus.MalformedRequest,
                    Error = $"Error when creating OcspReq from the request. Exception: {e.Message}"
                };
            }

            // Validates whether the ocsp request have certificate's requests
            var requests = ocspRequest.GetRequestList();
            if (requests == null || requests.Length == 0)
            {
                return new OcspReqResult
                {
                    Status = OcspRespStatus.MalformedRequest,
                    Error = "Request list is empty"
                };
            }

            // Valitates whether the requests are of this CA's responsibility
            var issuerCert = await OcspResponderRepository.GetIssuerCertificate();
            foreach (var request in ocspRequest.GetRequestList())
            {
                var certificateId = request.GetCertID();
                bool issuerMatches = certificateId.MatchesIssuer(issuerCert);
                if (!issuerMatches)
                {
                    return new OcspReqResult
                    {
                        Status = OcspRespStatus.Unauthorized,
                        Error = "Any certificate is not of this CA's responsibility"
                    };
                }
            }

            // Validation passed so we return the ocspRequest with success status
            return new OcspReqResult
            {
                Status = OcspRespStatus.Successful,
                OcspRequest = ocspRequest,
            };
        }

        /// <summary>
        /// Creates the <see cref="OcspReq"/> from <see cref="OcspHttpRequest"/>
        /// </summary>
        /// <param name="httpRequest"><see cref="OcspHttpRequest"/></param>
        /// <returns><see cref="OcspReq"/></returns>
        private OcspReq CreateOcspReqFromHttpRequest(OcspHttpRequest httpRequest)
        {
            switch (httpRequest.HttpMethod.ToUpper())
            {
                case "GET":
                    return CreateOcspReqFromGet(httpRequest);
                case "POST":
                    return CreateOcspReqFromPost(httpRequest);
                default:
                    throw new HttpRequestException("Only GET and POST methods are allowed");
            }
        }

        /// <summary>
        /// Creates the <see cref="OcspReq"/> from POST
        /// </summary>
        /// <param name="httpRequest"><see cref="OcspHttpRequest"/></param>
        /// <returns><see cref="OcspReq"/></returns>
        private OcspReq CreateOcspReqFromPost(OcspHttpRequest httpRequest)
        {
            return new OcspReq(httpRequest.Content);
        }

        /// <summary>
        /// Creates the <see cref="OcspReq"/> from GET/>
        /// </summary>
        /// <param name="httpRequest"><see cref="OcspHttpRequest"/></param>
        /// <returns><see cref="OcspReq"/></returns>
        private OcspReq CreateOcspReqFromGet(OcspHttpRequest httpRequest)
        {
            string encodedOcspRequest = HttpUtility.UrlDecode(httpRequest.RequestUri.Segments.Last());
            byte[] bytes = Convert.FromBase64String(encodedOcspRequest);
            return new OcspReq(bytes);
        }

        /// <summary>
        /// Creates a <see cref="OcspHttpResponse"/> including the ocsp response as byte array
        /// </summary>
        /// <param name="ocspResponseBytes">ocsp response as byte array</param>
        /// <returns><see cref="OcspHttpResponse"/></returns>
        private OcspHttpResponse CreateResponse(byte[] ocspResponseBytes)
        {
            return new OcspHttpResponse(ocspResponseBytes, "application/ocsp-response", HttpStatusCode.OK);
        }

        private static OCSPRespGenerator OcspResponseGenerator { get; } = new OCSPRespGenerator();

        /// <inheritdoc cref="IBcOcspResponderRepository"/>
        private IBcOcspResponderRepository OcspResponderRepository { get; }

        ///// <inheritdoc cref="ILog"/>
        private ILog Log { get; }

        public OcspResponder(IOcspResponderRepository ocspResponderRepository)
        {
            OcspResponderRepository = new BcOcspResponderRepositoryAdapter(ocspResponderRepository);
            Log = LogManager.GetLogger<OcspResponder>();
        }
    }
}