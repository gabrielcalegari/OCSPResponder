using System;
using System.Collections.Generic;
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
        /// <param name="issuerCertificate"></param>
        /// <returns><c>true</c> if the serial exists; otherwise, false</returns>
        Task<bool> SerialExists(BigInteger serial, X509Certificate issuerCertificate);

        /// <summary>
        /// Checks whether the serial is revoked for this CA repository.
        /// </summary>
        /// <param name="serial">serial</param>
        /// <param name="issuerCertificate"></param>
        /// <returns>A <see cref="CertificateRevocationStatus"/> containing whether the certificate is revoked and more info</returns>
        Task<CertificateRevocationStatus> SerialIsRevoked(BigInteger serial, X509Certificate issuerCertificate);

        /// <summary>
        /// Checks whether the CA is compromised.
        /// </summary>
        /// <param name="caCertificate"></param>
        /// <returns>A <see cref="CaCompromisedStatus"/> containing whether the CA is revoked and when it happens</returns>
        Task<CaCompromisedStatus> IsCaCompromised(X509Certificate caCertificate);

        /// <summary>
        /// Gets the private key of the CA or its designated responder
        /// </summary>
        /// <param name="caCertificate"></param>
        /// <returns>A <see cref="AsymmetricKeyParameter"/> that represents the private key of the CA</returns>
        Task<AsymmetricKeyParameter> GetResponderPrivateKey(X509Certificate caCertificate);

        /// <summary>
        /// Gets the public key of the CA or its designated responder
        /// </summary>
        /// <param name="caCertificate"></param>
        /// <returns>A <see cref="AsymmetricKeyParameter"/> that represents the public key of the CA</returns>
        Task<AsymmetricKeyParameter> GetResponderPublicKey(X509Certificate caCertificate);

        /// <summary>
        /// The certificate chain associated with the response signer.
        /// </summary>
        /// <param name="issuerCertificate"></param>
        /// <returns>An array of <see cref="Org.BouncyCastle.X509.X509Certificate"/></returns>
        Task<X509Certificate[]> GetChain(X509Certificate issuerCertificate);

        /// <summary>
        /// Gets the issuer certificates that this repository is responsible to evaluate
        /// </summary>
        /// <returns>A enumerable of <see cref="X509Certificate"/> that represents the issuer's certificates</returns>
        Task<IEnumerable<X509Certificate>> GetIssuerCertificates();

        /// <summary>
        /// Gets the date when the client should request the responder about the certificate status
        /// </summary>
        /// <returns>A <see cref="DateTime"/> that represents when the client should request the responder again</returns>
        Task<DateTimeOffset> GetNextUpdate();
    }
}