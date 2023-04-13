//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System.IO;
using System.Text;
using MimeKit;

namespace Caiso.WebServices.Security.Mime
{
	public class MimeParser
	{
		private static readonly char[] CRLF = new char[] { '\r', '\n' };

		public byte[] SerializeContent(MimeContent content)
		{
			using (var stream = new MemoryStream())
			{
				SerializeContent(content, stream);
				return stream.ToArray();
			}
		}

		public void SerializeContent(MimeContent content, Stream stream)
		{
			string boundary = "--" + content.Boundary;
			StringBuilder builder = new StringBuilder();
			foreach (var item in content.Parts)
			{
				builder.Append(CRLF);
				builder.Append(boundary);
				builder.Append(CRLF);
				builder.Append(string.Format("Content-Type: {0}", item.ContentType));
				if (!string.IsNullOrEmpty(item.CharSet))
				{
					builder.Append(string.Format("; charset={0}", item.CharSet));
				}
				if (!string.IsNullOrEmpty(item.Type))
				{
					builder.Append(string.Format("; type={0}", item.Type));
				}
				builder.Append(CRLF);
				builder.Append(string.Format("Content-Transfer-Encoding: {0}", item.TransferEncoding));
				builder.Append(CRLF);
				builder.Append(string.Format("Content-Id: {0}", item.ContentId));
				builder.Append(CRLF);
				builder.Append(CRLF);
				builder.Append(item.Content);
			}
			builder.Append(CRLF);
			builder.Append(boundary);
			builder.Append("--");
			builder.Append(CRLF);
			var payload = builder.ToString();
			var mimeBytes = Encoding.UTF8.GetBytes(payload);
			stream.Write(mimeBytes, 0, mimeBytes.Length);
		}

	    public MimeContent DeserializeContent(byte[] payload)
	    {
	        MimeContent content = new MimeContent();
	        using (var stream = new MemoryStream(payload))
	        {
	            stream.Seek(0, 0);
	            // I'd rather delegate the parsing code to those who knows what they are doing. I selected MimeKit because of the 
	            // permissive License used (MIT) and it implements all the MIME standards. It is easy to use and the company who 
	            // created it is Xamarin. Enough said.
	            var mimeMsg = MimeMessage.Load(stream);
	            foreach (MimeKit.MimePart bodyPart in mimeMsg.BodyParts)
	            {
	                var mimePart = new MimePart();
	                content.Parts.Add(mimePart);
	                foreach (var header in bodyPart.Headers)
	                {
	                    switch (header.Id)
	                    {
	                        case HeaderId.ContentId:
	                            mimePart.ContentId = header.Value;
	                            break;
	                        case HeaderId.ContentType:
	                            mimePart.ContentType = header.Value;
	                            break;
	                        case HeaderId.ContentTransferEncoding:
	                            mimePart.TransferEncoding = header.Value;
	                            break;
	                    }
	                }
	                using (Stream contentStream = bodyPart.Content.Open())
	                {
	                    using (var reader = new StreamReader(contentStream))
	                    {
	                        mimePart.Content = reader.ReadToEnd();
	                    }
	                }
	            }
	        }
	        return content;
	    }
    }
}
