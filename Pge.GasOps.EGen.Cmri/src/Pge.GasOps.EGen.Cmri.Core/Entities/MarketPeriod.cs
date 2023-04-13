using Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pge.GasOps.EGen.Cmri.Core.Entities
{
    public class MarketPeriod
    {
        public DateTime MarketPeriodStartTime;
        public DateTime MarketPeriodEndTime;
        public Dictionary<DateTime,TradeInterval> TradeIntervals = new Dictionary<DateTime, TradeInterval>();

        public MarketPeriod(GasBurnSummaryData data, DateTime startTime, DateTime endTime)
        {
            GasBurnSummaryRecord[] records = data?.MessagePayload;
            MarketPeriodStartTime = startTime;
            MarketPeriodEndTime = endTime;

            if (records == null || !records.Any()) return;
            foreach (GasBurnSummaryRecord record in records)
            {
                DateTime intervalStartTime = record.TradeIntervalData[0].intervalStartTime;
                DateTime intervalEndTime = record.TradeIntervalData[0].intervalEndTime;
                float value = record.TradeIntervalData[0].gasBurn;

                if (!TradeIntervals.TryGetValue(intervalStartTime, out _))
                {
                    TradeIntervals.Add(intervalStartTime, new TradeInterval(intervalStartTime, intervalEndTime));
                    TradeIntervals[intervalStartTime].MMCF = value;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Start Time: {MarketPeriodStartTime}");
            sb.AppendLine($"End Time:  {MarketPeriodEndTime}");
            sb.AppendLine($"Number of intervals: {TradeIntervals.Count}");

            foreach (TradeInterval tradeInterval in TradeIntervals.Values)
            {
                sb.AppendLine($"{tradeInterval}");
            }

            return sb.ToString();
        }
    }
}
