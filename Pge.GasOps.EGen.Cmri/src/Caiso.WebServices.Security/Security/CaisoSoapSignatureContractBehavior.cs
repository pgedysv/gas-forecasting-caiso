//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Xml;

namespace Caiso.WebServices.Security.Security
{
    public class CaisoSoapSignatureContractBehavior : Attribute, IContractBehavior
    {

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            ChannelProtectionRequirements requirements = bindingParameters.Find<ChannelProtectionRequirements>();

            XmlQualifiedName wsHeaderQName = new XmlQualifiedName("CAISOWSHeader", "http://www.caiso.com/soa/2006-09-30/CAISOWSHeader.xsd");            
            requirements.IncomingSignatureParts.ChannelParts.HeaderTypes.Add(wsHeaderQName);

            XmlQualifiedName attachmentQName = new XmlQualifiedName("standardAttachmentInfor", "http://www.caiso.com/soa/2006-06-13/StandardAttachmentInfor.xsd");
            requirements.IncomingSignatureParts.ChannelParts.HeaderTypes.Add(attachmentQName);

			XmlQualifiedName attachmentHashQName = new XmlQualifiedName("attachmentHash", "http://www.caiso.com/mrtu/soa/schemas/2005/09/attachmenthash");
			requirements.IncomingSignatureParts.ChannelParts.HeaderTypes.Add(attachmentHashQName); 

            MessagePartSpecification bodyPart = new MessagePartSpecification (true);
            requirements.IncomingSignatureParts.AddParts(bodyPart);
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

    }
}
