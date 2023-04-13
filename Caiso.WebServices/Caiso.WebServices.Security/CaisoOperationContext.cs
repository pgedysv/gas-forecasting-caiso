//------------------------------------------------------------------ 
//-- Copyright (c) 2014, Northern California Power Agency 
//-- Use subject to the accompanying Open Source License Agreement 
//------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Caiso.WebServices
{
	public class CaisoOperationContext : IExtension<OperationContext>
	{
		private readonly IDictionary<string, string> outgoingItems;
		private readonly IDictionary<string, byte[]> incomingItems;

		private CaisoOperationContext()
		{
			outgoingItems = new Dictionary<string, string>();
			incomingItems = new Dictionary<string, byte[]>();
		}

		/// <summary>
		/// This will hold all the attachments parsed by the Mime Parser. This is done because it is safe way of passing information between the WCF Pipeline 
		/// with out worrying for which thread is currently executing the process.
		/// </summary>
		public IDictionary<string, string> OutgoingItems
		{
			get { return outgoingItems; }
		}

		public IDictionary<string, byte[]> IncomingItems
		{
			get { return incomingItems; }
		}

		public static CaisoOperationContext Current
		{
			get
			{    
				if (OperationContext.Current == null)
				{
					var buffer = new StringBuilder();
					buffer.Append("Please provide an OperationContext by wrapping your web service call with this routine:\n");
					buffer.Append("using (var scope = new OperationContextScope(proxy.InnerChannel))\n");
					buffer.Append("{\n");
					buffer.Append("    var x = proxy.webServiceMethod();\n");
					buffer.Append("}");				
					throw new NullReferenceException(buffer.ToString());
				}
				CaisoOperationContext context = OperationContext.Current.Extensions.Find<CaisoOperationContext>();
				if (context == null)
				{
					context = new CaisoOperationContext();
					OperationContext.Current.Extensions.Add(context);
				}
				return context;
			}
		}

		public void Attach(OperationContext owner)
		{
		}

		public void Detach(OperationContext owner)
		{
		}
	}

}
