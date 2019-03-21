using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Creates a card in Pipefy.
    /// </summary>
    [Description("Moves a card to another phase in Pipefy.")]
    public class MoveCardToPhase : PipefyQueryActivity
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