using System;
using System.Collections.Generic;
using System.Text;

namespace Pge.GasOps.Caiso.Core.Events.Interfaces
{
    public interface ICertificateFactory
    {
        void Register(String key, IX509CertificateFactory factory);
    }
}
