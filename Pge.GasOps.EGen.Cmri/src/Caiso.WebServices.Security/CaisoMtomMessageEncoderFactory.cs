//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.ServiceModel.Channels;

namespace Caiso.WebServices.Security
{
    public class CaisoMtomMessageEncoderFactory : MessageEncoderFactory
    {
        protected readonly CaisoMtomMessageEncoder encoder;

        public override MessageEncoder Encoder
        {
            get { return encoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return encoder.MessageVersion; }
        }

        public CaisoMtomMessageEncoderFactory(MessageEncoderFactory encoderFactory)
        {
            if (encoderFactory == null)
            {
				throw new ArgumentNullException("encoderFactory", "You need to pass an inner encoder to the CaisoMtomMessageEncoderFactory to support SOAP-message processing!");
            }
            else
            {
                encoder = new CaisoMtomMessageEncoder(encoderFactory.Encoder, this);
            }
        }

        public override MessageEncoder CreateSessionEncoder()
        {
            return base.CreateSessionEncoder();
        }
    }
}
