using System;
using System.Linq;
using Pge.GasOps.EGen.Cmri.Core.Entities;
using Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary;
using Serilog;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Data.GasDays
{
    public static class GasDayService
    {
        public static GasDay ConvertToGasDayObject(GasBurnSummaryData data)
        {
            //TODO: Handle Daylight Savings
            //currently return timestamps in local time;
           
            // TODO: Change GasDay to Array; Add NumberOfDays parameter
            var gasDay = new GasDay(DateTime.Now.Date);
            var gasDay2 = new GasDay(DateTime.Now.Date.AddDays(1));

            //currently pulling single day. Need gasday range of two days
            GasBurnSummaryRecord[] records = data?.MessagePayload;
            
            if (records != null && records.Any())
            {
                foreach (GasBurnSummaryRecord record in records)
                {
                    DateTime endtime = record.TradeIntervalData[0].intervalEndTime;
                    float value = record.TradeIntervalData[0].gasBurn;
                    int endingHour = endtime.Hour == 0 ? 24 : endtime.Hour;
                    string property = "HE" + endingHour.ToString("D2");
                    if (record.marketStartTime.Date == gasDay.Date.Date)
                    {
                        var prevValue = (float) gasDay.GetType().GetField(property).GetValue(gasDay);
                        gasDay.GetType().GetField(property).SetValue(gasDay, value + prevValue);
                    }
                    else
                    {
                        gasDay2.GetType().GetField(property).SetValue(gasDay2, value);
                    }
                }

                Log.Verbose("GasDay object returned for {Date}.", gasDay.Date);
            }

            return gasDay;
        }
    }
}
