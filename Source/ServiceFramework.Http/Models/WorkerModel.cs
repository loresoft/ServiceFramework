using System;

namespace ServiceFramework.Http.Models
{
    public class WorkerModel
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string WorkerType { get; set; }
        public string ConfigurationType { get; set; }
        public bool IsBusy { get; set; }
        public int ActiveProcesses { get; set; }
    }
}