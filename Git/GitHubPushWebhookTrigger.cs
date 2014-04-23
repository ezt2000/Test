using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Taskmatics.Scheduler.Core;
using Taskmatics.TestComponents.Http;

namespace Taskmatics.TestComponents.Git
{
    [OutputParameters(typeof(GitHubPushWebhookTriggerOutputParameters))]
    public class GitHubPushWebhookTrigger : HttpEndpointTrigger
    {
        protected override Task ProcessRequestAsync(HttpListenerContext context)
        {
            var content = GetContent(context.Request);
            context.Response.Close();
            OnFired(new GitHubPushWebhookTriggerOutputParameters { RepositoryUrl = content.repository.url });

            return CompletedTask;
        }
    }
}
