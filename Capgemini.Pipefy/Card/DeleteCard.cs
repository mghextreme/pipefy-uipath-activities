using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Creates a card in Pipefy.
    /// </summary>
    [Description("Deletes a card in Pipefy.")]
    public class DeleteCard : PipefyQueryActivity
    {
        private const string DeleteCardQuery = @"mutation {{ deleteCard(input: {{ id: {0} }}){{ success }} }}";

        [Category("Input")]
        [Description("ID of the Card to be deleted")]
        [RequiredArgument]
        public InArgument<long> CardID { get; set; }

        public override string SuccessMessage => "Deleted";

        protected override string GetQuery(CodeActivityContext context)
        {
            long cardId = CardID.Get(context);
            return string.Format(DeleteCardQuery, cardId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            bool success = json["deleteCard"].Value<bool>("success");
            if (!success)
                throw new PipefyException("Couldn't delete card");
        }
    }
}