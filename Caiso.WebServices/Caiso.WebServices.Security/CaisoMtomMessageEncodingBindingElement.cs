//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml;

namespace Caiso.WebServices
{
    public class CaisoMtomMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private XmlDictionaryReaderQuotas readerQuotas;

        public CaisoMtomMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement())
        {
            readerQuotas = new XmlDictionaryReaderQuotas();
        }

        public CaisoMtomMessageEncodingBindingElement(MessageEncodingBindingElement innerBindingElement)
        {
            innerBindingElement.MessageVersion = MessageVersion.Soap11;
            if (innerBindingElement == null)
            {
                throw new ArgumentNullException("innerBindingElement", "The inner binding element cannot be null, please specify a valid binding element!");
            }
            else
            {
                InnerBindingElement = innerBindingElement;
            }
        }

        public MessageEncodingBindingElement InnerBindingElement 
        { 
            get; 
            set; 
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return InnerBindingElement.MessageVersion;
            }
            set
            {
                InnerBindingElement.MessageVersion = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return readerQuotas;
            }
        }

        public override BindingElement Clone()
        {
            return new CaisoMtomMessageEncodingBindingElement(InnerBindingElement);
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CaisoMtomMessageEncoderFactory(InnerBindingElement.CreateMessageEncoderFactory());
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentException("context", "Context cannot be null, please pass a BindingContext!");

            context.BindingParameters.Add(this);
            return base.BuildChannelFactory<TChannel>(context);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentException("context", "Context cannot be null, please pass a BindingContext!");

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentException("context", "Context cannot be null, please pass a BindingContext!");

            context.BindingParameters.Add(this);
            return base.BuildChannelListener<TChannel>(context);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentException("context", "Context cannot be null, please pass a BindingContext!");

            context.BindingParameters.Add(this);
            return base.CanBuildChannelListener<TChannel>(context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)this.ReaderQuotas;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}
