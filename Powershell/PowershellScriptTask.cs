using Taskmatics.TestComponents.ProcessExecution;

namespace Taskmatics.TestComponents.Powershell
{
    public class PowershellScriptTask : ExecuteProcessTask
    {
        protected override string GetProcessFileName()
        {
            return "powershell";
        }

        protected override string GetArguments()
        {
            var parameters = (PowershellScriptTaskInputParameters)Context.Parameters;
            return "-NonInteractive -NoLogo -NoProfile -WindowStyle Hidden " + parameters.Script;
        }
    }
}
