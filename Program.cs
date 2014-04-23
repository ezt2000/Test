using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Taskmatics.Scheduler.Core;
using Taskmatics.TestComponents.CommandLine;
using Taskmatics.TestComponents.MsBuild;
using Taskmatics.TestComponents.Nodejs;
using Taskmatics.TestComponents.Powershell;

namespace Taskmatics.TestComponents
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new CommandLineTaskInputParameters();
            parameters.Command = "dir";
            parameters.Arguments = @"""C:\Users\easy\Documents\Visual Studio 2013\Projects\AsyncReadTest\""";

            var harness = new TaskHarness<CommandLineTask>(parameters);
            harness.Execute();

            //var parameters = new MsBuildTaskInputParameters();
            //parameters.MsBuildExecutablePath = @"C:\Program Files (x86)\MSBuild\12.0\Bin\amd64\msbuild.exe";
            //parameters.SolutionOrProjectFilePath = @"C:\Users\easy\Documents\Visual Studio 2013\Projects\AsyncReadTest\AsyncReadTest.sln";

            //var harness = new TaskHarness<MsBuildTask>(parameters);
            //harness.Execute();

            //var parameters = new NodeScriptTaskInputParameters();
            //parameters.Script = "console.log(process.cwd());";

            //var harness = new TaskHarness<NodeScriptTask>(parameters);
            //harness.Execute();

            //var parameters = new PowershellScriptTaskInputParameters();
            //parameters.Script = "ls -recurse";

            //var harness = new TaskHarness<PowershellScriptTask>(parameters);
            //harness.Execute();

            //var info = new ProcessStartInfo("powershell");
            //info.Arguments = "-NonInteractive -NoLogo -NoProfile -WindowStyle Hidden get-command -name get-command";
            //info.RedirectStandardOutput = true;
            //info.RedirectStandardError = true;
            //info.UseShellExecute = false;
            //info.CreateNoWindow = true;

            //var process = new Process();
            //process.StartInfo = info;
            //process.Start();
            //process.WaitForExit();

            //var output = process.StandardOutput.ReadToEnd();
            //var error = process.StandardError.ReadToEnd();

            //Console.WriteLine(output);
            //Console.WriteLine(error);

            //var path = @"c:\users\easy\desktop\testrepo";
            //var parameters = new GitPullTaskInputParameters();
            //parameters.WorkingDirectoryPath = path;
            //parameters.RepositoryUrl = "https://github.com/ezt2000/Test";

            //var harness = new TaskHarness<GitPullTask>(parameters);
            //harness.Execute();

            Console.ReadLine();
        }
    }
}
