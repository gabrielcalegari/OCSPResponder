using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;

namespace OcspResponder.Core.Extensions
{
    /// <summary>
    /// Extensions for OcspObjectIdentifiers
    /// </summary>
    public class OcspObjectIdentifierExtensions : OcspObjectIdentifiers
    {
        public static readonly DerObjectIdentifier PkixOscpPrefSigAlgs = new DerObjectIdentifier(PkixOcsp + ".8");

        public static readonly DerObjectIdentifier PkixOcspExtendedRevoke = new DerObjectIdentifier(PkixOcsp + ".9");
    }
}
