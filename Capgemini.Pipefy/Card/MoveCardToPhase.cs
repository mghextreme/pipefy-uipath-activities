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
        private const string MoveCardToPhaseQuery = "mutation {{ moveCardToPhase(input: {{ card_id: {0} destination_phase_id: {1} }}){{ card {{ id current_phase {{ id name done }} done title }} }} }}";

        [Category("Input")]
        [Description("ID of the Card to be moved")]
        [RequiredArgument]
        public InArgument<long> CardID { get; set; }

        [Category("Input")]
        [Description("ID of the destination Phase")]
        [RequiredArgument]
        public InArgument<long> PhaseID { get; set; }

        public override string SuccessMessage => "Moved";

        protected override string GetQuery(CodeActivityContext context)
        {
            long card = CardID.Get(context);
            long phase = PhaseID.Get(context);

            return string.Format(string.Format(MoveCardToPhaseQuery, CardID, PhaseID));
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            
        }
    }
}