using System.Security.Cryptography.X509Certificates;

namespace Pge.GasOps.EGen.Cmri.Core.Events.Interfaces
{
    public interface IX509CertificateFactory
    {
        string SubjectName { get;}

        X509Certificate2 GetCertificate();
    }
}
