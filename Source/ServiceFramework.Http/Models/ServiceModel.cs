using System;
using System.Collections.Generic;

namespace ServiceFramework.Http.Models
{
    public class ServiceModel
    {
        public string Name { get; set; }
        public bool IsBusy { get; set; }
        public int ActiveProcesses { get; set; }
        public List<ProcessModel> Processes { get; set; }
    }
}