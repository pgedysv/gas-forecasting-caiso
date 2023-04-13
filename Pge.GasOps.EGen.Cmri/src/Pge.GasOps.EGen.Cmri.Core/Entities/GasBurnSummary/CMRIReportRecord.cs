namespace Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GasBurnSummaryRecord))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    public partial class CMRIReportRecord
    {

        private PTRExecutionType executionTypeField;

        private MarketType marketTypeField;

        /// <remarks/>
        public PTRExecutionType executionType
        {
            get
            {
                return this.executionTypeField;
            }
            set
            {
                this.executionTypeField = value;
            }
        }

        /// <remarks/>
        public MarketType marketType
        {
            get
            {
                return this.marketTypeField;
            }
            set
            {
                this.marketTypeField = value;
            }
        }
    }
}