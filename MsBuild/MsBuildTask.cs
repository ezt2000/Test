using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskmatics.Scheduler.Core;
using Taskmatics.TestComponents.ProcessExecution;

namespace Taskmatics.TestComponents.MsBuild
{
    public class MsBuildTask : ExecuteProcessTask
    {
        private readonly MsBuildTaskInputParameters _parameters;

        public MsBuildTask()
        {
            _parameters = (MsBuildTaskInputParameters)Context.Parameters;
        }

        protected override string GetProcessFileName()
        {
            if (!String.IsNullOrEmpty(_parameters.MsBuildExecutablePath))
                return _parameters.MsBuildExecutablePath;

            return "msbuild";
        }

        protected override string GetArguments()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("\"{0}\"", _parameters.SolutionOrProjectFilePath);
            builder.Append(" /nologo /m /t:Rebuild");

            if (!String.IsNullOrEmpty(_parameters.Configuration))
                builder.Append(" /p:Configuration=" + _parameters.Configuration);

            if (!String.IsNullOrEmpty(_parameters.Platform))
                builder.Append(" /p:Platform=" + _parameters.Platform);

            return builder.ToString();
        }
    }
}
