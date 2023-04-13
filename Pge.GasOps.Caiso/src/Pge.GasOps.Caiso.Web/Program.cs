// // Copyright (c) 2018 Pacific Gas and Electric Company. All rights reserved.

using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Pge.GasOps.Caiso.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", false)
                .Build();

            return WebHost
                .CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseApplicationInsights()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseSerilog((webHostBuilderContext, loggerConfiguration) =>
                    loggerConfiguration
                        .ReadFrom.Configuration(webHostBuilderContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console());
        }
    }
}