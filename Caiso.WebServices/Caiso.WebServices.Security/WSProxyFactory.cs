//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Security;
using System.Xml;
using XXXXXFOOBARRRRRR = Caiso.WebServices.Security;

namespace Caiso.WebServices
{
	public class WSProxyFactory<TClient, TInterface>
		where TClient : ClientBase<TInterface>
		where TInterface : class
	{

		public TClient CreateMTOMProxy(String url, X509Certificate2 cert, int sendTimeOutSeconds, int receiveTimeOutSeconds)
		{
			var endPoint = CreateEndPointAddress(cert, url);

			var binding = CreateBinding(true,  sendTimeOutSeconds, receiveTimeOutSeconds);

			TClient client = (TClient)Activator.CreateInstance(typeof(TClient), binding, endPoint);
			// set the protection level as CAISO WSDL does not support .NET's protection level and is
			// not automatically set upon creation of the proxy.
			client.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
            //https://docs.microsoft.com/en-us/dotnet/framework/wcf/understanding-protection-level

            client.Endpoint.Behaviors.Add(new XXXXXFOOBARRRRRR.CaisoMtomEndPointBehavior(cert));
			client.Endpoint.Contract.Behaviors.Add(new XXXXXFOOBARRRRRR.CaisoSoapSignatureContractBehavior());

			client.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(new Uri(url), cert);
			client.ClientCredentials.ClientCertificate.Certificate = cert;

			return client;
		}

		public TClient CreateMTOMProxy(String url, string certKey, int sendTimeOutSeconds, int receiveTimeOutSeconds)
		{
			var factory = CertificateFactory.Get(certKey);
			if (factory == null)
			{
				throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
			}
			var cert = factory.GetCertificate();
			return CreateMTOMProxy(url, cert, sendTimeOutSeconds, receiveTimeOutSeconds);
		}

		public TClient CreateMTOMProxy(String url, String certKey)
		{
			var factory = CertificateFactory.Get(certKey);
			if (factory == null)
			{
				throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
			}
			var cert = factory.GetCertificate();
			return CreateMTOMProxy(url, cert, 120, 300);
		}

		public TClient CreateMTOMProxy(String url, X509Certificate2 cert)
		{
			return CreateMTOMProxy(url, cert, 120, 300);
		}

		public TClient CreateProxy(String url, X509Certificate2 cert, int sendTimeOutSeconds, int receiveTimeOutSeconds)
		{
			var endPoint = CreateEndPointAddress(cert, url);

			// uncomment if you want to create binding from config and comment the next line.
			//var binding = CreateBindingFromConfig();
			var binding = CreateBinding(false, sendTimeOutSeconds, receiveTimeOutSeconds);

			TClient client = (TClient)Activator.CreateInstance(typeof(TClient), binding, endPoint);
			// set the protection level as CAISO WSDL does not support .NET's protection level and is
			// not automatically set upon creation of the proxy.
			client.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

			client.Endpoint.Behaviors.Add(new XXXXXFOOBARRRRRR.CaisoEndPointBehavior(cert));
			client.Endpoint.Contract.Behaviors.Add(new XXXXXFOOBARRRRRR.CaisoSoapSignatureContractBehavior());

			client.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(new Uri(url), cert);
			client.ClientCredentials.ClientCertificate.Certificate = cert;

			return client;
		}

		public TClient CreateProxy(String url, string certKey, int sendTimeOutSeconds, int receiveTimeOutSeconds)
		{
			var factory = CertificateFactory.Get(certKey);
			if (factory == null)
			{
				throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
			}
			var cert = factory.GetCertificate();
			return CreateProxy(url, cert, sendTimeOutSeconds, receiveTimeOutSeconds);
		}

		public TClient CreateProxy(String url, X509Certificate2 cert)
		{
			return CreateProxy(url, cert, 120, 300);
		}

		/// <summary>
		/// Creates the web service proxy with the service end point and binding properly configured.
		/// </summary>
		/// <param name="url">The url for the web service</param>
		/// <returns></returns>
		public TClient CreateProxy(String url, String certKey)
		{
			var factory = CertificateFactory.Get(certKey);
			if (factory == null)
			{
				throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
			}
			var cert = factory.GetCertificate();
			return CreateProxy(url, cert, 120, 300);
		}

		/// <summary>
		/// Creates the Service Endpoint without configuration. Adds the necessary end point identity
		/// by using the certificate's "Common Name" in the subject.
		/// </summary>
		/// <param name="cert"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		private static EndpointAddress CreateEndPointAddress(X509Certificate2 cert, string url)
		{
			string dns = ExtractCommonName(cert.Subject);
			EndpointIdentity identity = EndpointIdentity.CreateDnsIdentity(dns);
			Uri uri = new Uri(url);
			return new EndpointAddress(uri, identity);
		}

		/// <summary>
		/// Extracts the certificate's Common Name
		/// </summary>
		/// <param name="CertSubject"></param>
		/// <returns></returns>
		private static string ExtractCommonName(string CertSubject)
		{
			string commonName = null;
			string[] tokens = CertSubject.Split(',');
			foreach (string token in tokens)
			{
				if (token.Contains("CN="))
				{
					string[] cnTokens = token.Split('=');
					commonName = cnTokens[1];
					break;
				}
			}
			return commonName;
		}

		internal static CustomBinding CreateBindingFromConfig()
		{
			return new CustomBinding("CAISOWSBinding");
		}

		/// <summary>
		/// Creates the CustomBinding without configuration.
		/// </summary>
		/// <returns></returns>
		private static CustomBinding CreateBinding(bool useMTOM,  int sendTimeOut, int receiveTimeOut)
		{
			var binding = new CustomBinding();
			binding.Name = "CAISOWSBinding";
			var txtMsgEncoding = new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressingAugust2004, Encoding.UTF8);

			XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
			readerQuotas.MaxBytesPerRead = 1024 * 1024 * 5;
			readerQuotas.MaxStringContentLength = 2147483647;
			readerQuotas.MaxArrayLength = 2147483647;
			txtMsgEncoding.ReaderQuotas = readerQuotas;
			if (useMTOM)
			{
				var mtomMsgEncoding = new CaisoMtomMessageEncodingBindingElement(txtMsgEncoding);
				binding.Elements.Add(mtomMsgEncoding);
			}
			else
			{
				binding.Elements.Add(txtMsgEncoding);
			}
			binding.OpenTimeout = new TimeSpan(0, 0, 120);
			binding.CloseTimeout = new TimeSpan(0, 0, 120);
			binding.ReceiveTimeout = new TimeSpan(0, 0, receiveTimeOut);
			binding.SendTimeout = new TimeSpan(0, 0, sendTimeOut);

			var securityBinding = new AsymmetricSecurityBindingElement();
			securityBinding.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
			securityBinding.AllowInsecureTransport = false;

			X509SecurityTokenParameters initiatorSecurityToken = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Any);
			initiatorSecurityToken.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
			initiatorSecurityToken.ReferenceStyle = SecurityTokenReferenceStyle.Internal;
			initiatorSecurityToken.RequireDerivedKeys = false;

			X509SecurityTokenParameters recipientSecurityToken = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Any);
			recipientSecurityToken.InclusionMode = SecurityTokenInclusionMode.AlwaysToInitiator;
			recipientSecurityToken.ReferenceStyle = SecurityTokenReferenceStyle.Internal;
			recipientSecurityToken.RequireDerivedKeys = false;

			securityBinding.InitiatorTokenParameters = initiatorSecurityToken;
			securityBinding.RecipientTokenParameters = recipientSecurityToken;

			var algorithmSuite = new Basic256SecurityAlgorithmSuite();
			securityBinding.DefaultAlgorithmSuite = algorithmSuite;

			securityBinding.IncludeTimestamp = false;
			securityBinding.EnableUnsecuredResponse = true;
			securityBinding.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
			securityBinding.KeyEntropyMode = System.ServiceModel.Security.SecurityKeyEntropyMode.CombinedEntropy;
			securityBinding.AllowSerializedSigningTokenOnReply = false;
			securityBinding.RequireSignatureConfirmation = false;
			securityBinding.SecurityHeaderLayout = SecurityHeaderLayout.Strict;

			binding.Elements.Add(securityBinding);

			var httpsTransport = new HttpsTransportBindingElement();
			httpsTransport.MaxReceivedMessageSize = int.MaxValue;
			httpsTransport.MaxBufferSize = int.MaxValue;
			httpsTransport.RequireClientCertificate = true;

			binding.Elements.Add(httpsTransport);

			return binding;
		}
		 
	}
}
