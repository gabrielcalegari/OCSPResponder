using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OcspResponder.Core
{
    /// <summary>
    /// Contract that an OCSP Responder uses to validate a certificate in a CA repository
    /// </summary>
    public interface IOcspResponderRepository
    {
        /// <summary>
        /// Checks whether the serial exists for this CA repository
        /// </summary>
        /// <param name="serial">serial</param>
        /// <param name="issuerCertificate"></param>
        /// <returns><c>true</c> if the serial exists; otherwise, false</returns>
        Task<bool> SerialExists(string serial, X509Certificate2 issuerCertificate);

        /// <summary>
        /// Checks whether the serial is revoked for this CA repository.
        /// </summary>
        /// <param name="serial">serial</param>
        /// <param name="issuerCertificate"></param>
        /// <returns>A <see cref="CertificateRevocationStatus"/> containing whether the certificate is revoked and more info</returns>
        Task<CertificateRevocationStatus> SerialIsRevoked(string serial, X509Certificate2 issuerCertificate);

        /// <summary>
        /// Checks whether the CA is compromised.
        /// </summary>
        /// <param name="caCertificate"></param>
        /// <returns>A <see cref="CaCompromisedStatus"/> containing whether the CA is revoked and when it happens</returns>
        Task<CaCompromisedStatus> IsCaCompromised(X509Certificate2 caCertificate);

        /// <summary>
        /// Gets the private key of the CA or its designated responder
        /// </summary>
        /// <param name="caCertificate"></param>
        /// <returns>A <see cref="AsymmetricAlgorithm"/> that represents the private key of the CA</returns>
        Task<AsymmetricAlgorithm> GetResponderPrivateKey(X509Certificate2 caCertificate);

        /// <summary>
        /// The certificate chain associated with the response signer.
        /// </summary>
        /// <param name="issuerCertificate"></param>
        /// <returns>An array of <see cref="X509Certificate2"/></returns>
        Task<X509Certificate2[]> GetChain(X509Certificate2 issuerCertificate);

        /// <summary>
        /// Gets the date when the client should request the responder about the certificate status
        /// </summary>
        /// <returns>A <see cref="DateTime"/> that represents when the client should request the responder again</returns>
        Task<DateTimeOffset> GetNextUpdate();

        /// <summary>
        /// Gets the issuer certificate that this repository is responsible to evaluate
        /// </summary>
        /// <returns>A <see cref="X509Certificate2"/> that represents the issuer's certificate</returns>
        Task<IEnumerable<X509Certificate2>> GetIssuerCertificates();
    }
}
