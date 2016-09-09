using System;
using System.Collections.Generic;

namespace ServiceFramework.Http.Models
{
    public class ProcessModel
    {
        public string Name { get; set; }
        public string ProcessType { get; set; }
        public string ConfigurationType { get; set; }
        public bool IsBusy { get; set; }
        public int ActiveProcesses { get; set; }
        public List<WorkerModel> Workers { get; set; }
    }
}