namespace OcspResponder.Core
{
    /// <summary>
    /// Reasons for revocation
    /// </summary>
    public enum RevocationReason
    {
        Unspecified = 0,
        KeyCompromise = 1,
        CACompromise = 2,
        AffiliationChanged = 3,
        Superseded = 4,
        CessationOfOperation = 5,
        CertificateHold = 6,
        RemoveFromCrl = 8,
        PrivilegeWithdrawn = 9,
        AACompromise = 10
    }
}
