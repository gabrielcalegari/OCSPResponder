using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;

namespace OcspResponder.Core.Internal
{
    /// <summary>
    /// Extensions for OcspObjectIdentifiers
    /// </summary>
    internal class OcspObjectIdentifierExtensions : OcspObjectIdentifiers
    {
        public static readonly DerObjectIdentifier PkixOscpPrefSigAlgs = new DerObjectIdentifier(PkixOcsp + ".8");

        public static readonly DerObjectIdentifier PkixOcspExtendedRevoke = new DerObjectIdentifier(PkixOcsp + ".9");
    }
}
