using System;
using System.Security.Cryptography.X509Certificates;
using Pge.GasOps.Caiso.Core.Events.Interfaces;
using Serilog;

namespace Pge.GasOps.Caiso.Infrastructure.Data.Certificates
{
    public class X509CertificateFactory : IX509CertificateFactory
    {
        public string SubjectName { get; }

        public X509CertificateFactory(string subjectName)
        {
            SubjectName = "CN=" + subjectName + ", OU=people, O=CAISO, C=US";
        }

        public X509Certificate2 GetCertificate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            Log.Information("Searching Local Machine store for certificates with Subject {CertSubject}", SubjectName);

            foreach (X509Certificate2 cert in store.Certificates)
            {
                if (cert.Subject != SubjectName)
                {
                    Log.Verbose("Found certificate {FoundCertThumbprint} with Subject {FoundCertSubject}", cert.Thumbprint, cert.Subject);
                }
                else
                {
                    Log.Verbose("Found matching certificate {FoundCertThumbprint} - verifying validity period: {CertStart} through {CertEnd}", cert.Thumbprint, cert.NotBefore, cert.NotAfter);
                    
                    if (DateTime.Now >= cert.NotBefore && DateTime.Now <= cert.NotAfter)
                    {
                        Log.Information("Found valid certificate {FoundCertThumbprint}! Validity period: {CertStart} through {CertEnd}", cert.Thumbprint, cert.NotBefore, cert.NotAfter);

                        return cert;
                    }
                }
            }

            return null;
        }
    }
}
