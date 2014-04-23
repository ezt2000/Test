using System.IO;
using Taskmatics.Scheduler.Core;
using Taskmatics.TestComponents.ProcessExecution;

namespace Taskmatics.TestComponents.Nodejs
{
    [InputParameters(typeof(NodeScriptTaskInputParameters))]
    public class NodeScriptTask : ExecuteProcessTask
    {
        protected override string GetProcessFileName()
        {
            return "node";
        }

        protected override string GetArguments()
        {
            // Copy script text to a temporary file, and return file path as the argument.

            var parameters = (NodeScriptTaskInputParameters)Context.Parameters;
            var tempScriptFilePath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".js"));

            File.WriteAllText(tempScriptFilePath, parameters.Script);

            return "\"" + tempScriptFilePath + "\"";
        }
    }
}
