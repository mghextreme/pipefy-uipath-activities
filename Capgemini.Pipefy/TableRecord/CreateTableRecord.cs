using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.TableRecord
{
    /// <summary>
    /// Creates a TableRecord in a Pipefy Table.
    /// </summary>
    [Description("Creates a TableRecord in a Pipefy Table.")]
    public class CreateTableRecord : PipefyQueryActivity
    {
        private const string CreateTableRecordQuery = "mutation {{ createTableRecord(input: {{ table_id: \"{0}\" title: {1} due_date: {2} fields_attributes: [ {3} ] }}) {{ table_record {{ id title due_date record_fields {{ name value }} }} }} }}";
        private const string TableRecordFieldQueryPart = "{{ field_id: \"{0}\", field_value: {1} }}";

        [Category("Input")]
        [Description("Table ID to have an item created")]
        [RequiredArgument]
        public InArgument<string> TableID { get; set; }

        [Category("Input")]
        [Description("TableRecord Title (defaults to first field value)")]
        public InArgument<string> Title { get; set; }

        [Category("Input")]
        [Description("TableRecord Due Date (defaults to one month from now)")]
        public InArgument<DateTime> DueDate { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for record (Dictionary)")]
        [OverloadGroup("Dictionary input")]
        public InArgument<Dictionary<string, object>> DictionaryFields { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for record (DataRow)")]
        [OverloadGroup("DataRow input")]
        public InArgument<DataRow> DataRowFields { get; set; }

        [Category("Output")]
        [Description("The ID of the created TableRecord")]
        [RequiredArgument]
        public OutArgument<long> TableRecordID { get; set; }

        public override string SuccessMessage => "Created";

        protected override string GetQuery(CodeActivityContext context)
        {
            string tableId = TableID.Get(context);
            DateTime dueDate = DueDate.Get(context);
            string title = Title.Get(context);

            if (dueDate == null || dueDate == DateTime.MinValue)
                dueDate = DateTime.Now.AddMonths(1);

            var dict = DictionaryFields.Get(context);
            if (dict == null || dict.Count == 0)
            {
                var dataRow = DataRowFields.Get(context);
                var tempDict = dataRow.ToDictionary();
                if (tempDict != null && tempDict.Count > 0)
                    dict = tempDict;
            }

            if (string.IsNullOrWhiteSpace(title))
                title = dict.First().Value.ToString();

            return BuildQuery(tableId, title, dict, dueDate);
        }

        public static string BuildQuery(string tableId, string title, Dictionary<string, object> customFields, DateTime dueDate)
        {
            string fieldsString = string.Empty;
            if (customFields.Count > 0)
            {
                List<string> fields = new List<string>();
                foreach (var item in customFields)
                {
                    string value = item.Value.ToQueryValue();
                    fields.Add(string.Format(TableRecordFieldQueryPart, item.Key, value));
                }
                fieldsString = string.Join(", ", fields);
            }

            return string.Format(CreateTableRecordQuery, tableId, title.ToQueryValue(), dueDate.ToQueryValue(), fieldsString);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            JObject joResult = json["createTableRecord"] as JObject;
            JObject joTableRecord = joResult["table_record"] as JObject;
            long idPipefy = joTableRecord.Value<long>("id");
            TableRecordID.Set(context, idPipefy);
        }
    }
}