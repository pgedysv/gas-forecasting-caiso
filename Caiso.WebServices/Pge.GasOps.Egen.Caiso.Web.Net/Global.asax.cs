using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Slack.Core;
using Serilog.Sinks.ApplicationInsights;

namespace Pge.GasOps.Egen.Caiso.Web.Net
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
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

    }
}
