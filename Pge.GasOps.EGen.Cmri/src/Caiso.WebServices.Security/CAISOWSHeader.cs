﻿//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Caiso.WebServices.Security
{
    // 
    //This source code was auto-generated by xsd, Version=2.0.50727.42. 
    // 

    ///<remarks/> 
    // [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42"), System.SerializableAttribute(), System.Diagnostics.DebuggerStepThroughAttribute(), System.ComponentModel.DesignerCategoryAttribute("code"), System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/2006-09-30/CAISOWSHeader.xsd"), System.Xml.Serialization.XmlRootAttribute("CAISOWSHeader", Namespace = "http://www.caiso.com/soa/2006-09-30/CAISOWSHeader.xsd", IsNullable = false)]

    [Serializable]
    //[MessageContract(ProtectionLevel = ProtectionLevel.Sign)]
    public class CAISOWSHeader : MessageHeader
    {
        public CAISOWSHeader()
        {
            cAISOUsernameTokenField = new CAISOUsernameTokenType();
            cAISOMessageInfoField = new CAISOMessageInfoType();
        }

        private CAISOUsernameTokenType cAISOUsernameTokenField;

        private CAISOMessageInfoType cAISOMessageInfoField;

        ///<remarks/> 
        public CAISOUsernameTokenType CAISOUsernameToken
        {
            get { return cAISOUsernameTokenField; }
            set { cAISOUsernameTokenField = value; }
        }

        ///<remarks/> 
        public CAISOMessageInfoType CAISOMessageInfo
        {
            get { return cAISOMessageInfoField; }
            set { cAISOMessageInfoField = value; }
        }


        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteRaw(SerializeFields(cAISOUsernameTokenField));
            writer.WriteRaw(SerializeFields(cAISOMessageInfoField));
        }

        public override string Name
        {
            get { return "CAISOWSHeader"; }
        }

        public override string Namespace
        {
            get { return "http://www.caiso.com/soa/2006-09-30/CAISOWSHeader.xsd"; }
        }

        private String SerializeFields(Object objToSerialize)
        {
            string result = null;
            using (StringWriter memStr = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                // remove the <?xml ...?> declaration 
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.Encoding = System.Text.Encoding.UTF8;
                settings.CheckCharacters = true;

                string namespc = "";
                if (Attribute.IsDefined(objToSerialize.GetType(), typeof(XmlRootAttribute)))
                {
                    // Gets the namespace from the XMLRootAttribute so that when we 
                    // can assign an empty prefix for the header namespace to the 
                    // XMLSerializerNamespaces object 
                    XmlRootAttribute xmlRootAttr;
                    xmlRootAttr = (XmlRootAttribute)Attribute.GetCustomAttribute(objToSerialize.GetType(), typeof(XmlRootAttribute));
                    namespc = xmlRootAttr.Namespace;
                }

                // Add this xmlserializernamespaces during serialization to remove the default namespaces. 
                // This also removes the namespace prefix as I am passing an empty string for it. 
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", namespc);
                using (XmlWriter xmlWriter = XmlWriter.Create(memStr, settings))
                {
                    XmlSerializer ser = new XmlSerializer(objToSerialize.GetType());
                    ser.Serialize(xmlWriter, objToSerialize, ns);
                    //byte[] bytes = memStr.ToArray();
                    //result = System.Text.Encoding.UTF8.GetString(bytes).Trim();  
                    result = memStr.ToString();
                }
            }
            return result;
        }

    }

    ///<remarks/> 
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42"), SerializableAttribute(), System.Diagnostics.DebuggerStepThroughAttribute(), System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlRootAttribute("CAISOUsernameToken", IsNullable = false)]
    public partial class CAISOUsernameTokenType
    {

        private string usernameField;

        private string nonceField;

        private DateTime createdField;

        ///<remarks/> 
        public string Username
        {
            get { return usernameField; }
            set { usernameField = value; }
        }

        ///<remarks/> 
        public string Nonce
        {
            get { return nonceField; }
            set { nonceField = value; }
        }

        ///<remarks/> 
        public DateTime Created
        {
            get { return createdField; }
            set { createdField = value; }
        }
    }

    ///<remarks/> 
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42"), SerializableAttribute(), System.Diagnostics.DebuggerStepThroughAttribute(), System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TimestampType
    {

        private DateTime createdField;

        private DateTime expiresField;

        private bool expiresFieldSpecified;

        ///<remarks/> 
        public DateTime Created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        ///<remarks/> 
        public DateTime Expires
        {
            get { return expiresField; }
            set { expiresField = value; }
        }

        ///<remarks/> 
        [XmlIgnoreAttribute()]
        public bool ExpiresSpecified
        {
            get { return expiresFieldSpecified; }
            set { expiresFieldSpecified = value; }
        }
    }

    ///<remarks/> 
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42"), SerializableAttribute(), System.Diagnostics.DebuggerStepThroughAttribute(), System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlRootAttribute("CAISOMessageInfo", IsNullable = false)]
    public partial class CAISOMessageInfoType
    {

        private string messageIDField;

        private TimestampType timestampField;

        ///<remarks/> 
        public string MessageID
        {
            get { return messageIDField; }
            set { messageIDField = value; }
        }

        ///<remarks/> 
        public TimestampType Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }
    }
}

