using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Taskmatics.Scheduler.Core;

namespace Taskmatics.TestComponents
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new HttpEndpointTriggerInputParameters();
            parameters.Url = "http://+:5000/test";

            var harness = new TriggerHarness<HttpEndpointTrigger>(parameters);
            harness.TriggerFired += (s, e) => Console.WriteLine((e.OutputParameters as HttpEndpointTriggerOutputParameters).Message);

            Console.ReadLine();
        }
    }

    [InputParameters(typeof(HttpEndpointTriggerInputParameters))]
    [OutputParameters(typeof(HttpEndpointTriggerOutputParameters))]
    public class HttpEndpointTrigger : Trigger
    {
        private readonly TaskCompletionSource<object> _cancellation;
        private readonly HttpListener _httpListener;

        public HttpEndpointTrigger()
        {
            var parameters = (HttpEndpointTriggerInputParameters)Context.Parameters;

            _cancellation = new TaskCompletionSource<object>();

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(parameters.Url.TrimEnd('/') + '/');
            _httpListener.Start();

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
            catch
            {

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
                    var content = await new StreamReader(context.Request.InputStream).ReadToEndAsync();

                    context.Response.Close();

                    OnFired(new HttpEndpointTriggerOutputParameters { Message = content });
                }
                catch
                {

                }
            });
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
        public string Message { get; set; }
    }
}
