//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;

namespace Caiso.WebServices.Security
{
	public class CaisoMtomEndPointBehavior: IEndpointBehavior
    {
        #region IEndpointBehavior Members
	
        private X509Certificate2 cert;
		public CaisoMtomEndPointBehavior(X509Certificate2 cert)
        {
            this.cert = cert;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
			clientRuntime.MessageInspectors.Add(new CaisoMtomClientMessageInspector(cert));
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
