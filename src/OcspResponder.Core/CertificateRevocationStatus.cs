namespace OcspResponder.Core
{
    /// <summary>
    /// Status of revocation of a certificate
    /// </summary>
    public class CertificateRevocationStatus
    {
        /// <summary>
        /// It was revoked
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Info about revocation
        /// </summary>
        public RevokedInfo RevokedInfo { get; set; }
    }
}
