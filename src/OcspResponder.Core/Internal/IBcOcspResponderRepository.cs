using System;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace OcspResponder.Core.Internal
{
    /// <summary>
    /// Adapter from <see cref="IOcspResponderRepository"/> for BouncyCastle's library
    /// </summary>
    internal interface IBcOcspResponderRepository
    {
        /// <summary>
        /// Checks whether the serial exists for this CA repository
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns><c>true</c> if the serial exists; otherwise, false</returns>
        Task<bool> SerialExists(BigInteger serial);

        /// <summary>
        /// Checks whether the serial is revoked for this CA repository.
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns>A <see cref="CertificateRevocationStatus"/> containing whether the certificate is revoked and more info</returns>
        Task<CertificateRevocationStatus> SerialIsRevoked(BigInteger serial);

        /// <summary>
        /// Checks whether the CA is compromised.
        /// </summary>
        /// <returns>A <see cref="CaCompromisedStatus"/> containing whether the CA is revoked and when it happens</returns>
        Task<CaCompromisedStatus> IsCaCompromised();

        /// <summary>
        /// Gets the private key of the CA or its designated responder
        /// </summary>
        /// <returns>A <see cref="AsymmetricKeyParameter"/> that represents the private key of the CA</returns>
        Task<AsymmetricKeyParameter> GetResponderPrivateKey();

        /// <summary>
        /// Gets the subject of the CA or its designated responder
        /// </summary>
        /// <returns>A <see cref="X509Name"/> that represents the subject of the CA</returns>
        Task<X509Name> GetResponderSubjectDn();

        /// <summary>
        /// The certificate chain associated with the response signer.
        /// </summary>
        /// <returns>An array of <see cref="Org.BouncyCastle.X509.X509Certificate"/></returns>
        Task<X509Certificate[]> GetChain();

        /// <summary>
        /// Gets the issuer certificate that this repository is responsible to evaluate
        /// </summary>
        /// <returns>A <see cref="X509Certificate"/> that represents the issuer's certificate</returns>
        Task<X509Certificate> GetIssuerCertificate();

        /// <summary>
        /// Gets the date when the client should request the responder about the certificate status
        /// </summary>
        /// <returns>A <see cref="DateTime"/> that represents when the client should request the responder again</returns>
        Task<DateTime> GetNextUpdate();
    }
}