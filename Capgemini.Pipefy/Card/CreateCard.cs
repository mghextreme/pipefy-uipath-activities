using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Card
{
    /// <summary>
    /// Creates a card in Pipefy.
    /// </summary>
    [Description("Creates a card in Pipefy.")]
    public class CreateCard : PipefyQueryActivity
    {
        private const string CreateCardQuery = "mutation {{ createCard(input: {{ pipe_id: {0} {1} {2} }}) {3} }}";
        private const string CreateCardFieldQueryPart = "{{ field_id: \"{0}\" field_value: \"{1}\" }}";
        private const string CreateCardReturnQuery = "{ card { createdAt createdBy { email id name } current_phase { id name } due_date expiration { shouldExpireAt } id url } }";

        [Category("Input")]
        [Description("ID of the users to be assigned to the card")]
        public InArgument<long[]> AssigneeIDs { get; set; }

        [Category("Input")]
        [Description("The card due date")]
        public InArgument<DateTime> DueDate { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for the card (Dictionary)")]
        [OverloadGroup("Dictionary input")]
        public InArgument<Dictionary<string, object>> DictionaryFields { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for the card (DataRow)")]
        [OverloadGroup("DataRow input")]
        public InArgument<DataRow> DataRowFields { get; set; }

        [Category("Input")]
        [Description("ID of the labels to be added to the card")]
        public InArgument<long[]> LabelIDs { get; set; }

        [Category("Input")]
        [Description("ID of the parent Cards")]
        public InArgument<long[]> ParentsIDs { get; set; }

        [Category("Input")]
        [Description("ID of the Phase in which the Card should be placed")]
        [DefaultValue(-1)]
        public InArgument<long> PhaseID { get; set; }

        [Category("Input")]
        [Description("ID of the Pipe in which the Card should be created")]
        [RequiredArgument]
        public InArgument<long> PipeID { get; set; }

        [Category("Input")]
        [Description("The title of the Card")]
        [RequiredArgument]
        public InArgument<string> Title { get; set; }

        [Category("Output")]
        [Description("The created Card summary (JObject)")]
        public OutArgument<JObject> Card { get; set; }

        [Category("Output")]
        [Description("ID of the created Card")]
        public OutArgument<long> CardID { get; set; }

        public override string SuccessMessage => "Created";

        protected override string GetQuery(CodeActivityContext context)
        {
            var pipe = PipeID.Get(context);

            string cardFieldsString = GetCardFieldsQuery(context);
            string customFields = GetCardCustomFieldsQuery(context);
            return string.Format(CreateCardQuery, pipe, cardFieldsString, customFields, CreateCardReturnQuery);
        }

        private string GetCardFieldsQuery(CodeActivityContext context)
        {
            var assignees = AssigneeIDs.Get(context);
            var dueDate = DueDate.Get(context);
            var labels = LabelIDs.Get(context);
            var parents = ParentsIDs.Get(context);
            var phase = PhaseID.Get(context);
            var title = Title.Get(context);

            var cardFields = new List<string>();

            if (assignees?.Length > 0)
            {
                cardFields.Add(string.Format("assignee_ids: {0}", assignees.ToQueryValue()));
            }

            if (dueDate != null && dueDate != DateTime.MinValue)
            {
                cardFields.Add(string.Format("due_date: {0}", dueDate.ToQueryValue()));
            }

            if (labels?.Length > 0)
            {
                cardFields.Add(string.Format("label_ids: {0}", labels.ToQueryValue()));
            }

            if (parents?.Length > 0)
            {
                cardFields.Add(string.Format("parent_ids: {0}", parents.ToQueryValue()));
            }

            if (phase > 0)
            {
                cardFields.Add(string.Format("phase_id: \"{0}\"", phase));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                cardFields.Add(string.Format("title: {0}", title.ToQueryValue()));
            }

            return string.Join(" ", cardFields);
        }

        private string GetCardCustomFieldsQuery(CodeActivityContext context)
        {
            var dict = DictionaryFields.Get(context);
            if (dict == null || dict.Count == 0)
            {
                var dataRow = DataRowFields.Get(context);
                var tempDict = dataRow.ToDictionary();
                if (tempDict != null && tempDict.Count > 0)
                    dict = tempDict;
                else
                    return string.Empty;
            }

            var cardCustomFields = new List<string>();

            foreach (var item in dict)
            {
                string value = item.Value.ToString();
                value = value.ToQueryValue();
                cardCustomFields.Add(string.Format(CreateCardFieldQueryPart, item.Key, value));
            }

            string cardCustomFieldsStr = string.Join(", ", cardCustomFields);
            return string.Format("fields_attributes: [ {0} ]", cardCustomFieldsStr);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var card = json["createCard"]["card"] as JObject;
            Card.Set(context, card);
            CardID.Set(context, card.Value<long>("id"));
        }
    }
}