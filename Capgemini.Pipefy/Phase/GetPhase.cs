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
        private const string GetPhaseQuery = "query {{ phase(id: {0}){{ cards_can_be_moved_to_phases {{ id name done }} cards_count description done expiredCardsCount fields {{ description id is_multiple label required type }} id lateCardsCount name }} }}";

        [Category("Input")]
        [Description("ID of the Phase to be obtained")]
        [RequiredArgument]
        public InArgument<long> PhaseID { get; set; }

        [Category("Output")]
        [Description("Phase obtained (JObject)")]
        public OutArgument<JObject> Phase { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var phaseId = PhaseID.Get(context);
            return string.Format(GetPhaseQuery, phaseId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var phase = json["phase"] as JObject;
            Phase.Set(context, phase);
        }
    }
}