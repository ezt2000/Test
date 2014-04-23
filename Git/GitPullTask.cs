using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskmatics.Scheduler.Core;

namespace Taskmatics.TestComponents.Git
{
    public class GitPullTask : TaskBase
    {
        private readonly string _workingDirectoryPath;
        private readonly string _repositoryUrl;

        public GitPullTask()
        {
            var parameters = (GitPullTaskInputParameters)Context.Parameters;

            _workingDirectoryPath = parameters.WorkingDirectoryPath;
            if (!String.IsNullOrEmpty(parameters.RepositoryUrl))
                _repositoryUrl = parameters.RepositoryUrl;
            else
            {
                var triggerOutputParameters = (GitHubPushWebhookTriggerOutputParameters)Context.TriggerInfo.OutputParameters;
                _repositoryUrl = triggerOutputParameters.RepositoryUrl;
            }
        }

        protected override void Execute()
        {
            if (!Directory.Exists(_workingDirectoryPath))
                Directory.CreateDirectory(_workingDirectoryPath);

            if (!Repository.IsValid(_workingDirectoryPath))
                Repository.Clone(
                    _repositoryUrl,
                    _workingDirectoryPath,
                    onCheckoutProgress: ReportCheckoutProgress,
                    onTransferProgress: HandleTransferProgress);

            var repository = new Repository(_workingDirectoryPath);
            var remote = repository.Network.Remotes.FirstOrDefault(r =>
                String.Equals(
                r.Url.TrimEnd('/'),
                _repositoryUrl.TrimEnd('/'),
                StringComparison.InvariantCultureIgnoreCase));

            if (remote == null)
                throw new InvalidOperationException("Remote with URL '" + _repositoryUrl + "' does not exist in the repo within '" + _workingDirectoryPath + "'.");

            repository.Fetch(
                remote.Name,
                new FetchOptions
                {
                    OnProgress = ReportProgress,
                    OnTransferProgress = HandleTransferProgress
                });

            var commit = repository.Branches["origin/master"].Commits.FirstOrDefault();
            if (commit != null)
                repository.Merge(commit, commit.Author);
        }

        private void ReportCheckoutProgress(string path, int completed, int total)
        {
            Context.Logger.Log("{0} of {1}: {2}", completed, total, path);
        }

        private bool ReportProgress(string message)
        {
            Context.Logger.Log(message);
            return true;
        }

        private bool HandleTransferProgress(TransferProgress progress)
        {
            return true;
        }
    }
}
