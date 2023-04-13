using System;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.SharePoint.Client;
using Pge.GasOps.EGen.Cmri.Core.Entities;
using Serilog;

namespace Pge.GasOps.EGen.Cmri.Infrastructure.Services
{
    public static class SharePointServices
    {
        public static ListModel GetListModel(FieldCollection fields)
        {
            ListModel model = new ListModel();
            foreach (Field field in fields)
                if (model.GetType().GetField(field.Title) != null)
                    if (model.GetType().GetField(field.Title).GetValue(model) == null)
                        model.GetType().GetField(field.Title).SetValue(model, field.InternalName);

            return model;
        }

        public static ListItemCollection GetAllListItems(ClientContext ctx, List list)
        {
            var query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection allListItems = list.GetItems(query);
            ctx.Load(allListItems);
            ctx.ExecuteQuery();

            return allListItems;
        }

        public static string DeleteSharePointItems(string url, string listName, NetworkCredential credential, DateTime deleteBefore)
        {
            string message;
            string dateCompareString = deleteBefore.ToString("yyyy-MM-dd'T'HH:mm:ss");

            try
            {
                using (var ctx = new ClientContext(url))
                {
                    ctx.Credentials = credential;
                    List list = ctx.Web.Lists.GetByTitle(listName);
                    ctx.Load(list);
                    ctx.Load(list.Fields);
                    ctx.ExecuteQuery();
                    ListModel model = GetListModel(list.Fields);
 
                    // Get all items before deleteBefore
                    var query = new CamlQuery
                    {
                        ViewXml = $@"
<View>
  <Query>
    <Where>
      <Lt>
        <FieldRef Name='{model.TradeIntervalStartTime}' />
        <Value Type='DateTime'>{dateCompareString}</Value>
      </Lt>
    </Where>
  </Query>
</View>"
                    };
                    ListItemCollection collListItem = list.GetItems(query);
                    Log.Information(query.ToString());
                    ctx.Load(collListItem);
                    ctx.ExecuteQuery();

                    int count = 0;
                    int totalDeleted = 0;
                    int totalQueried = collListItem.Count;
                    foreach (ListItem t in collListItem.ToArray())
                    {
                        // can't delete source item directly
                        t.DeleteObject();
                        count++;
                        totalDeleted++;
                        if (count >= 199)
                        {
                            ctx.ExecuteQuery();
                            count = 0;
                        }
                    }

                    ctx.ExecuteQuery();
                    message = $"success - total returned from query: {totalQueried} - total deleted: {totalDeleted}.  ";
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var response = (HttpWebResponse)e.Response;
                    Log.Error(e, "Error encountered deleting GasDay data from SharePoint.");
                    Log.Error(e, response.StatusCode + ": " + response.StatusDescription);
                }

                message = "failure";
            }
            catch (Exception e)
            {
                Log.Error(e, "Error encountered deleting GasDay data from SharePoint.");
                message = "failure";
            }

            return message;
        }

        public static string
            DeleteSharePointItemsBeforeToday(string url, string listName, NetworkCredential credential) =>
            DeleteSharePointItems(url, listName, credential, DateTime.Today);

        public static string SendToSharePoint(string url, string listName, NetworkCredential credential, MarketPeriod data, string marketType)
        {
            string message;
            StringBuilder logMessage = new StringBuilder();

            try
            {
                using (var ctx = new ClientContext(url))
                {
                    ctx.Credentials = credential;
                    List list = ctx.Web.Lists.GetByTitle(listName);
                    ctx.Load(list);
                    FieldCollection colFields = list.Fields;
                    ctx.Load(colFields);
                    ctx.ExecuteQuery();

                    ListItemCollection allListItems = GetAllListItems(ctx, list);

                    // This model can be replaced with dictionary
                    ListModel model = GetListModel(colFields);

                    // inserting to or updating list
                    int countExistedUpdated = 0;
                    int countExistedNotUpdated = 0;
                    int countNewItem = 0;
                    foreach (TradeInterval tradeInterval in data.TradeIntervals.Values)
                    {
                        DateTime tradeIntervalStartTime = tradeInterval.TradeIntervalStartTime;
                        float mmcf = tradeInterval.MMCF;

                        ListItem existingItem = allListItems.SingleOrDefault(listItem =>
                            (DateTime) listItem[model.TradeIntervalStartTime] == tradeIntervalStartTime.ToUniversalTime() &&
                            (string) listItem[model.DA] == marketType);

                        bool mmcfAvailable = existingItem != null && (double) existingItem[model.MMCF] > -1;

                        double oldMMCF = mmcfAvailable ? (double) existingItem[model.MMCF] : 0;
                        bool mmcfEqual = mmcfAvailable ? oldMMCF.ToString() == mmcf.ToString() : false;


                        if (mmcfAvailable && !mmcfEqual)
                        {
                            logMessage.AppendLine($"{tradeIntervalStartTime} - {marketType}: Combination existed, old value: {oldMMCF}, new value: {mmcf}");
                            existingItem[model.MMCF] = mmcf;
                            existingItem.Update();

                            countExistedUpdated++;
                        }
                        else if (mmcfAvailable && mmcfEqual)
                        {
                            logMessage.AppendLine($"{tradeIntervalStartTime} - {marketType}: Combination existed, mmcf matches, no update necessary");
                            countExistedNotUpdated++;
                        }
                        else
                        {
                            logMessage.AppendLine($"{tradeIntervalStartTime} - {marketType}: Combination did not exist, creating new list item - mmcf: {mmcf}");
                            var newItem = new ListItemCreationInformation();
                            ListItem listItem = list.AddItem(newItem);
                            listItem[model.TradeIntervalStartTime] = tradeInterval.TradeIntervalStartTime;
                            listItem[model.TradeIntervalEndTime] = tradeInterval.TradeIntervalEndTime;
                            listItem[model.MMCF] = tradeInterval.MMCF; //float
                            listItem[model.DA] = marketType;
                            listItem.Update();

                            countNewItem++;
                        }
                    }
                    ctx.ExecuteQuery();

                    message = $"success - total updated: {countExistedUpdated} - total no update necessary: {countExistedNotUpdated} - total new items: {countNewItem}.  ";
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var response = (HttpWebResponse)e.Response;
                    Log.Error(e, "Error encountered writing GasDay data to SharePoint.");
                    Log.Error(e, response.StatusCode + ": " + response.StatusDescription);
                }

                logMessage.AppendLine("Error writing GasDay data to SharePoint - HTTP Error");
                message = "failure";
            }
            catch (Exception e)
            {
                Log.Error(e, "Error encountered writing GasDay data to SharePoint.");
                logMessage.AppendLine($"Error writing GasDay data to SharePoint - {e}");
                message = "failure";
            }

            Log.Information(logMessage.ToString());

            return message;
        }

    }
}
