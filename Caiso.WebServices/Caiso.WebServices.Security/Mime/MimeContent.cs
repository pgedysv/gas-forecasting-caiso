//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caiso.WebServices.Mime
{
	public class MimeContent
	{
		public string ContentType{ get; set; }
		public string Start { get; set; }
		public string Boundary { get; set; }
		private List<MimePart> parts = new List<MimePart>();
		public List<MimePart> Parts { get { return parts; } }
	}
}
