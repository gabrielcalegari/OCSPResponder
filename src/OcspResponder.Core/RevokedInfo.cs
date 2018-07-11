using System;

namespace OcspResponder.Core
{
    /// <summary>
    /// Info about revocation
    /// </summary>
    public class RevokedInfo
    {
        /// <summary>
        /// When it was revoked
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Reason for revocation
        /// </summary>
        public RevocationReason Reason { get; set; }
    }
}
