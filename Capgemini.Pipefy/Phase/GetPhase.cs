using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Phase
{
    /// <summary>
    /// Gets detailed information on a Phase.
    /// </summary>
    [Description("Gets detailed information on a Phase.")]
    public class GetPhase : PipefyQueryActivity
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