using System;
using System.Linq;
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
        public Task<bool> SerialExists(BigInteger serial)
        {
            return OcspResponderRepository.SerialExists(serial.ToString());
        }

        /// <inheritdoc />
        public Task<bool> SerialIsRevoked(BigInteger serial, out RevokedInfo revokedInfo)
        {
            return OcspResponderRepository.SerialIsRevoked(serial.ToString(), out revokedInfo);
        }

        /// <inheritdoc />
        public Task<bool> IsCaCompromised(out DateTime? compromisedDate)
        {
            return OcspResponderRepository.IsCaCompromised(out compromisedDate);
        }

        /// <inheritdoc />
        public async Task<AsymmetricKeyParameter> GetResponderPrivateKey()
        {
            var privateKey = await OcspResponderRepository.GetResponderPrivateKey();
            return DotNetUtilities.GetKeyPair(privateKey).Private;
        }

        /// <inheritdoc />
        public async Task<X509Name> GetResponderSubjectDn()
        {
            var subjectDn = await OcspResponderRepository.GetResponderSubjectDn();
            return new X509Name(subjectDn.Name);
        }

        /// <inheritdoc />
        public async Task<X509Certificate[]> GetChain()
        {
            var certificates = await OcspResponderRepository.GetChain();
            return certificates.Select(DotNetUtilities.FromX509Certificate).ToArray();
        }

        /// <inheritdoc />
        public async Task<X509Certificate> GetIssuerCertificate()
        {
            var certificate = await OcspResponderRepository.GetIssuerCertificate();
            return DotNetUtilities.FromX509Certificate(certificate);
        }

        /// <inheritdoc />
        public async Task<DateTime> GetNextUpdate()
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
