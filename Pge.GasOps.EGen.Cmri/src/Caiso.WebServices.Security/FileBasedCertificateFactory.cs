//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Caiso.WebServices.Security
{

	/// <summary>
	/// Sample implementation of IX509CertificateFactory.
	/// </summary>
	public class FileBasedCertificateFactory : IX509CertificateFactory
	{

		private string certFile = ConfigurationManager.AppSettings["CertificateFile"];
		private string password = ConfigurationManager.AppSettings["CertificatePassword"];
		
		public FileBasedCertificateFactory() 
		{			
			certFile = ConfigurationManager.AppSettings["CertificateFile"];
			password = ConfigurationManager.AppSettings["CertificatePassword"];
		}

		public FileBasedCertificateFactory(String certFile, String password)
		{
			this.certFile = certFile;
			this.password = password;
		}

		public X509Certificate2 GetCertificate()
		{		
			X509Certificate2 cert = new X509Certificate2();
			cert.Import(ReadFile(certFile), password, X509KeyStorageFlags.DefaultKeySet);
			return cert;
		}

		private byte[] ReadFile(string fileName)
		{
			byte[] data = null;
			using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				int size = Convert.ToInt32(f.Length);
				data = new byte[size + 1];
				size = f.Read(data, 0, size);
			}
			return data;
		}

	}
}
