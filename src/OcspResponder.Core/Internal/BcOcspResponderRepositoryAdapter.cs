using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace OcspResponder.Core.Internal
{
    /// <inheritdoc />
    internal class BcOcspResponderRepositoryAdapter : IBcOcspResponderRepository
    {
        /// <inheritdoc />
        public Task<bool> SerialExists(BigInteger serial, X509Certificate issuerCertificate)
        {
            var dotNetCertificate = new X509Certificate2(issuerCertificate.GetEncoded());
            return OcspResponderRepository.SerialExists(serial.ToString(), dotNetCertificate);
        }

        /// <inheritdoc />
        public Task<CertificateRevocationStatus> SerialIsRevoked(BigInteger serial, X509Certificate issuerCertificate)
        {
            var dotNetCertificate = new X509Certificate2(issuerCertificate.GetEncoded());
            return OcspResponderRepository.SerialIsRevoked(serial.ToString(), dotNetCertificate);
        }

        /// <param name="caCertificate"></param>
        /// <inheritdoc />
        public Task<CaCompromisedStatus> IsCaCompromised(X509Certificate caCertificate)
        {
            var dotNetCertificate = new X509Certificate2(caCertificate.GetEncoded());
            return OcspResponderRepository.IsCaCompromised(dotNetCertificate);
        }

        /// <param name="caCertificate"></param>
        /// <inheritdoc />
        public async Task<AsymmetricKeyParameter> GetResponderPrivateKey(X509Certificate caCertificate)
        {
            var dotNetCertificate = new X509Certificate2(caCertificate.GetEncoded());
            var privateKey = await OcspResponderRepository.GetResponderPrivateKey(dotNetCertificate);
            return DotNetUtilities.GetKeyPair(privateKey).Private;
        }

        /// <param name="caCertificate"></param>
        /// <inheritdoc />
        public async Task<X509Name> GetResponderSubjectDn(X509Certificate caCertificate)
        {
            var dotNetCertificate = new X509Certificate2(caCertificate.GetEncoded());
            var subjectDn = await OcspResponderRepository.GetResponderSubjectDn(dotNetCertificate);
            return new X509Name(subjectDn.Name);
        }

        /// <param name="issuerCertificate"></param>
        /// <inheritdoc />
        public async Task<X509Certificate[]> GetChain(X509Certificate issuerCertificate)
        {
            var dotNetCertificate = new X509Certificate2(issuerCertificate.GetEncoded());
            var certificates = await OcspResponderRepository.GetChain(dotNetCertificate);
            return certificates.Select(DotNetUtilities.FromX509Certificate).ToArray();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<X509Certificate>> GetIssuerCertificates()
        {
            var certificates = await OcspResponderRepository.GetIssuerCertificates();
            return certificates.Select(DotNetUtilities.FromX509Certificate).ToArray();
        }

        /// <inheritdoc />
        public async Task<DateTimeOffset> GetNextUpdate()
        {
            return await OcspResponderRepository.GetNextUpdate();
        }

        /// <see cref="OcspResponderRepository"/>
        private IOcspResponderRepository OcspResponderRepository { get; }

        internal BcOcspResponderRepositoryAdapter(IOcspResponderRepository ocspResponderRepository)
        {
            OcspResponderRepository = ocspResponderRepository;
        }
    }
}
