using System;
using System.Activities;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    public abstract class PipefyQueryActivity : CodeActivity
    {
        [Category("Input")]
        [Description("The user API Key")]
        [RequiredArgument]
        public InArgument<string> Bearer { get; set; }

        [Category("Input")]
        [Description("The request timeout (ms)")]
        [DefaultValue(120000)]
        public InArgument<int> Timeout { get; set; }

        [Category("Output")]
        [Description("The status of the transaction")]
        public OutArgument<string> Status { get; set; }

        [Category("Output")]
        [Description("True if the action was successful")]
        public OutArgument<bool> Success { get; set; }

        public virtual string SuccessMessage => "Success";

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                SafeExecute(context);

                Status.Set(context, SuccessMessage);
                Success.Set(context, true);
            }
            catch (Exception)
            {
                Status.Set(context, "Failed");
                Success.Set(context, false);
                throw;
            }
        }

        protected virtual void SafeExecute(CodeActivityContext context)
        {
            string bearer = Bearer.Get(context);
            var queryText = GetQuery(context);
            var query = new PipefyQuery(queryText, bearer);

            int timeout = Timeout.Get(context);
            query.SetTimeout(timeout);
            
            string result = query.Execute();
            CheckStatusCode(query.StatusCode);

            JObject json = ParseJson(result);
            ParseResult(context, json["data"] as JObject);
        }

        protected abstract string GetQuery(CodeActivityContext context);

        protected void CheckStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode != HttpStatusCode.OK &&
                statusCode != HttpStatusCode.Created &&
                statusCode != HttpStatusCode.Accepted)
                throw new PipefyException("Web request returned Status Code " + statusCode);
        }

        protected JObject ParseJson(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new PipefyException("The response received was empty.");

            var json = JObject.Parse(result);

            JArray jaErrors = json["errors"] as JArray;
            if (jaErrors != null)
            {
                if (jaErrors.Count > 0)
                {
                    string errorMessage = jaErrors.Count + " error(s) found:";
                    foreach (var error in jaErrors)
                        errorMessage += "\n- " + error.Value<string>("message");
                    throw new PipefyException(errorMessage);
                }
            }

            return json;
        }

        protected abstract void ParseResult(CodeActivityContext context, JObject json);
    }
}