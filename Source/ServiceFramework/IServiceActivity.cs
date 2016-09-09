using System;

namespace ServiceFramework
{
    public interface IServiceActivity
    {
        bool IsBusy { get; }
        int ActiveProcesses { get; }

        IDisposable BeginWork();
        void EndWork();
    }
}