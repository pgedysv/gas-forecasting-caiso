using CaisoWebServiceTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pge.GasOps.Egen.Caiso.Web.Net.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        private const string X509SubjectName = "ANDREW PONGx7824";
        private const string SharePointUrl = "http://wolverine";
        private const string SharePointSubsite = "http://wolverine/sites/pge";
        private const string SharePointList = "CMRI";

        // GET api/values
        public IEnumerable<string> Get()
        {
            DateTime startTime = DateTime.Now.Date;
            DateTime endTime = startTime.AddDays(2);

            BootStrapper.Configure(X509SubjectName);
            GasBurnSummaryData data = Program.RequestSummary(startTime, endTime, CaisoWebServiceTest.CmriGasBurnSummaryData.MarketType.Item2DA);
            GasDay x = Program.ConvertToGasDayObject(data);

            Program.SendToSharepoint(x);

            return new string[] { 
                startTime.ToShortDateString(), 
                startTime.ToShortTimeString(), 
                endTime.ToShortDateString(), 
                endTime.ToShortTimeString(),
                data.ToString(), x.ToString()
            };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
