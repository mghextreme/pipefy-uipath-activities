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
        protected override string GetQuery(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            throw new NotImplementedException();
        }
    }
}