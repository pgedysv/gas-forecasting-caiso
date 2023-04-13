using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using CMRItoSharepoint.GasBurnSummary;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.SharePoint.Client;
using Caiso.WebServices;

namespace CMRItoSharepoint
{
    //TODO: Currently pulling calendar day. Need to pull GasDay
    //TODO: Schedule program execution; 2nd half of next gasday(12AM - 7AM) not available until 5pm of current day
    //TODO: Find out calculation used to generate value on PG&E
    //TODO: Implement list backups in case shit hits the fan
        //TODO: Also implement error catching
    //TODO: Implement logging

    //TODO: if using NET core; find out if sharepoint 2013 sdk, Microsoft.SharePointOnline.CSOM , CAISO service references,


    class Program
    {
        // Change this to your SC ID
        private const string SCID = "JERRY VINx7728";
        private const string spUrl = "http://clean.dstcontrols.local";
        private const string subsite = "pge";
        private const string targetList = "CMRI";

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now.Date;
            DateTime endTime = startTime.AddDays(1);

            BootStrapper.Configure(SCID);
            var data = RequestSummary(startTime, endTime, GasBurnSummary.MarketType.DAM);
            var x = ConvertToGasDayObject(data);
            SendToSharepoint(x);
        }

        //add start, end, market type to parameters
        private static GasBurnSummaryData RequestSummary(DateTime start, DateTime end, GasBurnSummary.MarketType marketType)
        {
            GasBurnSummaryData data = new GasBurnSummaryData();
            outputDataType error;

            var url = "https://ws.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP";

            //black box of signing certificate
            var factory = new WSProxyFactory<retrieveGasBurnSummaryData_v1_DocAttachClient, retrieveGasBurnSummaryData_v1_DocAttach>();
            var proxy = factory.CreateMTOMProxy(url, "CMRI");

            var request = new RequestGasBurnSummaryData();
            request.MessageHeader = new GasBurnSummary.MessageHeader()
            {
                Source = "DSTPGE",
                TimeDate = DateTime.Now.ToUniversalTime(),
                Version = "v20171001"
            };

            request.MessagePayload = new GasBurnSummary.MessagePayload()
            {
                RequestGasBurnSummaryRecord = new RequestGasBurnSummaryRecord()
                {
                    GasCompanyList = new GasBurnSummary.GasCompanyList[1] { new GasBurnSummary.GasCompanyList() { gasCompany = "PGAE" } },
                    GasZoneList = new GasBurnSummary.GasZone[3]
                    {
                        new GasBurnSummary.GasZone()
                        {
                            name = "PGAE 300",
                            summaryLevel = GasBurnSummary.GasZoneSummaryLevel.FZONE
                        },
                        new GasBurnSummary.GasZone()
                        {
                            name = "PGAE 400/401",
                            summaryLevel = GasBurnSummary.GasZoneSummaryLevel.FZONE
                        },
                        new GasBurnSummary.GasZone()
                        {
                            name = "NOT SPECIFIED",
                            summaryLevel = GasBurnSummary.GasZoneSummaryLevel.FZONE
                        }
                    },
                    dateTimeStart = start,
                    dateTimeEnd = end,
                    marketType = marketType,
                    executionType = GasBurnSummary.PTRExecutionType.RUC
                }
            };

            //for debugging request message
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RequestGasBurnSummaryData));
            StringWriter sw2 = new StringWriter();
            var tw2 = new XmlTextWriter(sw2);
            xmlSerializer.Serialize(tw2, request);
            Console.WriteLine(sw2.ToString());
            sw2.Close();
            tw2.Close();
            Console.WriteLine();

            try
            {
                GasBurnSummary.ISOAttachment att = new GasBurnSummary.ISOAttachment();
                var req = new retrieveGasBurnSummaryData_v1_DocAttachRequest()
                {
                    RequestGasBurnSummaryData = request
                };

                //call to api
                var uselessDoc = proxy.retrieveGasBurnSummaryData_v1_DocAttach(request, out att);
                //att returns base64 encoded zipped string

                var decoded = Convert.FromBase64String(att.AttachmentValue);
                var dstream = new MemoryStream(decoded, false);
                using (var gZipStream = new System.IO.Compression.GZipStream(dstream, System.IO.Compression.CompressionMode.Decompress))
                {
                    var sr = new StreamReader(gZipStream);
                    sr.ReadLine();
                    var stuff = sr.ReadToEnd();

                    XmlSerializer summarySerializer = new XmlSerializer(typeof(GasBurnSummaryData));
                    XmlSerializer errorSerializer = new XmlSerializer(typeof(outputDataType));

                    using (StringReader stringReader = new StringReader(stuff))
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        StringWriter sw = new StringWriter();
                        XmlTextWriter tw = new XmlTextWriter(sw);

                        if (summarySerializer.CanDeserialize(xmlReader))
                        {
                            data = (GasBurnSummaryData)summarySerializer.Deserialize(xmlReader);
                            summarySerializer.Serialize(tw, data);
                        }
                        else if (errorSerializer.CanDeserialize(xmlReader))
                        {
                            error = (outputDataType)errorSerializer.Deserialize(xmlReader);
                            errorSerializer.Serialize(tw, error);
                        }
                        else
                        {
                            Console.WriteLine("Unknown Response");
                        }

                        Console.WriteLine(sw.ToString());
                        sw.Close();
                        tw.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                proxy.Close();
            }

            return data;
        }

        private static GasDay ConvertToGasDayObject(GasBurnSummaryData data)
        {
            //TODO: Handle Daylight Savings
            //currently return timestamps in local time;

            GasBurnSummaryRecord[] records = data.MessagePayload;
            //Change GasDay to Array; Add NumberOfDays parameter
            GasDay gasDay = new GasDay(DateTime.Now.Date);
            GasDay gasDay2 = new GasDay(DateTime.Now.Date.AddDays(1));
            foreach (var record in records)
            {
                var endtime = record.TradeIntervalData[0].intervalEndTime;
                var value = record.TradeIntervalData[0].gasBurn;
                var endingHour = endtime.Hour == 0 ? 24 : endtime.Hour;
                var property = "HE" + endingHour.ToString("D2");
                if (record.marketStartTime.Date == gasDay.Date.Date)
                {
                    var prevValue = (float)gasDay.GetType().GetField(property).GetValue(gasDay);
                    gasDay.GetType().GetField(property).SetValue(gasDay, value + prevValue);
                }
                else
                {
                    gasDay2.GetType().GetField(property).SetValue(gasDay2, value);
                }
            }

            return gasDay;
        }

        //Import Reference: Microsoft.Sharepoint.Client (and Client.Runtime) v15 for sharepoint 2013
        //google sharepoint 2013 sdk
        private static void SendToSharepoint(GasDay data)
        {
            

            ClientContext clientContext = new ClientContext(spUrl);
            Web oWeb = clientContext.Site.OpenWeb(subsite);
            clientContext.Load(oWeb);
            clientContext.ExecuteQuery();

            var list = oWeb.Lists.GetByTitle(targetList);
            clientContext.Load(list);
            clientContext.ExecuteQuery();

            FieldCollection colFields = list.Fields;
            clientContext.Load(colFields);
            clientContext.ExecuteQuery();

            //This model can be replaced with dictionary
                //If Dictionary, need to implement which columns/fields we want
            ListModel model = new ListModel();
            foreach (var field in colFields)
            {
                if (model.GetType().GetField(field.Title) != null)
                {
                    if (model.GetType().GetField(field.Title).GetValue(model) == null)
                        model.GetType().GetField(field.Title).SetValue(model, field.InternalName);
                }
            }

            //Deletion
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View/>";
            ListItemCollection collListItem = list.GetItems(query);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            ListItem[] delete = new ListItem[collListItem.Count];
            ListItem[] backup = new ListItem[collListItem.Count];
            backup = collListItem.ToArray();
            delete = collListItem.ToArray();
            for (var x = 0; x < delete.Length; x++)
            {
                //can't delete source item directly
                delete[x].DeleteObject();
            }
            clientContext.ExecuteQuery();

            //inserting to list
            for (var x = 1; x < 25; x++)
            {
                ListItemCreationInformation newItem = new ListItemCreationInformation();
                ListItem listItem = list.AddItem(newItem);
                listItem[model.GasDay] = data.Date.ToString(); //string
                listItem[model.Hour] = "HE" + x.ToString("D2"); //string
                listItem[model.DateTime] = data.Date.AddHours(x).ToString(); //DateTime
                listItem[model.DA] = data.DA; //string
                listItem[model.MMCF] = data.GetType().GetField("HE" + x.ToString("D2")).GetValue(data); //float
                listItem.Update();
                clientContext.ExecuteQuery();
            }
        }
    }

    public class BootStrapper
    {
        public static void Configure(string SCID)
        {
            CertificateFactory.Register("CMRI", new X509CertificateStoreCertificateFactory("CN=" + SCID + ", OU=people, O=CAISO, C=US"));
        }
    }
}
