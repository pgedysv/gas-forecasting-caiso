﻿//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.XPath;
using System.Security.Cryptography;

namespace Caiso.WebServices.Security
{
	public class CaisoMtomClientMessageInspector : CaisoClientMessageInspector
	{

		public CaisoMtomClientMessageInspector(X509Certificate2 cert)
			: base(cert)
		{
		}

		public override void AfterReceiveReply(ref Message reply, object correlationState)
		{
			base.AfterReceiveReply(ref reply, correlationState);
		}

		public override object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var xmlBody = MtomXmlHelper.MakeBodyMtomReferencedXml(request.ToString());
			var xmlReader = XmlReader.Create(new StringReader(xmlBody));
			var msg = Message.CreateMessage(request.Version, "", xmlReader);
			msg.Headers.Clear();
			msg.Headers.CopyHeadersFrom(request.Headers);
			base.BeforeSendRequest(ref msg, channel); // this will attach the CAISOWSHeader

			if (OperationContext.Current != null && CaisoOperationContext.Current.OutgoingItems.Count > 0)
			{
				var msgProps = CaisoOperationContext.Current.OutgoingItems;
				var attachBytes = UTF8Encoding.UTF8.GetBytes(msgProps.Values.Single());
				var hash = HashHelper.Sha1Hash(attachBytes);
				// now add the attachmentHash into the header
				var attachHash = new attachmentHashType();
				attachHash.hashValue = hash;
				msg.Headers.Add(attachHash);
			}
			request = msg;
			return null;
		}
	}
}
