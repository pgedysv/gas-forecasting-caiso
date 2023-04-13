//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;

namespace Caiso.WebServices
{
	public static class CertificateFactory
	{
		private static Dictionary<String, IX509CertificateFactory> index;

	    static CertificateFactory()
		{
			index = new Dictionary<string, IX509CertificateFactory>();
		}

		public static void Register(String key, IX509CertificateFactory factory)
		{
            if (index.ContainsKey(key))
            {
                Log.Debug("A Certificate Factory for \"{key}\" keys has already been registered; skipping registration.", key);
            } else
            {
                index.Add(key, factory);
            }
		}

		public static IX509CertificateFactory Get(String key)
		{
			if (index.ContainsKey(key))
			{
				return index[key];
			}
			return null;
		}

	}
}
