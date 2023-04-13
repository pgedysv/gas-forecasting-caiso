using System.Web;
using System.Web.Mvc;

namespace Pge.GasOps.Egen.Caiso.Web.Net
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}
