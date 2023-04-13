using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CMRItoSharepoint
{
	public class XmlHelper
	{
		public static string XmlSerialize(object payload)
		{
			string result = null;
			using (var memStream = new MemoryStream())
			{
				var tw = new XmlTextWriter(memStream, new UTF8Encoding());				
				try
				{
					var ser = new XmlSerializer(payload.GetType());
					ser.Serialize(tw, payload);
					memStream.Seek(0, SeekOrigin.Begin);
					var buf = memStream.ToArray();
					result = System.Text.Encoding.UTF8.GetString(buf).Trim();
				}
				finally
				{
					tw.Close();
				}
			}
			return result;
		}		
	}
}
