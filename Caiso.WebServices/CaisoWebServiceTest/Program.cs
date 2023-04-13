using Caiso.WebServices;

using CaisoWebServiceTest.CmriGasBurnSummaryData;

using Microsoft.SharePoint.Client;

using Serilog;
using Serilog.Sinks.Slack.Core;
using Serilog.Sinks.ApplicationInsights;
using Serilog.Sinks.EventLog;
using Serilog.Sinks.SystemConsole;

using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace CaisoWebServiceTest
{
    public class Program
    {
        // Change this to your SC ID
        private const string X509SubjectName = "ANDREW PONGx7824";
        private const string SharePointUrl = "http://wolverine";
        private const string SharePointSubsite = "http://wolverine/sites/pge";
        private const string SharePointList = "CMRI";

        private static void Main(string[] args)
        {

            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
            //    azureServiceTokenProvider.KeyVaultTokenCallback));
            //var secret = await keyVaultClient.GetSecretAsync("https://dst-prod-keyvault.vault.azure.net/secrets/secret").ConfigureAwait(false);



            //var client = EventHubClient.CreateFromConnectionString(description.Path);

            //var log = new LoggerConfiguration()
            //          .WriteTo.AzureEventHub()
            //          .CreateLogger();

            DateTime startTime = DateTime.Now.Date;
            DateTime endTime = startTime.AddDays(2);

            BootStrapper.Configure(X509SubjectName);
            GasBurnSummaryData data = RequestSummary(startTime, endTime, CmriGasBurnSummaryData.MarketType.Item2DA);
            GasDay x = ConvertToGasDayObject(data);

            SendToSharepoint(x);

            Log.CloseAndFlush();
            Console.ReadKey();
        }

        public static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Verbose()
                         .WriteTo.Debug()
                         .WriteTo.Console()
                         .WriteTo.ApplicationInsightsEvents("3f111ff9-818f-4f67-a8b3-b531b7980af1")
                         .WriteTo.EventLog("PG&E GasOps - EGen CAISO Integration Service")
                         .WriteTo.Slack("https://hooks.slack.com/services/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")
                         .CreateLogger();

            Log.Information("Logging started for PG&E GasOps - EGen CAISO Integration Service.");
        }


        private static string SerializeXml(object obj)
        {
            string response;
            var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            var serializer = new XmlSerializer(obj.GetType());
            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
                {
                    serializer.Serialize(xmlWriter, obj);
                }

                response = stringWriter.ToString();
            }
            return response;
        }

        //add start, end, market type to parameters
        public static GasBurnSummaryData RequestSummary(DateTime start, DateTime end,
                                                         CmriGasBurnSummaryData.MarketType marketType)
        {
            var response = new GasBurnSummaryData();
            var error = new outputDataType();

            const string url = "https://ws.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP";

            //black box of signing certificate
            var factory =
                new WSProxyFactory<retrieveGasBurnSummaryData_v1_DocAttachClient,
                    retrieveGasBurnSummaryData_v1_DocAttach>();
            retrieveGasBurnSummaryData_v1_DocAttachClient proxy = factory.CreateMTOMProxy(url, "CMRI");

            var request = new RequestGasBurnSummaryData
            {
                MessageHeader = new CmriGasBurnSummaryData.MessageHeader
                {
                    Source = "DSTPGE",
                    TimeDate = DateTime.Now.ToUniversalTime(),
                    Version = "v20171001"
                },
                MessagePayload = new MessagePayload
                {
                    RequestGasBurnSummaryRecord = new RequestGasBurnSummaryRecord
                    {
                        GasCompanyList = new GasCompanyList[1]
                            {new GasCompanyList {gasCompany = "PGAE"}},
                        GasZoneList = new CmriGasBurnSummaryData.GasZone[3]
                        {
                            new CmriGasBurnSummaryData.GasZone
                            {
                                name = "PGAE 300",
                                summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
                            },
                            new CmriGasBurnSummaryData.GasZone
                            {
                                name = "PGAE 400/401",
                                summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
                            },
                            new CmriGasBurnSummaryData.GasZone
                            {
                                name = "NOT SPECIFIED",
                                summaryLevel = CmriGasBurnSummaryData.GasZoneSummaryLevel.FZONE
                            }
                        },
                        dateTimeStart = start,
                        dateTimeEnd = end,
                        marketType = marketType,
                        executionType = CmriGasBurnSummaryData.PTRExecutionType.RUC
                    }
                }
            };

            //for debugging request message
            Log.Verbose(SerializeXml(request));

            try
            {
                var req = new retrieveGasBurnSummaryData_v1_DocAttachRequest {RequestGasBurnSummaryData = request};

                //call to api - att returns base64 encoded zipped string

                proxy.retrieveGasBurnSummaryData_v1_DocAttach(request, out ISOAttachment att);

                byte[] decoded = Convert.FromBase64String(att.AttachmentValue);
                var dstream = new MemoryStream(decoded, false);
                using (var gZipStream = new GZipStream(dstream, CompressionMode.Decompress))
                {
                    string isoAttachmentString;
                    using (var stringReader = new StreamReader(gZipStream))
                    {
                        stringReader.ReadLine();
                        isoAttachmentString = stringReader.ReadToEnd();
                    }

                    var summarySerializer = new XmlSerializer(response.GetType());
                    var errorSerializer = new XmlSerializer(error.GetType());

                    using (var stringReader = new StringReader(isoAttachmentString))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(stringReader))
                        {
                            using (var stringWriter = new StringWriter())
                            {
                                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                                {
                                    if (summarySerializer.CanDeserialize(xmlReader))
                                    {
                                        response = (GasBurnSummaryData) summarySerializer.Deserialize(xmlReader);
                                        summarySerializer.Serialize(xmlWriter, response);
                                        Log.Verbose(
                                            "{count} GasBurnSummaryRecords were retrieved and deserialized.",
                                            response.MessagePayload.Length);
                                    }
                                    else if (errorSerializer.CanDeserialize(xmlReader))
                                    {
                                        error = (outputDataType) errorSerializer.Deserialize(xmlReader);
                                        errorSerializer.Serialize(xmlWriter, error);
                                        Log.Verbose(
                                            "{count} GasBurnSummaryRecords were retrieved, but an {errorType} error occurred while trying to deserialize the XML data: {error}",
                                            error.EventLog.Length, error.GetType().ToString(), error.ToString());
                                    }
                                    else
                                    {
                                        Log.Verbose(
                                            "GasBurnSummaryRecords were retrieved, but an unknown error occurred while trying to deserialize the XML data.");
                                    }
                                }
                            }
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed while trying to parse RequestGasBurnSummaryData");
                return response;
            }
            finally
            {
                proxy.Close();
            }
        }

        public static GasDay ConvertToGasDayObject(GasBurnSummaryData data)
        {
            //TODO: Handle Daylight Savings
            //currently return timestamps in local time;
           
            // TODO: Change GasDay to Array; Add NumberOfDays parameter
            var gasDay = new GasDay(DateTime.Now.Date);
            var gasDay2 = new GasDay(DateTime.Now.Date.AddDays(1));

            //currently pulling single day. Need gasday range of two days
            GasBurnSummaryRecord[] records = data?.MessagePayload;
            
            if (records != null && records.Count() > 0)
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

        public static void SendToSharepoint(GasDay data)
        {
            //Import Reference: Microsoft.Sharepoint.Client (and Client.Runtime) v15 for sharepoint 2013
            //google sharepoint 2013 sdk

            //install nuget Microsoft.SharePointOnline.CSOM
            try
            {
                using (var ctx = new ClientContext(SharePointSubsite))
                {
                    Log.Debug("Using Execute Query on Context: {context}", ctx);

                    ctx.Credentials = new NetworkCredential("svc-sp-pge", "Leavinghatcheryfrugallyninepins45", "DST-DOM");
                    Log.Debug("Using credentials: {credentials}", ctx.Credentials);

                    List list = ctx.Web.Lists.GetByTitle(SharePointList);
                    Log.Debug("Loading SharePoint List named: {listName}", SharePointList);
                    ctx.Load(list);
                    //Log.Debug("SharePoint list located named : {listName} with {count} items.", list.Title, list.ItemCount);

                    //Log.Debug("Using Execute Query on Context: {context}", ctx);
                    //ctx.ExecuteQuery();

                    FieldCollection colFields = list.Fields;
                    ctx.Load(colFields);
                    ctx.ExecuteQuery();

                    //This model can be replaced with dictionary
                    var model = new ListModel();
                    foreach (Field field in colFields)
                        if (model.GetType().GetField(field.Title) != null)
                            if (model.GetType().GetField(field.Title).GetValue(model) == null)
                                model.GetType().GetField(field.Title).SetValue(model, field.InternalName);

                    //Deletion
                    var query = new CamlQuery();
                    query.ViewXml = "<View/>";
                    ListItemCollection collListItem = list.GetItems(query);
                    ctx.Load(collListItem);
                    ctx.ExecuteQuery();

                    ListItem[] delete = collListItem.ToArray();
                    foreach (ListItem t in delete)
                        //can't delete source item directly
                        t.DeleteObject();

                    ctx.ExecuteQuery();

                    //inserting to list
                    for (var x = 1; x < 25; x++)
                    {
                        var newItem = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(newItem);
                        listItem[model.GasDay] = data.Date.ToString(CultureInfo.InvariantCulture); //string
                        listItem[model.Hour] = "HE" + x.ToString("D2"); //string
                        listItem[model.DateTime] =
                            data.Date.AddHours(x).ToString(CultureInfo.InvariantCulture); //DateTime
                        listItem[model.DA] = data.DA; //string
                        listItem[model.MMCF] = data.GetType().GetField("HE" + x.ToString("D2")).GetValue(data); //float
                        listItem.Update();
                        ctx.ExecuteQuery();
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var response = (HttpWebResponse) e.Response;
                    Log.Error(e, "Error encountered writing GasDay data to SharePoint.");
                    Log.Error(e, response.StatusCode + ": " + response.StatusDescription);
                }
            }
            catch (Exception e)
            { 
                Log.Error(e, "Error encountered writing GasDay data to SharePoint.");
            }
        }
    }

    public static class BootStrapper
    {
        public static void Configure(string scid)
        {
            CertificateFactory.Register("CMRI",
                new X509CertificateStoreCertificateFactory("CN=" + scid + ", OU=people, O=CAISO, C=US"));
        }
    }
}