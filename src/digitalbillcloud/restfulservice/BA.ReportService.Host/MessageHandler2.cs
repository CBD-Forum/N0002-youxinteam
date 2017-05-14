using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BA.ReportService.Host
{
    public class MessageHandler2 : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";


        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {

            bool isCorsRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

                    string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();

                    if (accessControlRequestMethod != null)
                    {
                        response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                       

                    }
                    string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
                    if (!string.IsNullOrEmpty(requestedHeaders))
                    {
                        response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);

                    }
                   // response.Headers.Remove("Access-Control-Allow-Origin");

                   // response.Headers.Add("Access-Control-Allow-Origin", "*");

                    TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
                    tcs.SetResult(response);
                    return tcs.Task;
                }
                else
                {
                    
                    return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(t =>
                    {
                        HttpResponseMessage resp = t.Result;
                       // resp.Headers.Add("Access-Control-Allow-Origin", "*");

                        resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                        return resp;
                    });
                    

                    /*
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        
                    };
                    response.Headers.Add("Access-Control-Allow-Origin", "*");
                    // Note: TaskCompletionSource creates a task that does not contain a delegate.
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);   // Also sets the task state to "RanToCompletion"
                    return tsc.Task;
                    */
                }
            }
            else
            {
                string pathAndQuery = request.RequestUri.PathAndQuery.ToLower();

                if (pathAndQuery != "/clientaccesspolicy.xml")
                    return base.SendAsync(request, cancellationToken);


                string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                            <access-policy>
                                  <cross-domain-access>
                                        <policy>
                                              <allow-from>
                                                    <domain uri='*' />
                                              </allow-from>
                                              <grant-to>
                                                    <resource path='/' include-subpaths='true' />
                                              </grant-to>
                                        </policy>
                                  </cross-domain-access>
                            </access-policy>";

                // Create the response.
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(xml)
                };

                // Note: TaskCompletionSource creates a task that does not contain a delegate.
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);   // Also sets the task state to "RanToCompletion"
                return tsc.Task;
            }









        }
    }
}
