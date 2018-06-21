using System;
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
        /// <returns><c>true</c> if the serial exists; otherwise, false</returns>
        Task<bool> SerialExists(string serial);

        /// <summary>
        /// Checks whether the serial is revoked for this CA repository.
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns>A <see cref="CertificateRevocationStatus"/> containing whether the certificate is revoked and more info</returns>
        Task<CertificateRevocationStatus> SerialIsRevoked(string serial);

        /// <summary>
        /// Checks whether the CA is compromised.
        /// </summary>
        /// <returns>A <see cref="CaCompromisedStatus"/> containing whether the CA is revoked and when it happens</returns>
        Task<CaCompromisedStatus> IsCaCompromised();

        /// <summary>
        /// Gets the private key of the CA or its designated responder
        /// </summary>
        /// <returns>A <see cref="AsymmetricAlgorithm"/> that represents the private key of the CA</returns>
        Task<AsymmetricAlgorithm> GetResponderPrivateKey();

        /// <summary>
        /// Gets the subject of the CA or its designated responder
        /// </summary>
        /// <returns>A <see cref="X500DistinguishedName"/> that represents the subject of the CA</returns>
        Task<X500DistinguishedName> GetResponderSubjectDn();

        /// <summary>
        /// The certificate chain associated with the response signer.
        /// </summary>
        /// <returns>An array of <see cref="X509Certificate2"/></returns>
        Task<X509Certificate2[]> GetChain();

        /// <summary>
        /// Gets the date when the client should request the responder about the certificate status
        /// </summary>
        /// <returns>A <see cref="DateTime"/> that represents when the client should request the responder again</returns>
        Task<DateTime> GetNextUpdate();

        /// <summary>
        /// Gets the issuer certificate that this repository is responsible to evaluate
        /// </summary>
        /// <returns>A <see cref="X509Certificate2"/> that represents the issuer's certificate</returns>
        Task<X509Certificate2> GetIssuerCertificate();
    }
}
