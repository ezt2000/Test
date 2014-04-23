using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskmatics.TestComponents.ProcessExecution;

namespace Taskmatics.TestComponents.CommandLine
{
    public class CommandLineTask : ExecuteProcessTask
    {
        protected override string GetProcessFileName()
        {
            return "cmd";
        }

        protected override string GetArguments()
        {
            var parameters = (CommandLineTaskInputParameters)Context.Parameters;
            var builder = new StringBuilder(" /c ");
            builder.Append(parameters.Command);

            if (!String.IsNullOrEmpty(parameters.Arguments))
                builder.AppendFormat(" {0}", parameters.Arguments);

            return builder.ToString();
        }
    }
}
