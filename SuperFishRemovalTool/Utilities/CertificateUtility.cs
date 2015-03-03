using System;

namespace SuperFishRemovalTool.Utilities
{
    internal class CertificateUtility : ISuperfishDetector
    {
        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameCert; } }

        public bool DoesExist()
        {
            bool FoundCertificate = false;

            foreach (var storeValue in Enum.GetValues(typeof(System.Security.Cryptography.X509Certificates.StoreName)))
            {
                // Superfish should be in "Root" or "AuthRoot", but check ALL to be safe
                System.Security.Cryptography.X509Certificates.X509Store store = 
                    new System.Security.Cryptography.X509Certificates.X509Store((System.Security.Cryptography.X509Certificates.StoreName) storeValue);

                store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

                foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 mCert in store.Certificates)
                {
                    if (IsSuperfishCert(mCert))
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish certificate - Store: " + storeValue.ToString());
                        Logging.Logger.Log(Logging.LogSeverity.Information, "  Certificate: " + mCert.Issuer);
                        FoundCertificate = true;
                        break;
                    }
                }
            }

            return FoundCertificate;
        }

        public bool Remove()
        {
            bool FoundSuperfishCert = false;
            bool ProblemDeletingCertificate = false;

            foreach (var storeValue in Enum.GetValues(typeof(System.Security.Cryptography.X509Certificates.StoreName)))
            {
                // Superfish should be in "Root" or "AuthRoot", but check ALL to be safe
                System.Security.Cryptography.X509Certificates.X509Store store =
                    new System.Security.Cryptography.X509Certificates.X509Store((System.Security.Cryptography.X509Certificates.StoreName)storeValue);

                //StorePermission sp = new StorePermission(PermissionState.Unrestricted);
                //sp.Flags = StorePermissionFlags.OpenStore;

                store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.MaxAllowed);

                foreach (System.Security.Cryptography.X509Certificates.X509Certificate2 mCert in store.Certificates)
                {
                    if (IsSuperfishCert(mCert))
                    {
                        FoundSuperfishCert = true;

                        Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish certificate - Store: " + storeValue.ToString());
                        try
                        {
                            Logging.Logger.Log(Logging.LogSeverity.Information, "  DELETING Certificate: " + mCert.Issuer);
                            store.Remove(mCert);                            
                        }
                            catch (Exception ex)
                        {
                            ProblemDeletingCertificate = true;

                            Logging.Logger.Log(ex, "  Exception deleting certificate: " + ex.ToString());
                            //throw;
                        }
                    }
                }
            }

            return (FoundSuperfishCert && (!ProblemDeletingCertificate));
        }

        private bool IsSuperfishCert(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            string Issuer = cert.Issuer;
            string IssuerName = cert.IssuerName.Name;
            
            return ( (Issuer.ToLowerInvariant().Contains("superfish, inc")) || (IssuerName.ToLowerInvariant().Contains("superfish, inc")) );
        }

    }
}
