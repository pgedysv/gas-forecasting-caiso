using Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary;
using Pge.GasOps.EGen.Cmri.Core.Entities.StandardOutput;
using Pge.GasOps.EGen.Cmri.Core.Utilities;
using Pge.GasOps.EGen.Cmri.Infrastructure.CmriGasBurnSummaryService;
using Pge.GasOps.EGen.Cmri.Infrastructure.Data.WebServices;

using Serilog;

using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

using MarketType = Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary.MarketType;
using PTRExecutionType = Pge.GasOps.EGen.Cmri.Core.Entities.GasBurnSummary.PTRExecutionType;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Data.GasBurnSummaries
{
    public static class GasBurnSummaryService
    {

        public static GasBurnSummaryData GetGasBurnSummaryData(
            DateTime start, 
            DateTime end, 
            MarketType marketType = MarketType.DAM, 
            PTRExecutionType executionType = PTRExecutionType.IFM)
        {
            GasBurnSummaryData response = new GasBurnSummaryData();
            OutputDataType error = new OutputDataType();

            const string url = "https://ws.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP";

            //black box of signing certificate
            var factory = new WebServiceProxyFactory<retrieveGasBurnSummaryData_v1_DocAttachClient, retrieveGasBurnSummaryData_v1_DocAttach>();

            retrieveGasBurnSummaryData_v1_DocAttachClient proxy = factory.CreateMtomProxy(url, "CMRI");

            RequestGasBurnSummaryData request = new RequestGasBurnSummaryData
            {
                MessageHeader = new CmriGasBurnSummaryService.MessageHeader
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
                        GasZoneList = new CmriGasBurnSummaryService.GasZone[1]
                        {
                            new CmriGasBurnSummaryService.GasZone
                            {
                                name = "PGAE",
                                summaryLevel = CmriGasBurnSummaryService.GasZoneSummaryLevel.GASCO
                            }
                        },
                        dateTimeStart = start,
                        dateTimeEnd = end,
                        marketType = (CmriGasBurnSummaryService.MarketType)marketType,
                        executionType = (CmriGasBurnSummaryService.PTRExecutionType)executionType
                    }
                }
            };

            //for debugging request message
            Log.Verbose(Utilities.SerializeObjectToXml(request));

            try
            {
                var req = new retrieveGasBurnSummaryData_v1_DocAttachRequest {RequestGasBurnSummaryData = request};

                //call to api - att returns base64 encoded zipped string

                proxy.retrieveGasBurnSummaryData_v1_DocAttach(request, out ISOAttachment att);
                
                byte[] decoded = Convert.FromBase64String(att.AttachmentValue);
                using (MemoryStream memoryStream = new MemoryStream(decoded, false))
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        string isoAttachmentString;
                        using (StreamReader stringReader = new StreamReader(gZipStream))
                        {
                            isoAttachmentString = stringReader.ReadToEnd();
                        }

                        Log.Information(isoAttachmentString);

                        XmlSerializer summarySerializer = new XmlSerializer(response.GetType());
                        XmlSerializer errorSerializer = new XmlSerializer(error.GetType());

                        using (StringReader stringReader = new StringReader(isoAttachmentString))
                        {
                            using (XmlReader xmlReader = XmlReader.Create(stringReader))
                            {
                                using (StringWriter stringWriter = new StringWriter())
                                {
                                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                                    {
                                        if (summarySerializer.CanDeserialize(xmlReader))
                                        {
                                            response = (GasBurnSummaryData)summarySerializer.Deserialize(xmlReader);
                                            summarySerializer.Serialize(xmlWriter, response);
                                        }
                                        else if (errorSerializer.CanDeserialize(xmlReader))
                                        {
                                            error = (OutputDataType)errorSerializer.Deserialize(xmlReader);
                                            errorSerializer.Serialize(xmlWriter, error);
                                        }
                                        else
                                        {
                                            Log.Warning("GasBurnSummaryRecords were retrieved, but an unknown error occurred during XML deserialization.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed while trying to parse RequestGasBurnSummaryData.");
                return response;
            }
            finally
            {
                proxy.Close();
            }
        }

    }
}
