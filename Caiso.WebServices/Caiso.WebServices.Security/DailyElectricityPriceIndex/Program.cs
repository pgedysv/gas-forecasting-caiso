using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using DailyElectricityPriceIndex.electricity;
using Caiso.WebServices;

namespace CaisoWebServiceTest
{
    class Program
    {
        // Change this to your SC ID
        private const string SCID = "JERRY VINx7728";

        static void Main(string[] args)
        {
            BootStrapper.Configure(SCID);
            RequestSummary();
        }

        private static void RequestSummary()
        {
            //var url = "https://wsmap.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP";
            var url = "https://ws.caiso.com/sst/cmri/RetrieveDailyElectricityPriceIndex_CMRIv1_DocAttach_AP";
            var factory = new WSProxyFactory<retrieveDailyElectricityPriceIndex_v1_DocAttachClient, retrieveDailyElectricityPriceIndex_v1_DocAttach>();
            var proxy = factory.CreateMTOMProxy(url, "CMRI");

            var request = new RequestDailyElectricityPriceIndex();
            request.MessageHeader = new MessageHeader()
            {
                Source = "CMRI",
                TimeDate = DateTime.Now.ToUniversalTime(),
                Version = "v20171001"
            };

            //request.MessagePayload = new MessagePayload()
            //{
            //    RequestDailyElectricityPriceIndexRecord = new RequestDailyElectricityPriceIndexRecord()
            //    {
            //        dateTimeStart = DateTime.Parse("2018-01-13"),
            //        dateTimeEnd = DateTime.Parse("2018-01-14"),
            //        marketType = MarketType.DAM,
            //        SchedulingCoordinatorList = new SchedulingCoordinatorList[1]
            //        {
            //            new SchedulingCoordinatorList()
            //            {
            //                schedulingCoordinator = "27425"
            //            }
            //        }
            //    }
            //};
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RequestDailyElectricityPriceIndex));
            StringWriter sw = new StringWriter();
            var tw = new XmlTextWriter(sw);
            xmlSerializer.Serialize(tw, request);
            Console.WriteLine(sw.ToString());
            sw.Close();
            tw.Close();
            Console.WriteLine();

            try
            {
                ISOAttachment att = new ISOAttachment();
                var req = new retrieveDailyElectricityPriceIndex_v1_DocAttachRequest()
                {
                    RequestDailyElectricityPriceIndex = request
                };

                var uselessDoc = proxy.retrieveDailyElectricityPriceIndex_v1_DocAttach(request, out att);
                //att returns base64 encoded zipped string

                var decoded = Convert.FromBase64String(att.AttachmentValue);
                var dstream = new MemoryStream(decoded, false);
                using (var gZipStream = new System.IO.Compression.GZipStream(dstream, System.IO.Compression.CompressionMode.Decompress))
                {
                    var sr = new StreamReader(gZipStream);
                    sr.ReadLine();
                    var stuff = sr.ReadLine();
                    Console.WriteLine(stuff);

                    XmlSerializer serializer = new XmlSerializer(typeof(outputDataType));
                    using (TextReader reader = new StringReader(stuff))
                    {
                        var x = (outputDataType)serializer.Deserialize(reader);
                    
                        StringWriter sw2 = new StringWriter();
                        var tw2 = new XmlTextWriter(sw2);
                        serializer.Serialize(tw2, x);
                        Console.WriteLine(sw2.ToString());
                        sw2.Close();
                        tw2.Close();
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
