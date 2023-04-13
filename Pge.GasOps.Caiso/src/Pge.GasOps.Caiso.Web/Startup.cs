// // Copyright (c) 2018 Pacific Gas and Electric Company. All rights reserved.

using System;
using System.Text;
using Autofac;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Pge.GasOps.Caiso.Web
{
    // ReSharper disable once ClassNeverInstantiated.Global - invalid error for Startup class.
    public class Startup
    {
        private IServiceCollection services;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SnapshotCollectorConfiguration>(
                Configuration.GetSection(nameof(SnapshotCollectorConfiguration)));

            services.AddSingleton<ITelemetryProcessorFactory>(sp => new SnapshotCollectorTelemetryProcessorFactory(sp));

            services.AddOptions();

            // Add services to the collection.
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    // Lambda required for GDRP, determines whether user consent
                    // for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://wolverine", 
                                "http://wolverine.dstcontrols.local", 
                                "https://sps.utility.pge.com"
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });


            this.services = services;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            Log.Debug("ConfigureContainer() Start.");

            // builder.RegisterModule(new AutofacModule());
            //builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

            Log.Debug("ConfigureContainer() End.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
            }

            Log.Debug("Dev mode: {isDev}", env.IsDevelopment());

            ListAllRegisteredServices(app);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            //app.UseMvc();



            //3.1 Upgrade


            app.UseStaticFiles();

            app.UseRouting();

            // The equivalent of 'app.UseMvcWithDefaultRoute()'
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                // Which is the same as the template
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            Log.Debug("ListAllRegisteredServices() Start.");

            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (ServiceDescriptor svc in services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString())
                    .ConfigureAwait(false);
            }));

            Log.Debug("ListAllRegisteredServices() End.");
        }

        private class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
        {
            private readonly IServiceProvider serviceProvider;

            public SnapshotCollectorTelemetryProcessorFactory(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            public ITelemetryProcessor Create(ITelemetryProcessor next)
            {
                var snapshotConfigurationOptions =
                    serviceProvider.GetService<IOptions<SnapshotCollectorConfiguration>>();
                return new SnapshotCollectorTelemetryProcessor(next, snapshotConfigurationOptions.Value);
            }
        }
    }
}