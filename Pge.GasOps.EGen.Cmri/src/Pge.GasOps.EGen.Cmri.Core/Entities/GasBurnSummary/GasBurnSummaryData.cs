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
namespace Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#", IsNullable = false)]
    public partial class GasBurnSummaryData
    {

        private MessageHeader messageHeaderField;

        private GasBurnSummaryRecord[] messagePayloadField;

        /// <remarks/>
        public MessageHeader MessageHeader
        {
            get
            {
                return this.messageHeaderField;
            }
            set
            {
                this.messageHeaderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public GasBurnSummaryRecord[] MessagePayload
        {
            get
            {
                return this.messagePayloadField;
            }
            set
            {
                this.messagePayloadField = value;
            }
        }
    }
}
