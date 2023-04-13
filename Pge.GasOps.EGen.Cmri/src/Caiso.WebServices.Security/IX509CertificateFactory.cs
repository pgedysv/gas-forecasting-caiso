//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System.Security.Cryptography.X509Certificates;

namespace Caiso.WebServices.Security
{
    public interface IX509CertificateFactory
    {
        X509Certificate2 GetCertificate();
    }
}