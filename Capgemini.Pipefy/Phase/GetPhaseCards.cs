using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Phase
{
    /// <summary>
    /// Gets Cards present on a Phase.
    /// </summary>
    [Description("Gets Cards present on a Phase.")]
    public class GetPhaseCards : PipefyQueryActivity
    {
        private const string GetPhaseCardsQuery = "query {{ phase(id: {0}){{ cards({1}){{ edges {{ node {{ created_at done due_date expired id late title url }} }} pageInfo {{ hasNextPage endCursor }} }} }} }}";

        [Category("Input")]
        [Description("ID of the Phase to be obtained")]
        [RequiredArgument]
        public InArgument<long> PhaseID { get; set; }

        [Category("Input (Pagination)")]
        [Description("Amount to get")]
        [DefaultValue(20)]
        public InArgument<int> Amount { get; set; }

        [Category("Input (Pagination)")]
        [Description("Page cursor to continue")]
        public InArgument<string> AfterCursor { get; set; }

        [Category("Input (Filter)")]
        [Description("IDs of the assignees")]
        public InArgument<long[]> AssignedTo { get; set; }

        [Category("Input (Filter)")]
        [Description("Card IDs to ignore")]
        public InArgument<long[]> IgnoreIDs { get; set; }

        [Category("Input (Filter)")]
        [Description("IDs of the labels")]
        public InArgument<long[]> Labels { get; set; }

        [Category("Input (Filter)")]
        [Description("Title of the card")]
        public InArgument<string> Title { get; set; }

        [Category("Output")]
        [Description("Cards obtained (JObject)")]
        public OutArgument<JObject[]> Cards { get; set; }

        [Category("Output")]
        [Description("True if there are elements after the last obtained")]
        public OutArgument<bool> HasNextPage { get; set; }

        [Category("Output")]
        [Description("The cursor to be used to obtain the next page")]
        public OutArgument<string> NextPageCursor { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var phaseId = PhaseID.Get(context);
            string inputQuery = GetInputQuery(context);
            return string.Format(GetPhaseCardsQuery, phaseId, inputQuery);
        }

        private string GetInputQuery(CodeActivityContext context)
        {
            var cardsInput = new List<string>();
            var first = Amount.Get(context);
            var after = AfterCursor.Get(context);

            if (first <= 0)
                first = 20;

            cardsInput.Add("first: " + first);

            if (!string.IsNullOrWhiteSpace(after))
                cardsInput.Add(string.Format("title: {0}", after.ToQueryValue()));

            var searchFields = new List<string>();
            var assignees = AssignedTo.Get(context);
            var ignoreIds = IgnoreIDs.Get(context);
            var labels = Labels.Get(context);
            var title = Title.Get(context);

            if (assignees?.Length > 0)
                searchFields.Add(string.Format("assignee_ids: {0}", assignees.ToQueryValue()));

            if (ignoreIds?.Length > 0)
                searchFields.Add(string.Format("ignore_ids: {0}", ignoreIds.ToQueryValue()));

            if (labels?.Length > 0)
                searchFields.Add(string.Format("label_ids: {0}", labels.ToQueryValue()));

            if (!string.IsNullOrWhiteSpace(title))
                searchFields.Add(string.Format("title: {0}", title.ToQueryValue()));

            string searchFieldsStr = string.Empty;
            if (searchFields.Count > 0)
            {
                searchFieldsStr = "search { " + string.Join(" ", searchFields) + " }";
                cardsInput.Add(searchFieldsStr);
            }

            return string.Join(" ", cardsInput);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var cards = json["phase"]["cards"] as JObject;
            bool nextPage = cards["pageInfo"].Value<bool>("hasNextPage");
            HasNextPage.Set(context, nextPage);
            string nextCursor = cards["pageInfo"].Value<string>("endCursor");
            NextPageCursor.Set(context, nextCursor);

            var edges = cards["edges"] as JArray;
            var cardsList = new List<JObject>();
            foreach (var item in edges)
                cardsList.Add(item["node"] as JObject);
            Cards.Set(context, cardsList.ToArray());
        }
    }
}