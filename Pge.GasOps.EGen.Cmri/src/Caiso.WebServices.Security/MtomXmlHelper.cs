//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.IO;
using System.ServiceModel;
using System.Xml;
using System.Xml.XPath;
using Caiso.WebServices.Security.Mime;

namespace Caiso.WebServices.Security
{
	public class MtomXmlHelper
	{

		public static string GetAttachmentHash(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var nav = doc.CreateNavigator();
			nav.MoveToFirstChild();

			var headerXPath = string.Format("//{0}:Envelope/{0}:Header", nav.Prefix);
			XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
			manager.AddNamespace(nav.Prefix, nav.NamespaceURI);
			var headerNode = nav.SelectSingleNode(headerXPath, manager);
			var attachmentNode = headerNode.SelectSingleNode("//*[local-name()='attachmentHash']");
			if (attachmentNode != null)
			{
				var hashValueNode = attachmentNode.SelectSingleNode("//*[local-name()='hashValue']");
				if (hashValueNode != null)
				{
					return hashValueNode.Value;
				}
			}		
			throw new ApplicationException("Attachement Hash was not found in the Soap Header");
		}

		public static string MakeBodyMtomReferencedXml(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var nav = doc.CreateNavigator();
			nav.MoveToFirstChild();

			var bodyXPath = string.Format("//{0}:Envelope/{0}:Body", nav.Prefix);
			XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
			manager.AddNamespace(nav.Prefix, nav.NamespaceURI);
			var bodyNode = nav.SelectSingleNode(bodyXPath, manager);
			var attachmentNode = bodyNode.SelectSingleNode("//*[local-name()='AttachmentInfor']");
			if (attachmentNode != null)
			{
				using (var memStream = new MemoryStream())
				{
					var xmlWriter = XmlDictionaryWriter.CreateTextWriter(memStream);
					ProcessElementForMtomReferencing(attachmentNode, xmlWriter);
					xmlWriter.Flush();
					memStream.Position = 0;
					using (var sr = new StreamReader(memStream))
					{
						xml = sr.ReadToEnd();
					}
				}
			}
			else
			{
				xml = bodyNode.InnerXml;
			}
			return xml;
		}

		private static void ProcessElementForMtomReferencing(XPathNavigator nav, XmlDictionaryWriter xmlWriter)
		{
			do
			{
				if (nav.NodeType == XPathNodeType.Element)
				{
					xmlWriter.WriteStartElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
					if (nav.HasAttributes)
					{
						nav.MoveToFirstAttribute();
						do
						{
							xmlWriter.WriteAttributeString(nav.LocalName, nav.NamespaceURI, nav.Value);
						} while (nav.MoveToNextAttribute());
						nav.MoveToParent();
					}

					if (nav.HasChildren)
					{
						var clonedNav = nav.Clone();
						clonedNav.MoveToFirstChild();
						if (clonedNav.NodeType == XPathNodeType.Element)
						{
							ProcessElementForMtomReferencing(clonedNav, xmlWriter);
						}
						else if (clonedNav.NodeType == XPathNodeType.Text)
						{
							if (string.IsNullOrEmpty(nav.Value))
							{
								xmlWriter.WriteEndElement();
							}
							else
							{
								//<inc:Include href="cid:255629815578" xmlns:inc="http://www.w3.org/2004/08/xop/include"/>
								xmlWriter.WriteStartElement("inc", "Include", "http://www.w3.org/2004/08/xop/include");
								var id = "cid:" + Guid.NewGuid().ToString("N");
								xmlWriter.WriteAttributeString("href", id);
								xmlWriter.WriteEndElement();

								CaisoOperationContext.Current.OutgoingItems.Add(id, nav.Value);
							}
						}
					}
					xmlWriter.WriteFullEndElement();
				}
			} while (nav.MoveToNext());
		}

		public static string PutMtomAttachmentBackToSoapEnvelope(MimeContent Content)
		{
			var xml = Content.Parts[0].Content;
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);
			var nav = xmlDoc.CreateNavigator();
			nav.MoveToFirstChild();
			var bodyXPath = string.Format("//{0}:Envelope/{0}:Body", nav.Prefix);
			XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
			manager.AddNamespace(nav.Prefix, nav.NamespaceURI);
			var bodyNode = nav.SelectSingleNode(bodyXPath, manager);
			bodyNode.MoveToFirstChild();
			ProcessMtomAttachment(bodyNode, Content);
			return xmlDoc.OuterXml;
		}
			
		private static void ProcessMtomAttachment(XPathNavigator nav, MimeContent Content)
		{
			do
			{
				if (nav.NodeType == XPathNodeType.Element)
				{
					if (nav.HasChildren)
					{
						var clonedNav = nav.Clone();
						clonedNav.MoveToFirstChild();
						if (clonedNav.NodeType == XPathNodeType.Element)
						{
							ProcessMtomAttachment(clonedNav, Content);
						}
					}
					else
					{
						if (nav.NamespaceURI == "http://www.w3.org/2004/08/xop/include")
						{
							var href = nav.GetAttribute("href", string.Empty);
							for (int i = 1; i < Content.Parts.Count; i++)
							{
								var contentID = GetCorrectContentID(Content.Parts[i].ContentId);
								if (href == contentID)
								{
									var clonedNav = nav.Clone();
									clonedNav.MoveToParent();
									// If operation context is available, then use it to hold the payload. It is more expensive to push it back to the xml,
									// because the system will need to reparse the xml again with the larger content. Better to pick up the payload from the
									// operation context
									if (OperationContext.Current == null)
									{										
										clonedNav.SetValue(Content.Parts[i].Content);
									}
									else
									{										
										clonedNav.SetValue(string.Empty);// set value to empty string so there is no invalid elements in the xml									
										CaisoOperationContext.Current.IncomingItems.Add(contentID, Convert.FromBase64String(Content.Parts[i].Content));
									}
								}
							}
						}
					}
				}
			} while (nav.MoveToNext());
		}

		private static string GetCorrectContentID(string cid)
		{
			// as per conversation with CAISO, the ContentID will be cid:xxxxxxxxxxxxxx so 
			// I am stripping the enclosing < and > and prepeding cid:
			var result = cid;
			result = result.Substring(1, result.Length - 2);
			return string.Format("cid:{0}", result);
		}

	}
}
