using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskmatics.Scheduler.Core;

namespace Taskmatics.TestComponents.ProcessExecution
{
    public abstract class ExecuteProcessTask : TaskBase
    {
        protected override void Execute()
        {
            var info = new ProcessStartInfo(GetProcessFileName());
            info.Arguments = GetArguments();
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var process = new Process();
            process.StartInfo = info;
            process.Start();
            process.OutputDataReceived += (s, e) => Log("INFO", e.Data);
            process.ErrorDataReceived += (s, e) => Log("ERROR", e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            HandleExitCode(process.ExitCode);
        }

        protected abstract string GetProcessFileName();
        protected abstract string GetArguments();

        protected virtual void Log(string type, string message)
        {
            if (message == null)
                return;

            Context.Logger.Log("{0}: {1}", type, message);
        }

        protected virtual void HandleExitCode(int exitCode)
        {
            if (exitCode == 0)
                return;

            throw new ApplicationException("The process failed with exit code " + exitCode + ".");
        }
    }
}
