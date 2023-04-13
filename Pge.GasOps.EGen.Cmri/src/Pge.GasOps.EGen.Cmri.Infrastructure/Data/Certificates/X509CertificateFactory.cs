using System;
using System.Security.Cryptography.X509Certificates;
using Pge.GasOps.EGen.Cmri.Core.Events.Interfaces;
using Serilog;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Data.Certificates
{
    public class X509CertificateFactory : IX509CertificateFactory
    {
        public string SubjectName { get; }
        public string Thumbprint { get; }

        public X509CertificateFactory(string subjectName)
        {
            SubjectName = "CN=" + subjectName + ", OU=people, O=CAISO, C=US";
        }

        public X509CertificateFactory(string subjectName, string thumbprint)
        {
            Thumbprint = thumbprint;
            SubjectName = "CN=" + subjectName + ", OU=people, O=CAISO, C=US";
        }

        public X509Certificate2 GetCertificate()
        {
            var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            X509Certificate2Collection certCollection;

            // search for certificate by Thumbprint first.
            if (!String.IsNullOrEmpty(Thumbprint))
            {
                Log.Information("Searching Local Machine store for certificate with Thumbprint {Thumbprint}", Thumbprint);
                certStore.Open(OpenFlags.ReadOnly);
                certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);
                certStore.Close();
                if (certCollection.Count > 0)
                {
                    X509Certificate2 cert = certCollection[0];
                    Log.Verbose("Found certificate with Thumbprint {Thumbprint} and Subject {Subject}", cert.Thumbprint, cert.Subject);
                    return cert;
                }
                else
                {
                    Log.Error("No certificate with thumbprint {CertThumbprint} found.", Thumbprint);
                }
            }

            if (!String.IsNullOrEmpty(SubjectName))
            {

                Log.Information("Searching Local Machine store for certificates with Subject {CertSubject}",
                    SubjectName);

                certStore.Open(OpenFlags.ReadOnly);
                certCollection = certStore.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, SubjectName, false);
                certStore.Close();
                if (certCollection.Count > 0)
                {
                    foreach (X509Certificate2 cert in certCollection)
                    {
                        Log.Verbose("Found matching certificate {FoundCertThumbprint} - verifying validity period: {CertStart} through {CertEnd}", cert.Thumbprint, cert.NotBefore, cert.NotAfter);

                        if (DateTime.Now >= cert.NotBefore && DateTime.Now <= cert.NotAfter)
                        {
                            Log.Information(
                                "Found valid certificate {FoundCertThumbprint}! Validity period: {CertStart} through {CertEnd}",  cert.Thumbprint, cert.NotBefore, cert.NotAfter);
                            return cert;
                        }
                    }
                }
            }

            Log.Error("No certificates found.");
            
            return null;

        }
    }
}
