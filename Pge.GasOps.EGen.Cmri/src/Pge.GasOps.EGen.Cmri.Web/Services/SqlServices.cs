using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Pge.GasOps.EGen.Cmri.Core.Entities;
using Pge.GasOps.EGen.Cmri.Web.Models;
using Serilog;


namespace Pge.GasOps.EGen.Cmri.Web.Services
{
    public static class SqlServices
    {
        public static string SendToSql(ConnectionStringSettings connectionString, MarketPeriod data, string marketType)
        {
            string message;
            StringBuilder logMessage = new StringBuilder();

            try
            {
                using (var ctx = new ForecastingContext(connectionString))
                {
                    // inserting to or updating list
                    int countExistedUpdated = 0;
                    int countExistedNotUpdated = 0;
                    int countNewItem = 0;
                    foreach (TradeInterval tradeInterval in data.TradeIntervals.Values)
                    {
                        // Check if item exists
                        bool matchingRowExists = ctx.CMRIs.Any(a => (a.StartTime == tradeInterval.TradeIntervalStartTime) &&
                                                                    (a.DA == marketType));
                        
                        // Write everything except the created field if it does exist
                        if (matchingRowExists)
                        {
                            var matchingRow = ctx.CMRIs.Single(a => (a.StartTime == tradeInterval.TradeIntervalStartTime) &&
                                                              (a.DA == marketType));
                            double oldMMCF = matchingRow.MMCF;
                            if (oldMMCF != tradeInterval.MMCF)
                            {
                                matchingRow.MMCF = Math.Round(tradeInterval.MMCF, 2);
                                logMessage.AppendLine($"{tradeInterval.TradeIntervalStartTime} - {marketType}: Combination existed, mmcf matches, no update necessary");
                                countExistedNotUpdated++;
                            }
                            else
                            {
                                logMessage.AppendLine($"{tradeInterval.TradeIntervalStartTime} - {marketType}: Combination existed, old value: {oldMMCF}, new value: {tradeInterval.MMCF}");
                                countExistedUpdated++;
                            }
                            matchingRow.Modified = DateTime.Now;
                            ctx.SaveChanges();
                        }
                        else // if it doesn't exist, save new entries
                        {
                            var newRow = new CMRI();
                            newRow.StartTime = tradeInterval.TradeIntervalStartTime;
                            newRow.EndTime = tradeInterval.TradeIntervalEndTime;
                            newRow.MMCF = Math.Round(tradeInterval.MMCF, 2);
                            newRow.DA = marketType;
                            var now = DateTime.Now;
                            newRow.Modified = now;
                            newRow.Created = now;
                            ctx.CMRIs.Add(newRow);
                            ctx.SaveChanges();
                            countNewItem++;
                            logMessage.AppendLine($"{tradeInterval.TradeIntervalStartTime} - {marketType}: Combination did not exist, creating new list item - mmcf: {tradeInterval.MMCF}");
                        }
                    }

                    message = $"success - total updated: {countExistedUpdated} - total no update necessary: {countExistedNotUpdated} - total new items: {countNewItem}.  ";
                }
            }
            catch (SqlException e)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < e.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + e.Errors[i].Message + "\n" +
                        "LineNumber: " + e.Errors[i].LineNumber + "\n" +
                        "Source: " + e.Errors[i].Source + "\n" +
                        "Procedure: " + e.Errors[i].Procedure + "\n");
                }

                logMessage.AppendLine("Error writing GasDay data to SQL");
                message = "failure";
            }
            catch (Exception e)
            {
                Log.Error(e, "Generic error encountered writing GasDay data to SQL.");
                logMessage.AppendLine($"Generic error encountered writing GasDay data to SQL. - {e}");
                message = "failure";
            }

            Log.Information(logMessage.ToString());

            return message;
        }
    }
}
