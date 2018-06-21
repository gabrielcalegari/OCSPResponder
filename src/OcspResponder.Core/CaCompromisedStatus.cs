using System;

namespace OcspResponder.Core
{
    /// <summary>
    /// Status of compromising of the CA
    /// </summary>
    public class CaCompromisedStatus
    {
        /// <summary>
        /// If the CA is compromised
        /// </summary>
        public bool IsCompromised { get; set; }

        /// <summary>
        /// When it was compromised
        /// </summary>
        public DateTime? CompromisedDate { get; set; }
    }
}
