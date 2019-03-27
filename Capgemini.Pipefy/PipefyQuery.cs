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

        /// <summary>
        /// The query to be executed
        /// </summary>
        public string Query { get; protected set; }

        /// <summary>
        /// The resulting StatusCode of the request
        /// </summary>
        public HttpStatusCode StatusCode { get; protected set; }

        /// <summary>
        /// Instances a PipefyQuery with no Query or Authorization token
        /// </summary>
        public PipefyQuery()
        {
            request = WebRequest.Create(PipefyApiUrl);
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Timeout = 120000;
            request.ContentType = "application/json";
            request.Method = "POST";
        }

        /// <summary>
        /// Instances a PipefyQuery with no Authorization token
        /// </summary>
        public PipefyQuery(string query) : this()
        {
            SetQuery(query);
        }

        /// <summary>
        /// Instances a PipefyQuery
        /// </summary>
        public PipefyQuery(string query, string authorization) : this(query)
        {
            SetAuthorization(authorization);
        }

        /// <summary>
        /// Executes the query in Pipefy API
        /// </summary>
        /// <returns>The string returned by the API</returns>
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

        /// <summary>
        /// Sets the Pipefy Bearer Authorization token. No need to add the "Bearer" before the token
        /// </summary>
        /// <param name="bearer">The Authorization token</param>
        public void SetAuthorization(string bearer)
        {
            if (request == null)
                return;

            request.Headers.Add("Authorization", "Bearer " + bearer);
        }

        /// <summary>
        /// Sets the query to be executed. May be either query or mutation
        /// </summary>
        /// <param name="query">The query to be executed</param>
        public void SetQuery(string query)
        {
            var jObj = new JObject();
            jObj["query"] = query;
            Query = jObj.ToString();
        }

        /// <summary>
        /// Sets the timeout (in milisseconds) for the request. Defaults to 120000
        /// </summary>
        /// <param name="timeout">The timeout (in ms)</param>
        public void SetTimeout(int timeout)
        {
            if (request == null)
                return;

            request.Timeout = timeout;
        }
    }
}