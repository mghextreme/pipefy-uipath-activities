using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// A generic class for running a Pipefy API Query
    /// </summary>
    public class PipefyQuery
    {
        private const string PipefyApiUrl = "https://app.pipefy.com/queries";

        private WebRequest request;

        public string Query { get; protected set; }

        public HttpStatusCode StatusCode { get; protected set; }

        public string Method { get; protected set; }

        public PipefyQuery()
        {
            request = WebRequest.Create(PipefyApiUrl);
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Timeout = 120000;
            request.ContentType = "application/json";
            request.Method = "POST";
        }

        public PipefyQuery(string query) : this()
        {
            SetQuery(query);
        }

        public PipefyQuery(string query, string authorization) : this(query)
        {
            SetAuthorization(authorization);
        }

        public PipefyQuery(string query, string authorization, string method) : this(query, authorization)
        {
            SetMethod(method);
        }

        public string Execute()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Query);
            request.ContentLength = bytes.Length;
            Stream os = request.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            StatusCode = resp.StatusCode;
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd();
        }

        public void SetAuthorization(string bearer)
        {
            if (request == null)
                return;

            request.Headers.Add("Authorization", "Bearer " + bearer);
        }

        public void SetQuery(string query)
        {
            var jObj = new JObject();
            jObj["query"] = query;
            Query = jObj.ToString();
        }

        private void SetMethod(string method)
        {
            if (request == null)
                return;

            method = method.ToUpper();
            switch (method)
            {
                case "GET":
                case "POST":
                case "PUT":
                case "OPTIONS":
                    request.Method = method;
                    break;
            }
            throw new ArgumentException(string.Format("The method {0} is unsupported", method));
        }

        public void SetTimeout(int timeout)
        {
            if (request == null)
                return;

            request.Timeout = timeout;
        }
    }
}