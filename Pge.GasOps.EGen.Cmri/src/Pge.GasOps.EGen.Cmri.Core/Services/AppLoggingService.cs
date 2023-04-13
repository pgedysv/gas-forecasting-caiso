using Microsoft.Extensions.Logging;

namespace Pge.GasOps.EGen.Cmri.Core.Services
{
    public static class AppLoggingService
    {
        private static ILoggerFactory _Factory = null;

	    public static void ConfigureLogger(ILoggerFactory factory)
	    {
		    //factory.AddDebug(LogLevel.Trace);
		    //factory.AddConsole(LogLevel.Trace);
		    //factory.AddFile("logFileFromHelper.log"); //serilog file extension
	    }

        public static ILoggerFactory LoggerFactory
	    {
		    get
		    {
			    if (_Factory == null)
			    {
				    _Factory = new LoggerFactory();
				    ConfigureLogger(_Factory);
			    }
			    return _Factory;
		    }
		    set { _Factory = value; }
	    }
        public static ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }
    }
}
