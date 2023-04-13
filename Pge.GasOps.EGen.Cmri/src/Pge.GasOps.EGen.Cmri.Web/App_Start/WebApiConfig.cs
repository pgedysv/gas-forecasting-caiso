using System.Web.Http;

namespace Pge.GasOps.EGen.Cmri.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GasBurnSummary",
                routeTemplate: "api/caiso/cmri/gasburnsummary/{startTime}",
                defaults: new { controller = "GasBurnSummary", startTime = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
