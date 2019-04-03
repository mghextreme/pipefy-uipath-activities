using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Creates a card in Pipefy.
    /// </summary>
    [Description("Updates a card in Pipefy.")]
    public class UpdateCard : PipefyQueryActivity
    {
        private const string UpdateCardQuery = "mutation {{ updateCard(input: {{ id: {0} {1} }}){{ card {{ id updated_at }} }} }}";

        [Category("Input")]
        [Description("ID of the Card to be updated")]
        [RequiredArgument]
        public InArgument<long> CardID { get; set; }

        [Category("Input")]
        [Description("ID of the users to be assigned to the card")]
        public InArgument<long[]> AssigneeIDs { get; set; }

        [Category("Input")]
        [Description("The card due date")]
        public InArgument<DateTime> DueDate { get; set; }

        [Category("Input")]
        [Description("ID of the labels to be set to the card")]
        public InArgument<long[]> LabelIDs { get; set; }

        [Category("Input")]
        [Description("The title of the Card")]
        public InArgument<string> Title { get; set; }

        public override string SuccessMessage => "Updated";

        protected override string GetQuery(CodeActivityContext context)
        {
            var card = CardID.Get(context);
            var assignees = AssigneeIDs.Get(context);
            var dueDate = DueDate.Get(context);
            var labels = LabelIDs.Get(context);
            var title = Title.Get(context);

            var cardFields = new List<string>();

            if (assignees?.Length > 0)
                cardFields.Add(string.Format("assignee_ids: {0}", assignees.ToQueryValue()));

            if (dueDate != null && dueDate != DateTime.MinValue)
                cardFields.Add(string.Format("due_date: {0}", dueDate.ToQueryValue()));

            if (labels?.Length > 0)
                cardFields.Add(string.Format("label_ids: {0}", labels.ToQueryValue()));

            if (!string.IsNullOrWhiteSpace(title))
                cardFields.Add(string.Format("title: {0}", title.ToQueryValue()));

            var paramsStr = string.Join(" ", cardFields);

            return string.Format(UpdateCardQuery, card, paramsStr);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            
        }
    }
}