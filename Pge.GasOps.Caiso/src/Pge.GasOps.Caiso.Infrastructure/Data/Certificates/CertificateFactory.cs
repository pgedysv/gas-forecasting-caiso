using System.Collections.Generic;
using Pge.GasOps.Caiso.Core.Events.Interfaces;

namespace Pge.GasOps.Caiso.Infrastructure.Data.Certificates
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
            // Add check for existing key.
            index.Add(key, factory);
        }
    }
}
 