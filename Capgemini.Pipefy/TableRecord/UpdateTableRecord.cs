using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.TableRecord
{
    /// <summary>
    /// Updates a TableRecord.
    /// </summary>
    [Description("Updates a TableRecord.")]
    public class UpdateTableRecord : PipefyQueryActivity
    {
        private const string UpdateTableRecordQuery = @"mutation {{ updateTableRecord(input: {{ id: ""{0}"" {1} {2} {3} }){ table_record { id updated_at } } }";

        [Category("Input")]
        [Description("TableRecord ID to be obtained")]
        [RequiredArgument]
        public InArgument<long> TableRecordID { get; set; }

        [Category("Input")]
        [Description("The new title for the TableRecord")]
        public InArgument<string> Title { get; set; }

        [Category("Input")]
        [Description("The new due date for the TableRecord")]
        public InArgument<DateTime> DueDate { get; set; }

        [Category("Input")]
        [Description("The ID of the new status for the TableRecord")]
        public InArgument<string> StatusID { get; set; }

        public override string SuccessMessage => "Updated";

        protected override string GetQuery(CodeActivityContext context)
        {
            long recordId = TableRecordID.Get(context);
            string title = Title.Get(context);
            DateTime dueDate = DueDate.Get(context);
            string statusId = StatusID.Get(context);

            string titleStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(title))
                titleStr = string.Format("title: \"{0}\"", title.EscapeQueryValue());

            string dueDateStr = string.Empty;
            if (dueDate != null && dueDate != DateTime.MinValue)
                dueDateStr = string.Format("due_date: \"{0}\"", dueDate.ToPipefyFormat());

            string statusStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(statusId))
                statusStr = string.Format("statusId: \"{0}\"", statusId);

            return string.Format(UpdateTableRecordQuery, recordId, titleStr, dueDateStr, statusStr);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {

        }
    }
}