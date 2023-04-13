namespace Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    public partial class MessageHeader
    {

        private System.DateTime timeDateField;

        private string sourceField;

        private string versionField;

        public MessageHeader()
        {
            this.versionField = "v20171001";
        }

        /// <remarks/>
        public System.DateTime TimeDate
        {
            get
            {
                return this.timeDateField;
            }
            set
            {
                this.timeDateField = value;
            }
        }

        /// <remarks/>
        public string Source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }
}