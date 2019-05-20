using System;
using System.Activities;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// Base class for other Pipefy Activities containing the base structure for the development
    /// </summary>
    public abstract class PipefyQueryActivity : CodeActivity
    {
        /// <summary>
        /// The Personal Access Token
        /// </summary>
        [Category("Input")]
        [Description("The Personal Access Token")]
        [RequiredArgument]
        public InArgument<string> Bearer { get; set; }

        /// <summary>
        /// The request timeout (ms)
        /// </summary>
        [Category("Input")]
        [Description("The request timeout (ms)")]
        [DefaultValue(120000)]
        public InArgument<int> Timeout { get; set; }

        /// <summary>
        /// The resulting status of the transaction
        /// </summary>
        [Category("Output")]
        [Description("The status of the transaction")]
        public OutArgument<string> Status { get; set; }

        /// <summary>
        /// True if the action was successful
        /// </summary>
        [Category("Output")]
        [Description("True if the action was successful")]
        public OutArgument<bool> Success { get; set; }

        /// <summary>
        /// The message returned in Status in case of success
        /// </summary>
        public virtual string SuccessMessage => "Success";

        /// <summary>
        /// Executes the Activity
        /// </summary>
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

        /// <summary>
        /// Only the main part of the execution. Will be run inside of a try / catch statement
        /// </summary>
        protected virtual void SafeExecute(CodeActivityContext context)
        {
            string bearer = Bearer.Get(context);
            var queryText = GetQuery(context);
            var query = new PipefyQuery(queryText, bearer);

            int timeout = Timeout.Get(context);
            if (timeout > 100)
                query.SetTimeout(timeout);
            
            string result = query.Execute();
            CheckStatusCode(query.StatusCode);

            JObject json = PipefyQuery.ParseJson(result);
            ParseResult(context, json["data"] as JObject);
        }

        /// <summary>
        /// Gets the query to be executed based on the arguments
        /// </summary>
        /// <returns>The query to be executed</returns>
        protected abstract string GetQuery(CodeActivityContext context);

        /// <summary>
        /// Checks if the requests resulting status code is within the accepted options
        /// </summary>
        /// <param name="statusCode"></param>
        protected void CheckStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode != HttpStatusCode.OK &&
                statusCode != HttpStatusCode.Created &&
                statusCode != HttpStatusCode.Accepted)
                throw new PipefyException("Web request returned Status Code " + statusCode);
        }

        /// <summary>
        /// Parses the resulting JObject for relevant information
        /// </summary>
        /// <param name="context">The activity context</param>
        /// <param name="json">The data JObject</param>
        protected abstract void ParseResult(CodeActivityContext context, JObject json);
    }
}