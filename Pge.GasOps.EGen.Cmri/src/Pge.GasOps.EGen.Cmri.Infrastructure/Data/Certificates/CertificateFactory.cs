using System.Collections.Generic;
using Pge.GasOps.EGen.Cmri.Core.Events.Interfaces;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Data.Certificates
{
    public static class CertificateFactory
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Dictionary<string, IX509CertificateFactory> index;

        static CertificateFactory()
        {
            index = new Dictionary<string, IX509CertificateFactory>();
        }

        public static IX509CertificateFactory Get(string key)
        {
            return index.ContainsKey(key) ? index[key] : null;
        }

        public static void Register(string key, IX509CertificateFactory factory)
        {
            if (index.ContainsKey(key))
            {
                //Log.Debug("A Certificate Factory for \"{key}\" keys has already been registered; skipping registration.", key);
            }
            else
            {
                index.Add(key, factory);
            }
        }
    }
}
 