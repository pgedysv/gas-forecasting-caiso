using Pge.GasOps.EGen.Cmri.Core.Entities;
using Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary;
using Pge.GasOps.EGen.Cmri.Core.Events.Interfaces;
using Pge.GasOps.EGen.Cmri.Infrastructure.Data.Certificates;
using Pge.GasOps.EGen.Cmri.Infrastructure.Data.GasBurnSummaries;
using Pge.GasOps.EGen.Cmri.Infrastructure.Services;
using Pge.GasOps.EGen.Cmri.Web.Services;

using Serilog;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Pge.GasOps.EGen.Cmri.Web.Controllers
{
    public class GasBurnSummaryController : ApiController
    {
        private readonly string x509SubjectName = ConfigurationManager.AppSettings["X509SubjectName"];
        private readonly string x509ThumbPrint = ConfigurationManager.AppSettings["X509ThumbPrint"];
        private readonly string caisoWebServiceName = ConfigurationManager.AppSettings["CaisoWebServiceName"];
        private readonly string precedingDaysToDelete = ConfigurationManager.AppSettings["PrecedingDaysToDelete"];
        private readonly ConnectionStringSettings dbConnectionString = ConfigurationManager.ConnectionStrings["Forecasting"];

        //private readonly ILogger<GasBurnSummaryController> logger;

        // GET api/caiso/cmri/gasburnsummary
        public IEnumerable<string> Get()
        {
            Log.Verbose("Retrieving CertFactory for CA ISO '{caisoWebServiceName}' web service for certs with subject name '{x509SubjectName}' or Thumbprint '{x509ThumbPrint}'.",
                caisoWebServiceName, x509SubjectName, x509ThumbPrint);
            CertificateFactory.Register(caisoWebServiceName, new X509CertificateFactory(x509SubjectName, x509ThumbPrint));

            IX509CertificateFactory factory = CertificateFactory.Get(caisoWebServiceName);
            string message;

            if (factory != null)
            {
                message = $"Created {caisoWebServiceName} Certificate Factory with Subject {factory.SubjectName}";
                Log.Information(message);

                X509Certificate2 cert = factory.GetCertificate();

                if (cert != null)
                {
                    DateTime startTime = DateTime.Now.Date;
                    DateTime endTime = startTime.AddDays(3);
                    StringBuilder statusMessage = new StringBuilder();

                    // TODO refactor the below into 2 calls of a single method
                    // get day ahead forecast
                    GasBurnSummaryData dataDam = GasBurnSummaryService.GetGasBurnSummaryData(startTime, endTime);
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    if (dataDam.MessagePayload == null || !dataDam.MessagePayload.Any())
                    {
                       statusMessage.AppendLine("No data for DAM - CA ISO API no data detected");
                    }

                    MarketPeriod marketPeriodDam = new MarketPeriod(dataDam, startTime, endTime);
                    Log.Verbose($"{marketPeriodDam}");

                    // Convert PrecedingDaysToDelete to a negative number
                    int.TryParse(precedingDaysToDelete, out int minusDays);

                    //TODO - make this an app config and/or url paramater for debug support
                    bool writeToSql = true;

                    if (writeToSql)
                    {
                        //string deleteOutcome = SharePointServices.DeleteSharePointItems(sharePointUrl, sharePointListName, sharePointCredential, DateTime.Today.AddDays(minusDays * -1));
                        //statusMessage.AppendLine($"Clear list: {deleteOutcome}");
                        string writeDamOutcome = SqlServices.SendToSql(dbConnectionString, marketPeriodDam, "DAM");
                        statusMessage.AppendLine($"Write DA to list: {writeDamOutcome}");
                    }

                    stopWatch.Stop();
                    int timeBetweenRequests = stopWatch.Elapsed.Milliseconds;
                    if (timeBetweenRequests < 5000)
                    {
                        Thread.Sleep(5000 - timeBetweenRequests);
                    }
                    // get 2-day ahead forecast
                    GasBurnSummaryData data2Da = GasBurnSummaryService.GetGasBurnSummaryData(startTime, endTime, MarketType.Item2DA);

                    if (data2Da?.MessagePayload == null || !data2Da.MessagePayload.Any())
                    {
                        statusMessage.AppendLine("No data for 2DA - CA ISO API no data detected");
                    }

                    MarketPeriod marketPeriod2Da = new MarketPeriod(data2Da, startTime, endTime);
                    Log.Verbose($"{marketPeriod2Da}");
                    
                    if (writeToSql)
                    {
                        string write2DaOutcome = SqlServices.SendToSql(dbConnectionString, marketPeriod2Da, "2DA");
                        statusMessage.AppendLine($"Write 2DA to list: {write2DaOutcome}");
                    }

                    return new string[] { $"{statusMessage}", $"{marketPeriodDam}", $"{marketPeriod2Da}", startTime.ToShortDateString(), endTime.ToShortDateString() };
                }

                message = $"The {caisoWebServiceName} CertificateFactory {factory.SubjectName} could not return a certificate with Subject Name {x509SubjectName}.";
                Log.Error("Error: " + message);
                
                return new string[] { message };
            }

            message = $"A CertificateFactory could not be created to return certificates for CA ISO '{caisoWebServiceName}' web service.";
            Log.Error("Error: " + message);
            
            return new string[] { message };
        }

        // GET api/caiso/cmri/gasburnsummary/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/caiso/cmri/gasburnsummary
        public void Post([FromBody]string value)
        {
        }

        // PUT api/caiso/cmri/gasburnsummary/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/caiso/cmri/gasburnsummary/5
        public void Delete(int id)
        {
        }
    }
}