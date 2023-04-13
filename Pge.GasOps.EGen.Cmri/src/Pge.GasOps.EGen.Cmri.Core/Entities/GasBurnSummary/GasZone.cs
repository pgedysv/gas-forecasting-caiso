namespace Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    public partial class GasZone
    {

        private string nameField;

        private GasZoneSummaryLevel summaryLevelField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public GasZoneSummaryLevel summaryLevel
        {
            get
            {
                return this.summaryLevelField;
            }
            set
            {
                this.summaryLevelField = value;
            }
        }
    }
}