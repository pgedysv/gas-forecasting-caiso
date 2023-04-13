namespace Pge.GasOps.Caiso.Core.Entities.GasBurnSummary
{
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TradeIntervalData))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.caiso.com/soa/GasBurnSummaryData_v1.xsd#")]
    public partial class TradeHourData
    {

        private float gasBurnField;

        private System.DateTime intervalEndTimeField;

        private bool intervalEndTimeFieldSpecified;

        private System.DateTime intervalStartTimeField;

        /// <remarks/>
        public float gasBurn
        {
            get
            {
                return this.gasBurnField;
            }
            set
            {
                this.gasBurnField = value;
            }
        }

        /// <remarks/>
        public System.DateTime intervalEndTime
        {
            get
            {
                return this.intervalEndTimeField;
            }
            set
            {
                this.intervalEndTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool intervalEndTimeSpecified
        {
            get
            {
                return this.intervalEndTimeFieldSpecified;
            }
            set
            {
                this.intervalEndTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime intervalStartTime
        {
            get
            {
                return this.intervalStartTimeField;
            }
            set
            {
                this.intervalStartTimeField = value;
            }
        }
    }
}