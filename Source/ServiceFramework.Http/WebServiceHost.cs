using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using ServiceFramework.Logging;

namespace ServiceFramework.Http
{
    public class WebServiceHost
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private IDisposable _webApplication;

        public WebServiceHost()
        {
            EndPoint = ConfigurationManager.AppSettings["Service.Http.Endpoint"] ?? "http://localhost:9988/";
        }

        public string EndPoint { get; set; }

        public void Start()
        {
            _logger.Info()
                .Message("HTTP Service Listening on: {0}", EndPoint)
                .Write();

            _webApplication = WebApp.Start(EndPoint, builder =>
            {
                // Configure Web API for self-host. 
                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();

                builder.UseWebApi(config);
            });
        }

        public void Stop()
        {
            _webApplication?.Dispose();
            _webApplication = null;
        }
    }
}
