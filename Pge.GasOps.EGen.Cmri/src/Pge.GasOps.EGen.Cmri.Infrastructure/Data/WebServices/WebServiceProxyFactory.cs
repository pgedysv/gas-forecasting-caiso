// // Copyright (c) 2018 Pacific Gas and Electric Company. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Security;
using System.Xml;
using Pge.GasOps.EGen.Cmri.Infrastructure.Data.Certificates;
using Caiso.WebServices.Security;
using Caiso.WebServices.Security.Security;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Data.WebServices
{
    public class WebServiceProxyFactory<TClient, TInterface>
        where TClient : ClientBase<TInterface>
        where TInterface : class
    {

        public TClient CreateMtomProxy(string url, X509Certificate2 cert, int sendTimeOutSeconds = 120, int receiveTimeOutSeconds = 300)
        {
            var endPoint = CreateEndPointAddress(cert, url);
            var binding = CreateBinding(true, sendTimeOutSeconds, receiveTimeOutSeconds);
            var client = (TClient)Activator.CreateInstance(typeof(TClient), binding, endPoint);
            // set the protection level as CAISO WSDL does not support .NET's protection level and is
            // not automatically set upon creation of the proxy.
            client.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
            //https://docs.microsoft.com/en-us/dotnet/framework/wcf/understanding-protection-level

            client.Endpoint.Behaviors.Add(new CaisoMtomEndPointBehavior(cert));
            client.Endpoint.Contract.Behaviors.Add(new CaisoSoapSignatureContractBehavior());

            if (client.ClientCredentials == null) return client;

            client.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(new Uri(url), cert);
            client.ClientCredentials.ClientCertificate.Certificate = cert;

            return client;
        }

        public TClient CreateMtomProxy(string url, string certKey, int sendTimeOutSeconds, int receiveTimeOutSeconds)
        {
            var factory = CertificateFactory.Get(certKey);
            if (factory == null)
            {
                throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
            }
            var cert = factory.GetCertificate();
            return CreateMtomProxy(url, cert, sendTimeOutSeconds, receiveTimeOutSeconds);
        }

        public TClient CreateMtomProxy(string url, string certKey)
        {
            var factory = CertificateFactory.Get(certKey);
            if (factory == null)
            {
                throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
            }
            var cert = factory.GetCertificate();
            return CreateMtomProxy(url, cert, 120, 300);
        }

        private TClient CreateProxy(string url, X509Certificate2 cert, int sendTimeOutSeconds, int receiveTimeOutSeconds)
        {
            var endPoint = CreateEndPointAddress(cert, url);

            // uncomment if you want to create binding from config and comment the next line.
            //var binding = CreateBindingFromConfig();
            var binding = CreateBinding(false, sendTimeOutSeconds, receiveTimeOutSeconds);

            TClient client = (TClient)Activator.CreateInstance(typeof(TClient), binding, endPoint);
            // set the protection level as CAISO WSDL does not support .NET's protection level and is
            // not automatically set upon creation of the proxy.
            client.Endpoint.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            client.Endpoint.Behaviors.Add(new CaisoEndPointBehavior(cert));
            client.Endpoint.Contract.Behaviors.Add(new CaisoSoapSignatureContractBehavior());

            if (client.ClientCredentials == null) return client;

            client.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(new Uri(url), cert);
            client.ClientCredentials.ClientCertificate.Certificate = cert;

            return client;
        }

        public TClient CreateProxy(string url, string certKey, int sendTimeOutSeconds, int receiveTimeOutSeconds)
        {
            var factory = CertificateFactory.Get(certKey);
            if (factory == null)
            {
                throw new ApplicationException("CertificateFactory is not configured properly. Please register a valid IX509CertificateFactory with the key" + certKey);
            }
            var cert = factory.GetCertificate();
            return CreateProxy(url, cert, sendTimeOutSeconds, receiveTimeOutSeconds);
        }

        public TClient CreateProxy(string url, X509Certificate2 cert)
        {
            return CreateProxy(url, cert, 120, 300);
        }

        /// <summary>
        /// Creates the web service proxy with the service end point and binding properly configured.
        /// </summary>
        /// <param name="url">The url for the web service</param>
        /// <returns></returns>
        public TClient CreateProxy(string url, string certKey)
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
            var uri = new Uri(url);
            return new EndpointAddress(uri, identity);
        }

        /// <summary>
        /// Extracts the certificate's Common Name
        /// </summary>
        /// <param name="certSubject"></param>
        /// <returns></returns>
        private static string ExtractCommonName(string certSubject)
        {
            string[] tokens = certSubject.Split(',');
            return (from token in tokens where token.Contains("CN=") select token.Split('=') into cnTokens select cnTokens[1]).FirstOrDefault();
        }

        internal static CustomBinding CreateBindingFromConfig()
        {
            return new CustomBinding("CAISOWebServiceBinding");
        }

        /// <summary>
        /// Creates the CustomBinding without configuration.
        /// </summary>
        /// <returns></returns>
        private static CustomBinding CreateBinding(bool useMtom, int sendTimeOut, int receiveTimeOut)
        {
            var binding = new CustomBinding {Name = "CAISOWebServiceBinding"};
            var txtMsgEncoding = new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressingAugust2004, Encoding.UTF8);

            var readerQuotas = new XmlDictionaryReaderQuotas
            {
                MaxBytesPerRead = 1024 * 1024 * 5,
                MaxStringContentLength = 2147483647,
                MaxArrayLength = 2147483647
            };
            txtMsgEncoding.ReaderQuotas = readerQuotas;
            if (useMtom)
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

            var securityBinding = new AsymmetricSecurityBindingElement
            {
                MessageSecurityVersion = MessageSecurityVersion
                    .WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10,
                AllowInsecureTransport = false
            };

            var initiatorSecurityToken =
                new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Any)
                {
                    InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient,
                    ReferenceStyle = SecurityTokenReferenceStyle.Internal,
                    RequireDerivedKeys = false
                };

            var recipientSecurityToken =
                new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Any)
                {
                    InclusionMode = SecurityTokenInclusionMode.AlwaysToInitiator,
                    ReferenceStyle = SecurityTokenReferenceStyle.Internal,
                    RequireDerivedKeys = false
                };

            securityBinding.InitiatorTokenParameters = initiatorSecurityToken;
            securityBinding.RecipientTokenParameters = recipientSecurityToken;

            var algorithmSuite = new Basic256SecurityAlgorithmSuite();
            securityBinding.DefaultAlgorithmSuite = algorithmSuite;

            securityBinding.IncludeTimestamp = false;
            securityBinding.EnableUnsecuredResponse = true;
            securityBinding.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
            securityBinding.KeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
            securityBinding.AllowSerializedSigningTokenOnReply = false;
            securityBinding.RequireSignatureConfirmation = false;
            securityBinding.SecurityHeaderLayout = SecurityHeaderLayout.Strict;

            binding.Elements.Add(securityBinding);

            var httpsTransport = new HttpsTransportBindingElement
            {
                MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue, RequireClientCertificate = true
            };

            binding.Elements.Add(httpsTransport);

            return binding;
        }

    }
}
