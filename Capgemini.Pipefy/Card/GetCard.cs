using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Gets detailes information on a Card.
    /// </summary>
    [Description("Gets detailes information on a Card.")]
    public class GetCard : PipefyQueryActivity
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