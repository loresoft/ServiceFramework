using System;
using System.Linq;
using System.Web.Http;
using ServiceFramework.Http.Models;

namespace ServiceFramework.Http
{
    public class ServiceController : ApiController
    {
        private readonly IServiceRuntime _serviceRuntime;

        public ServiceController(IServiceRuntime serviceRuntime)
        {
            _serviceRuntime = serviceRuntime;
        }

        [HttpGet]
        [Route("api/Service")]
        public IHttpActionResult Get()
        {
            // copy to model
            var model = new ServiceModel
            {
                Name = _serviceRuntime.Name,
                ActiveProcesses = _serviceRuntime.ActiveProcesses,
                IsBusy = _serviceRuntime.IsBusy,
                Processes = _serviceRuntime.Processors
                    .Select(process => new ProcessModel
                    {
                        Name = process.Name,
                        ProcessType = process.GetType().FullName,
                        ConfigurationType = process.Configuration.GetType().FullName,
                        IsBusy = process.IsBusy,
                        ActiveProcesses = process.ActiveProcesses,
                        Workers = process.Workers
                        .Select(worker => new WorkerModel
                        {
                            Name = worker.Name,
                            Status = worker.Status,
                            WorkerType = worker.GetType().FullName,
                            ConfigurationType = worker.Configuration.GetType().FullName,
                            IsBusy = worker.IsBusy,
                            ActiveProcesses = process.ActiveProcesses
                        })
                        .ToList()
                    })
                    .ToList()
            };

            return Ok(model);
        }
    }
}
