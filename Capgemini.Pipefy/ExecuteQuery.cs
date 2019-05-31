using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// Executes a custom query.
    /// </summary>
    [Description("Executes a custom query.")]
    public class ExecuteQuery : PipefyQueryActivity
    {
        [Category("Input")]
        [Description("The query to be executed")]
        [RequiredArgument]
        public InArgument<string> Query { get; set; }

        [Category("Output")]
        [Description("The response from the query (JObject)")]
        public OutArgument<JObject> Result { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var query = Query.Get(context);

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("The input must contain a non-empty query.");

            return query;
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            Result.Set(context, json);
        }
    }
}