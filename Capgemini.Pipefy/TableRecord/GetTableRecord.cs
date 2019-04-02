using System.Activities;
using System.Collections.Generic;
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
        private const string GetTableRecordQuery = "query {{ table_record(id: {0}){{ created_at created_by {{ id name email }} due_date finished_at id status {{ id name }} record_fields {{ name value array_value datetime_value field {{ id type }} }} table {{ id name }} title updated_at url }} }}";

        [Category("Input")]
        [Description("TableRecord ID to be obtained")]
        [RequiredArgument]
        public InArgument<long> TableRecordID { get; set; }

        [Category("Output")]
        [Description("TableRecord obtained (JObject)")]
        public OutArgument<JObject> TableRecord { get; set; }

        [Category("Output")]
        [Description("Card fields in Dictionary form")]
        public OutArgument<Dictionary<string, object>> TableRecordFieldsDictionary { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            long recordId = TableRecordID.Get(context);
            return string.Format(GetTableRecordQuery, recordId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var record = json["table_record"] as JObject;
            TableRecord.Set(context, record);

            var fieldsJson = record["record_fields"] as JArray;
            var fieldsDict = PipefyExtensions.FieldsJArrayToDictionary(fieldsJson);
            TableRecordFieldsDictionary.Set(context, fieldsDict);
        }
    }
}