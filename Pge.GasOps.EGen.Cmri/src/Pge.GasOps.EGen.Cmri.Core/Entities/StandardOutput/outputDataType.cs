﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.7.3062.0.
// 
namespace Pge.GasOps.EGen.Cmri.Core.Entities.StandardOutput
{
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/2006-06-13/StandardOutput.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.caiso.com/soa/2006-06-13/StandardOutput.xsd", IsNullable = false)]
    public partial class OutputDataType
    {

        private EventLog[] eventLogField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EventLog")]
        public EventLog[] EventLog
        {
            get
            {
                return this.eventLogField;
            }
            set
            {
                this.eventLogField = value;
            }
        }
    }
}
