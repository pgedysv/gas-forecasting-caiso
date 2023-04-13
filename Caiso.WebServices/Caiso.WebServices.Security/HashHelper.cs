//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Caiso.WebServices
{
	public class HashHelper
	{
		public static string Sha1Hash(byte[] bytesToHash)
		{
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(bytesToHash);
				var hashStr = Convert.ToBase64String(hash);
				return hashStr;
			}
		}	

	}
}
