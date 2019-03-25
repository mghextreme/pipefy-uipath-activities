using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Creates a card in Pipefy.
    /// </summary>
    [Description("Updates a field in a card in Pipefy.")]
    public class UpdateCardField : PipefyQueryActivity
    {
        private const string UpdateCardFieldQuery = "mutation {{ updateCardField(input: {{ card_id: {0} field_id: \"{1}\" new_value: {2} }}){{ card {{ id updated_at }} success }} }}";

        [Category("Input")]
        [Description("ID of the Card to be updated")]
        [RequiredArgument]
        public InArgument<long> CardID { get; set; }

        [Category("Input")]
        [Description("ID of the field to be updated")]
        [RequiredArgument]
        public InArgument<string> FieldID { get; set; }

        [Category("Input")]
        [Description("Value to be placed at the field")]
        [RequiredArgument]
        public InArgument<object> Value { get; set; }

        public override string SuccessMessage => "Updated";

        protected override string GetQuery(CodeActivityContext context)
        {
            long id = CardID.Get(context);
            string fieldId = FieldID.Get(context);
            object fieldValue = Value.Get(context);

            return string.Format(UpdateCardFieldQuery, id, fieldId, fieldValue.ToQueryValue());
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            bool success = json["updateCardField"].Value<bool>("success");
            if (!success)
                throw new PipefyException("Couldn't update card field");
        }
    }
}