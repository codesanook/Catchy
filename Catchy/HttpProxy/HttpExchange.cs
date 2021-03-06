﻿using System;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace Catchy.HttpProxy
{
    /// <summary>
    /// Our model of a single request and corresponding response.
    /// This class exposes Titanium Proxy's <see cref="SessionEventArgs"/> properties and
    /// methods on an interface for easier testing / mocking.
    /// </summary>
    public class HttpExchange : IHttpExchange
    {
        private readonly SessionEventArgs session;

        public HttpExchange(SessionEventArgs session)
        {
            this.session = session;
        }

        public string RequestMethod => session.HttpClient.Request.Method;
        public Uri RequestUrl => session.HttpClient.Request.RequestUri;
        public HeaderCollection RequestHeaders => session.HttpClient.Request.Headers;
        public Task<string> GetRequestBody() => session.GetRequestBodyAsString();

        public async Task<Response> GetResponse()
        {
            var response = session.HttpClient.Response;
            response.KeepBody = true; // keep the response body around after the request/response is finished
            if (response.ContentLength > 0) // GetResponseBody will throw if there's no content
            {
                _ = await session.GetResponseBody(); // force the body to be read, so we can access it later
            }
            return response;
        }

        public object? UserData
        {
            get => session.UserData;
            set => session.UserData = value;
        }

        public void Respond(Response response) =>
            session.Respond(response);
    }

    /// <summary>
    /// Our model of a single request and corresponding response.
    /// </summary>
    public interface IHttpExchange
    {
        string RequestMethod { get; }
        Uri RequestUrl { get; }
        HeaderCollection RequestHeaders { get; }
        Task<string> GetRequestBody();
        Task<Response> GetResponse();
        object? UserData { get; set; }

        void Respond(Response response);
    }
}
