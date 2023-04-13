//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Caiso.WebServices.Security.Security
{
    public class CaisoClientMessageInspector : IClientMessageInspector
    {

        private X509Certificate2 cert;
        public CaisoClientMessageInspector(X509Certificate2 cert)
        {
            this.cert = cert;
        }

        #region IClientMessageInspector Members

        public virtual void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {            
        }

        public virtual object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            CAISOWSHeader header = new CAISOWSHeader();
            header.CAISOUsernameToken.Username = NormalizeSubject(cert.Subject);
            header.CAISOUsernameToken.Nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Replace("-", "")));
            header.CAISOMessageInfo.MessageID = Guid.NewGuid().ToString();
            header.CAISOMessageInfo.Timestamp = new TimestampType();
            header.CAISOMessageInfo.Timestamp.Created = CreateDate();
            header.CAISOMessageInfo.Timestamp.Expires = header.CAISOMessageInfo.Timestamp.Created.AddMinutes(15);
            header.CAISOMessageInfo.Timestamp.ExpiresSpecified = true;
            header.CAISOUsernameToken.Created = header.CAISOMessageInfo.Timestamp.Created;
            request.Headers.Add(header);            
            return null;
        }

        private DateTime CreateDate()
        {
            DateTime dt = DateTime.Now;
            string sSec = dt.Millisecond.ToString();
            if (sSec.Length > 3)
            {
                sSec = sSec.Substring(0, 3);
            }
            int msec = int.Parse(sSec);
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, msec, DateTimeKind.Local);
        }

        /// <summary> 
        /// Removes all the white spaces in the subject except on the CN attribute. 
        /// Replaces the OID.2.5.4.5 with its friendly name SerialNumber. 
        /// </summary> 
        /// <param name="Subject"></param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        private string NormalizeSubject(string Subject)
        {
            int firstCommaIndex = Subject.IndexOf(",");
            string cn = Subject.Substring(0, firstCommaIndex);

            string certString = Subject.Substring(firstCommaIndex + 1, Subject.Length - firstCommaIndex - 1);

            // Replace the OID with Serial Number as it should be the friendly name of OID.2.5.4.5 
            certString = certString.Replace("OID.2.5.4.5", "SerialNumber");
            string[] temp = certString.Trim().Split(' ');
            string result = null;
            result = cn + ",";
            foreach (string s in temp)
            {
                result = result + s;
            }
            result = result.Replace("SERIALNUMBER", "SerialNumber");
            return result;
        }


        #endregion
    }
}
