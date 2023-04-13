//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;

namespace Caiso.WebServices.Security.Security
{
    public class CaisoEndPointBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members
	
        private X509Certificate2 cert;
        public CaisoEndPointBehavior(X509Certificate2 cert)
        {
            this.cert = cert;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new CaisoClientMessageInspector(cert));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        #endregion
    }
}
