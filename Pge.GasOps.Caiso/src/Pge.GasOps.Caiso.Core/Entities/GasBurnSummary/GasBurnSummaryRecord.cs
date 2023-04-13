namespace Pge.GasOps.Caiso.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    public partial class GasBurnSummaryRecord : CMRIReportRecord
    {

        private string gasCompanyField;

        private System.DateTime marketEndTimeField;

        private bool marketEndTimeFieldSpecified;

        private System.DateTime marketStartTimeField;

        private bool marketStartTimeFieldSpecified;

        private GasZone gasZoneField;

        private TradeIntervalData[] tradeIntervalDataField;

        /// <remarks/>
        public string gasCompany
        {
            get
            {
                return this.gasCompanyField;
            }
            set
            {
                this.gasCompanyField = value;
            }
        }

        /// <remarks/>
        public System.DateTime marketEndTime
        {
            get
            {
                return this.marketEndTimeField;
            }
            set
            {
                this.marketEndTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool marketEndTimeSpecified
        {
            get
            {
                return this.marketEndTimeFieldSpecified;
            }
            set
            {
                this.marketEndTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime marketStartTime
        {
            get
            {
                return this.marketStartTimeField;
            }
            set
            {
                this.marketStartTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool marketStartTimeSpecified
        {
            get
            {
                return this.marketStartTimeFieldSpecified;
            }
            set
            {
                this.marketStartTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public GasZone GasZone
        {
            get
            {
                return this.gasZoneField;
            }
            set
            {
                this.gasZoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TradeIntervalData")]
        public TradeIntervalData[] TradeIntervalData
        {
            get
            {
                return this.tradeIntervalDataField;
            }
            set
            {
                this.tradeIntervalDataField = value;
            }
        }
    }
}