using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Pipe
{
    /// <summary>
    /// Gets detailes information on a Pipe.
    /// </summary>
    [Description("Gets detailes information on a Pipe.")]
    public class GetPipe : PipefyQueryActivity
    {
        private const string GetPipeQuery = "query {{ pipe(id: {0}){{ anyone_can_create_card cards_count description id labels {{ id name }} name opened_cards_count phases {{ cards_count id name done }} public title_field {{ id }} }} }}";

        [Category("Input")]
        [Description("ID of the Pipe to be obtained")]
        [RequiredArgument]
        public InArgument<long> PipeID { get; set; }

        [Category("Output")]
        [Description("Pipe obtained (JObject)")]
        public OutArgument<JObject> Pipe { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var pipeId = PipeID.Get(context);
            return string.Format(GetPipeQuery, pipeId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var pipe = json["pipe"] as JObject;
            Pipe.Set(context, pipe);
        }
    }
}