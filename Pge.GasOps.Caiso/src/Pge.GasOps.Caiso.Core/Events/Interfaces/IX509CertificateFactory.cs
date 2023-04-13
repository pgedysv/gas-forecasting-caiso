using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Pge.GasOps.Caiso.Core.Events.Interfaces
{
    public interface IX509CertificateFactory
    {
        string SubjectName { get;}

        X509Certificate2 GetCertificate();
    }
}
