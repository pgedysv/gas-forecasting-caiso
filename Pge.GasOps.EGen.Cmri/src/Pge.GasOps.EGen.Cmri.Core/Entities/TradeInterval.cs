using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pge.GasOps.EGen.Cmri.Core.Entities
{
    public class TradeInterval
    {
        public DateTime TradeIntervalStartTime { get; set; }
        public DateTime TradeIntervalEndTime { get; set; }
        public float MMCF { get; set; }
        public float FZNotSpecified { get; set; }
        public float FZ300 { get; set; }
        public float FZ400 { get; set; }

        public TradeInterval()
        {
            TradeIntervalStartTime = DateTime.Today;
            TradeIntervalEndTime = DateTime.Now;
            MMCF = 0;
            FZNotSpecified = 0;
            FZ300 = 0;
            FZ400 = 0;
        }

        public TradeInterval(DateTime startTime, DateTime endTime)
        {
            TradeIntervalStartTime = startTime;
            TradeIntervalEndTime = endTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var prop in this.GetType().GetProperties())
            {
                sb.AppendLine($"{prop.Name}: {prop.GetValue(this)}");
            }

            return sb.ToString();
        }
    }
}
