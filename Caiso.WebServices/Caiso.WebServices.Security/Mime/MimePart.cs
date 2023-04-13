//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Caiso.WebServices.Mime
{
	public class MimePart
	{
		public string ContentType { get; set; }
		public string TransferEncoding { get; set; }
		public string ContentId { get; set; }
		public string CharSet { get; set; }
		public string Type { get; set; }
		public string Content { get; set; }
	}
}
