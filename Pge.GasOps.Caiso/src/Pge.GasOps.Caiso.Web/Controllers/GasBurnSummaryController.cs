// // Copyright (c) 2018 Pacific Gas and Electric Company. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using CmriGasBurnSummaryData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pge.GasOps.Caiso.Core.Entities.GasBurnSummary;
using Pge.GasOps.Caiso.Core.Entities.StandardOutput;
using Pge.GasOps.Caiso.Core.Events.Interfaces;
using Pge.GasOps.Caiso.Infrastructure.Data.Certificates;
using Serilog;

namespace Pge.GasOps.Caiso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GasBurnSummaryController : ControllerBase
    {
        private readonly ILogger<GasBurnSummaryController> logger;


        private const string X509SubjectName = "ANDREW PONGx7824";
        private const string SharePointUrl = "http://wolverine";
        private const string SharePointSubsite = "http://wolverine/sites/pge";
        private const string SharePointList = "CMRI";
        private const string CaisoWebServiceName = "CMRI";

        public GasBurnSummaryController(ILogger<GasBurnSummaryController> logger)
        {
            this.logger = logger;
        }

        // GET api/GasBurnSummary
        [HttpGet]        
        public ActionResult<IEnumerable<string>> Get()
        {

            CertificateFactory.Register(CaisoWebServiceName, new X509CertificateFactory(X509SubjectName));

            IX509CertificateFactory factory = CertificateFactory.Get(CaisoWebServiceName);

            string message;

            if (factory != null)
            {
                message = $"Found {CaisoWebServiceName} CertificateFactory with Subject {factory.SubjectName}";

                logger.LogDebug(message);
                X509Certificate2 cert = factory.GetCertificate();

                DateTime startTime = DateTime.Now.Date;
                DateTime endTime = startTime.AddDays(2);

                // TODO: Fix this.
                //GasBurnSummaryData data = RequestSummary(startTime, endTime, CmriGasBurnSummaryData.MarketType.Item2DA);
                //GasDay x = ConvertToGasDayObject(data);

                return new string[] {message, startTime.ToShortDateString(), endTime.ToShortDateString()};
            }
            else
            {
                message = $"No certificates were fround with Subject Name {X509SubjectName}";

                logger.LogDebug(message);

                return new string[] {message};

            }
        }

        //// TODO: Fix this.
        //private static GasBurnSummaryData RequestSummary(DateTime start, DateTime end,
        //                                                 CmriGasBurnSummaryData.MarketType marketType)
        //{
        //    var response = new GasBurnSummaryData();
        //    var error = new outputDataType();

        //    const string url = "https://ws.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP";

        //    //black box of signing certificate
        //    var factory =
        //        new WSProxyFactory<retrieveGasBurnSummaryData_v1_DocAttachClient,
        //            retrieveGasBurnSummaryData_v1_DocAttach>();
        //    retrieveGasBurnSummaryData_v1_DocAttachClient proxy = factory.CreateMTOMProxy(url, "CMRI");

        //    var request = new RequestGasBurnSummaryData
        //    {
        //        MessageHeader = new CmriGasBurnSummaryData.MessageHeader
        //        {
        //            Source = "DSTPGE",
        //            TimeDate = DateTime.Now.ToUniversalTime(),
        //            Version = "v20171001"
        //        },
        //        MessagePayload = new MessagePayload
        //        {
        //            RequestGasBurnSummaryRecord = new RequestGasBurnSummaryRecord
        //            {
        //                GasCompanyList = new GasCompanyList[1]
        //                    {new GasCompanyList {gasCompany = "PGAE"}},
        //                GasZoneList = new CmriGasBurnSummaryData.GasZone[3]
        //                {
        //                    new CmriGasBurnSummaryData.GasZone
        //                    {
        //                        name = "PGAE 300",
        //                        summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
        //                    },
        //                    new CmriGasBurnSummaryData.GasZone
        //                    {
        //                        name = "PGAE 400/401",
        //                        summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
        //                    },
        //                    new CmriGasBurnSummaryData.GasZone
        //                    {
        //                        name = "NOT SPECIFIED",
        //                        summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
        //                    }
        //                },
        //                dateTimeStart = start,
        //                dateTimeEnd = end,
        //                marketType = marketType,
        //                executionType = CmriGasBurnSummaryData.PTRExecutionType.RUC
        //            }
        //        }
        //    };

        //    //for debugging request message
        //    Log.Verbose(SerializeXml(request));

        //    try
        //    {
        //        var req = new retrieveGasBurnSummaryData_v1_DocAttachRequest {RequestGasBurnSummaryData = request};

        //        //call to api - att returns base64 encoded zipped string

        //        proxy.retrieveGasBurnSummaryData_v1_DocAttach(request, out ISOAttachment att);

        //        byte[] decoded = Convert.FromBase64String(att.AttachmentValue);
        //        var dstream = new MemoryStream(decoded, false);
        //        using (var gZipStream = new GZipStream(dstream, CompressionMode.Decompress))
        //        {
        //            string isoAttachmentString;
        //            using (var stringReader = new StreamReader(gZipStream))
        //            {
        //                stringReader.ReadLine();
        //                isoAttachmentString = stringReader.ReadToEnd();
        //            }

        //            var summarySerializer = new XmlSerializer(response.GetType());
        //            var errorSerializer = new XmlSerializer(error.GetType());

        //            using (var stringReader = new StringReader(isoAttachmentString))
        //            {
        //                using (XmlReader xmlReader = XmlReader.Create(stringReader))
        //                {
        //                    using (var stringWriter = new StringWriter())
        //                    {
        //                        using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
        //                        {
        //                            if (summarySerializer.CanDeserialize(xmlReader))
        //                            {
        //                                response = (GasBurnSummaryData) summarySerializer.Deserialize(xmlReader);
        //                                summarySerializer.Serialize(xmlWriter, response);
        //                                Log.Verbose(
        //                                    "{count} GasBurnSummaryRecords were retrieved and deserialized.",
        //                                    response.MessagePayload.Length);
        //                            }
        //                            else if (errorSerializer.CanDeserialize(xmlReader))
        //                            {
        //                                error = (outputDataType) errorSerializer.Deserialize(xmlReader);
        //                                errorSerializer.Serialize(xmlWriter, error);
        //                                Log.Verbose(
        //                                    "{count} GasBurnSummaryRecords were retrieved, but an {errorType} error occurred while trying to deserialize the XML data: {error}",
        //                                    error.EventLog.Length, error.GetType().ToString(), error.ToString());
        //                            }
        //                            else
        //                            {
        //                                Log.Verbose(
        //                                    "GasBurnSummaryRecords were retrieved, but an unknown error occurred while trying to deserialize the XML data.");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Failed while trying to parse RequestGasBurnSummaryData");
        //        return response;
        //    }
        //    finally
        //    {
        //        proxy.Close();
        //    }
        //}
    }
}