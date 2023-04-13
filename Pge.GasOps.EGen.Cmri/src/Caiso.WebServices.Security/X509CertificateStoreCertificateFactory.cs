//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Security.Cryptography.X509Certificates;
using Serilog;

namespace Caiso.WebServices.Security
{
    public class X509CertificateStoreCertificateFactory : IX509CertificateFactory
    {
        private readonly string subjectName;

        public X509CertificateStoreCertificateFactory(string subjectName)
        {
            this.subjectName = subjectName;
        }

        public X509Certificate2 GetCertificate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            foreach (X509Certificate2 cert in store.Certificates)
            {
                Log.Verbose(
                    "Found {FoundCertThumbprint} in Current User - verifying Subject match: {TargetCertSubject} <> {FoundCertSubject}",
                    cert.Thumbprint,
                    subjectName,
                    cert.Subject);

                if (cert.Subject == subjectName)
                {
                    Log.Verbose(
                        "Found matching certificate {FoundCertThumbprint} - verifying validity period: {CertStart} through {CertEnd}",
                        cert.Thumbprint,
                        cert.NotBefore,
                        cert.NotAfter);
                    if (DateTime.Now >= cert.NotBefore && DateTime.Now <= cert.NotAfter)
                    {
                        return cert;
                    }
                }
            }

            return null;
        }
    }
}