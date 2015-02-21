using System;

namespace SuperFishRemovalTool.Utilities
{
    internal class CertificateUtility : ISuperfishDetector
    {
        public FixResult RemoveItem()
        {
            var result = new FixResult(Localizer.Get().DetectorNameCert);
            try
            {
                result.DidExist = this.DoesExist();
                result.WasRemoved = this.Remove();
            }
            catch(Exception ex)
            {
                result.DidFail = true;
            }
            return result;
        }

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
                        Console.WriteLine("Found Superfish certificate - Store: " + storeValue.ToString());
                        Console.WriteLine("  Certificate: " + mCert.Issuer);
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

                        Console.WriteLine("Found Superfish certificate - Store: " + storeValue.ToString());
                        try
                        {
                            Console.WriteLine("  DELETING Certificate: " + mCert.Issuer);
                            store.Remove(mCert);                            
                        }
                            catch (Exception ex)
                        {
                            ProblemDeletingCertificate = true;

                            Console.WriteLine("  Exception deleting certificate: " + ex.ToString());
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

        private void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[32768];

            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);

                if (read <= 0)
                {
                    return;
                }

                output.Write(buffer, 0, read);
            }
        }

    }
}
