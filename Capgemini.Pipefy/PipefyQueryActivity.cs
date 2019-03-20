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
        public OutArgument<int> Timeout { get; set; }

        [Category("Output")]
        [Description("The status of the transaction")]
        public OutArgument<string> Status { get; set; }

        [Category("Output")]
        [Description("True if the action was successful")]
        public OutArgument<string> Success { get; set; }
        
        public virtual string SuccessMessage => "Success";
        
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string bearer = Bearer.Get(context);
                var queryText = GetQuery(context);
                var query = new PipefyQuery(queryText, bearer);

                int timeout = Timeout.Get(context);
                query.SetTimeout(timeout);
                
                string result = query.Execute();

                if (query.StatusCode != HttpStatusCode.OK &&
                    query.StatusCode != HttpStatusCode.Created &&
                    query.StatusCode != HttpStatusCode.Accepted)
                    throw new PipefyException("Web request returned Status Code " + query.StatusCode);

                JObject json = ParseJson(result);
                ParseResult(context, json);

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

        protected abstract string GetQuery(CodeActivityContext context);

        private JObject ParseJson(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new PipefyException("The response received was empty.");

            var json = JObject.Parse(result);

            JArray jaErrors = json["errors"] as JArray;
            if (jaErrors != null)
            {
                if (jaErrors.Count > 0)
                    throw new PipefyException(jaErrors[0]["message"].ToString());
            }

            return json;
        }

        protected abstract void ParseResult(CodeActivityContext context, JObject json);
    }
}