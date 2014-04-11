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

namespace Taskmatics.TestComponents
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"c:\users\easy\desktop\testrepo";

            //Directory.CreateDirectory(path);

            if (!Repository.IsValid(path))
                Repository.Init(path);

            var repo = new Repository(path);
            if (repo.Network.Remotes["origin"] == null)
                repo.Network.Remotes.Add("origin", "https://github.com/ezt2000/Test");

            repo.Network.Fetch(repo.Network.Remotes["origin"]/*, new FetchOptions
            {
                Credentials = new Credentials { Username = "ezt2000", Password = "Password.1" }
            }*/);

            repo.Merge(repo.Commits.First(), null);

            //var parameters = new HttpEndpointTriggerInputParameters();
            //parameters.Url = "http://+:5000/test";

            //var harness = new TriggerHarness<GitHubPushWebhookTrigger>(parameters);
            //harness.TriggerFired += (s, e) => Console.WriteLine((e.OutputParameters as GitHubPushWebhookTriggerOutputParameters).RepoUrl);

            Console.ReadLine();
        }
    }

    [InputParameters(typeof(HttpEndpointTriggerInputParameters))]
    [OutputParameters(typeof(HttpEndpointTriggerOutputParameters))]
    public class HttpEndpointTrigger : Trigger
    {
        private readonly TaskCompletionSource<object> _cancellation;
        private readonly HttpListener _httpListener;
        protected readonly Task CompletedTask;

        public HttpEndpointTrigger()
        {
            var parameters = (HttpEndpointTriggerInputParameters)Context.Parameters;

            _cancellation = new TaskCompletionSource<object>();

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(parameters.Url.TrimEnd('/') + '/');
            _httpListener.Start();

            CompletedTask = Task.FromResult<object>(null);

            HandleRequestsAsync();
        }

        private async void HandleRequestsAsync()
        {
            try
            {
                while (true)
                {
                    var task = await Task.WhenAny(_httpListener.GetContextAsync(), _cancellation.Task);
                    if (task == _cancellation.Task)
                        break;

                    var context = ((Task<HttpListenerContext>)task).Result;
                    HandleRequestAsync(context);
                }
            }
            catch (Exception ex)
            {
                OnHandleError(ex, isTerminal: true);
            }
            finally
            {
                _httpListener.Close();
            }
        }

        private void HandleRequestAsync(HttpListenerContext context)
        {
            Task.Run(async () =>
            {
                try
                {
                    await ProcessRequestAsync(context);
                }
                catch (Exception ex)
                {
                    OnHandleError(ex);
                }
            });
        }

        protected async virtual Task ProcessRequestAsync(HttpListenerContext context)
        {
            var content = GetContent(context.Request);
            await SendResponseAsync(content, context);
            context.Response.Close();
            OnFired(new HttpEndpointTriggerOutputParameters { Content = content });
            Trace.WriteLine(content);
        }

        protected virtual Task SendResponseAsync(object requestContent, HttpListenerContext context)
        {
            return CompletedTask;
        }

        protected virtual object GetContent(HttpListenerRequest request)
        {
            if (request.ContentType.IndexOf("xml") > -1)
                return XElement.Load(request.InputStream);

            if (request.ContentType.IndexOf("json") > -1) 
                return JToken.Load(new JsonTextReader(new StreamReader(request.InputStream)));

            throw new InvalidOperationException("Unrecognized content type.");
        }

        protected virtual void OnHandleError(Exception ex, bool isTerminal = false)
        {
            Trace.WriteLine(ex.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
            _cancellation.SetResult(null);
        }
    }

    public class HttpEndpointTriggerInputParameters
    {
        public string Url { get; set; }
    }

    public class HttpEndpointTriggerOutputParameters
    {
        public object Content { get; set; }
    }

    [OutputParameters(typeof(GitHubPushWebhookTriggerOutputParameters))]
    public class GitHubPushWebhookTrigger : HttpEndpointTrigger
    {
        protected override Task ProcessRequestAsync(HttpListenerContext context)
        {
            var content = (dynamic)GetContent(context.Request);
            context.Response.Close();
            OnFired(new GitHubPushWebhookTriggerOutputParameters { RepoUrl = content.repository.url });

            return CompletedTask;
        }
    }

    public class GitHubPushWebhookTriggerOutputParameters
    {
        public string RepoUrl { get; set; }
    }

    public class PullRepoTask : TaskBase
    {
        private readonly string _repoUrl;

        public PullRepoTask()
        {
            var output = (GitHubPushWebhookTriggerOutputParameters)Context.TriggerInfo.OutputParameters;
            _repoUrl = output.RepoUrl;
        }

        protected override void Execute()
        {

        }
    }

}
