//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Caiso.WebServices.Security.Mime;

namespace Caiso.WebServices.Security
{
    public class CaisoMtomMessageEncoder : MessageEncoder
    {
        protected readonly CaisoMtomMessageEncoderFactory factory;

        protected readonly MessageEncoder innerEncoder;

        protected readonly MimeParser mimeParser;

        protected MimeContent content;

        protected MimePart soapMimeContent;

        private string contentType;

        public CaisoMtomMessageEncoder(MessageEncoder innerEncoder, CaisoMtomMessageEncoderFactory factory)
        {
            this.factory = factory;
            this.innerEncoder = innerEncoder;
            mimeParser = new MimeParser();
            soapMimeContent = new MimePart
            {
                                           ContentType = "application/xop+xml",
                                           ContentId = $"<{Guid.NewGuid():N}>",
                                           CharSet = "UTF-8",
                                           Type = "text/xml",
                                           TransferEncoding = "8bit"
                                       };
            content = new MimeContent { Boundary = $"----=_{Guid.NewGuid():N}"};
            contentType =
                $"multipart/related; type=\"application/xop+xml\"; start=\"{soapMimeContent.ContentId}\";start-info=\"text/xml\"; boundary=\"{content.Boundary}\"";
            MediaType = contentType;
            content.Parts.Add(soapMimeContent);
        }

        public override string ContentType
        {
            get
            {
                if (OperationContext.Current != null && CaisoOperationContext.Current.OutgoingItems.Count > 0)
                {
                    return contentType;
                }

                return innerEncoder.ContentType;
            }
        }

        public override string MediaType { get; }

        public override MessageVersion MessageVersion => MessageVersion.Soap11;

        public static byte[] ReadBytes(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public override bool IsContentTypeSupported(string contentType)
        {
            var loweredContentType = contentType.ToLower();
            if (loweredContentType.StartsWith("multipart/related"))
                return true;
            if (loweredContentType.StartsWith("text/xml"))
                return true;
            if (loweredContentType.StartsWith("application/xop+xml"))
                return true;
            return false;
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);
            var ms = new MemoryStream(msgContents);
            return ReadMessage(ms, int.MaxValue, contentType);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var lowerContentType = contentType.ToLower();
            if (lowerContentType.StartsWith("multipart/related"))
            {
                var contentBytes = ReadBytes(stream);
                stream.Dispose(); // I can dispose this now since a new stream will be created later

                // MimeKit needs the Content-Type before it can successfully parse the mime payload
                var contentTypeBytes = Encoding.UTF8.GetBytes("Content-Type: " + contentType + "\n\n");
                var bytesForDeserialization = new byte[contentBytes.Length + contentTypeBytes.Length];
                contentTypeBytes.CopyTo(bytesForDeserialization, 0);
                contentBytes.CopyTo(bytesForDeserialization, contentTypeBytes.Length);
                MimeContent content = mimeParser.DeserializeContent(bytesForDeserialization);
                if (content.Parts.Count >= 2)
                {
                    // Assumption is that there will only be one attachment per message.
                    // If this changes then we need to adjust this code or not check for the hash
                    var attachmentHash = MtomXmlHelper.GetAttachmentHash(content.Parts[0].Content);
                    var hash = HashHelper.Sha1Hash(Encoding.UTF8.GetBytes(content.Parts[1].Content));
                    if (hash != attachmentHash)
                    {
                        throw new ApplicationException("Hash of attachment is not the same as the hash in the header.");
                    }

                    var resultXml = MtomXmlHelper.PutMtomAttachmentBackToSoapEnvelope(content);
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(resultXml));
                    Message msg = ReadMessage(ms, int.MaxValue, content.Parts[0].ContentType);
                    return msg;
                }

                throw new ApplicationException(
                    "Invalid mime message sent! Requires at least 2 mime message content parts!");
            }

            if (lowerContentType.StartsWith("text/xml") || lowerContentType == "application/xop+xml")
            {
                XmlReader reader = XmlReader.Create(stream);
                Message msg = Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
                return msg;
            }

            throw new ApplicationException(
                $"Invalid content type for reading message: {contentType}! Supported content types are multipart/related, application/xop+xml and text/xml!");
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            if (OperationContext.Current != null && CaisoOperationContext.Current.OutgoingItems.Count > 0)
            {
                using (var ms = new MemoryStream())
                {
                    innerEncoder.WriteMessage(
                        message,
                        ms); // xml message gets signed by invoking the WriteMessage of the encoder
                    var xml = message.ToString();
                    soapMimeContent.Content = xml;
                    foreach (var key in CaisoOperationContext.Current.OutgoingItems.Keys)
                    {
                        var msgProps = CaisoOperationContext.Current.OutgoingItems[key];
                        var amc = new MimePart
                        {
                            ContentType = "application/octet-stream",
                            TransferEncoding = "binary",
                            Content = msgProps,
                            ContentId = key
                        };
                        content.Parts.Add(amc);
                    }

                    mimeParser.SerializeContent(content, stream);
                }
            }
            else
            {
                innerEncoder.WriteMessage(
                    message,
                    stream); // xml message gets signed by invoking the WriteMessage of the encoder
            }
        }

        public override ArraySegment<byte> WriteMessage(
            Message message,
            int maxMessageSize,
            BufferManager bufferManager,
            int messageOffset)
        {
            var arrBuffer = innerEncoder.WriteMessage(
                message,
                maxMessageSize,
                bufferManager,
                messageOffset); // xml message gets signed by invoking the WriteMessage of the encoder
            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
                      + Encoding.UTF8.GetString(arrBuffer.Array, 0, arrBuffer.Count);
            if (OperationContext.Current != null && CaisoOperationContext.Current.OutgoingItems.Count > 0)
            {
                foreach (var key in CaisoOperationContext.Current.OutgoingItems.Keys)
                {
                    var amc = new MimePart { ContentType = "application/octet-stream", TransferEncoding = "binary" };
                    amc.Content = CaisoOperationContext.Current.OutgoingItems[key];
                    amc.ContentId = key;
                    content.Parts.Add(amc);
                }

                soapMimeContent.Content = xml;
                byte[] mimeBytes = mimeParser.SerializeContent(content);

                // var text = UTF8Encoding.UTF8.GetString(mimeBytes);
                int mimeContentLength = mimeBytes.Length;
                byte[] targetBuffer = bufferManager.TakeBuffer(mimeContentLength + messageOffset);
                Array.Copy(mimeBytes, 0, targetBuffer, messageOffset, mimeContentLength);
                return new ArraySegment<byte>(targetBuffer, messageOffset, mimeContentLength);
            }
            else
            {
                var xmlBytes = Encoding.UTF8.GetBytes(xml);
                byte[] targetBuffer = bufferManager.TakeBuffer(xmlBytes.Length + messageOffset);
                Array.Copy(xmlBytes, 0, targetBuffer, messageOffset, xmlBytes.Length);
                return new ArraySegment<byte>(targetBuffer, messageOffset, xmlBytes.Length);
            }
        }
    }
}