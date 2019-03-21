using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.TableRecord
{
    /// <summary>
    /// Gets detailed information about a TableRecord in a Pipefy Table.
    /// </summary>
    [Description("Gets detailed information about a TableRecord in a Pipefy Table.")]
    public class GetTableRecord : PipefyQueryActivity
    {
        private const string GetTableRecordQuery = "table_record(id: {0}){{ created_at created_by {{ id name email }} due_date finished_at id status {{ id name }} record_fields {{ name value }} table {{ id name }} title updated_at url }}";

        [Category("Input")]
        [Description("TableRecord ID to be obtained")]
        [RequiredArgument]
        public InArgument<long> TableRecordID { get; set; }

        [Category("Output")]
        [Description("TableRecord obtained (JObject)")]
        public InArgument<JObject> TableRecord { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            long recordId = TableRecordID.Get(context);
            return string.Format(GetTableRecordQuery, recordId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var record = json["data"]["table_record"] as JObject;
            TableRecord.Set(context, record);
        }
    }
}