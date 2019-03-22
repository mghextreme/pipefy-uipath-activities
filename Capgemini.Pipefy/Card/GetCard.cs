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
        private const string GetCardQuery = "query { card(id: {0}){ age assignees { email id name } checklist_items_checked_count checklist_items_count comments_count created_at created_by { email id name } current_phase { id name done } current_phase_age done due_date expiration { expiredAt shouldExpireAt } expired fields { name value array_value datetime_value filled_at } finished_at id labels { name id } late phases_history { duration firstTimeIn lastTimeOut phase { id name } } pipe { id name } started_current_phase_at title updated_at url } }";

        [Category("Input")]
        [Description("ID of the Card to be obtained")]
        [RequiredArgument]
        public InArgument<long> CardID { get; set; }

        [Category("Output")]
        [Description("Card obtained (JObject)")]
        public OutArgument<JObject> Card { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var cardId = CardID.Get(context);
            return string.Format(GetCardQuery, cardId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var card = json["card"] as JObject;
            Card.Set(context, card);
        }
    }
}