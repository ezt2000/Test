﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Taskmatics.Scheduler.Core;

namespace Taskmatics.TestComponents.Http
{
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
        }

        protected virtual Task SendResponseAsync(object requestContent, HttpListenerContext context)
        {
            return CompletedTask;
        }

        protected virtual dynamic GetContent(HttpListenerRequest request)
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
}
