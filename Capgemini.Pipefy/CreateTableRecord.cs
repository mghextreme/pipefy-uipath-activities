using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// Activity for creating a TableRecord in a Pipefy Table.
    /// </summary>
    [Description("Activity for creating a TableRecord in a Pipefy Table.")]
    public class CreateTableRecord : PipefyQueryActivity
    {
        private const string CreateTableRecordQuery = "mutation {{ createTableRecord(input: {{ table_id: \"{0}\" title: \"record\" due_date: \"{1}\" fields_attributes: [ {2} ] }}) {{ table_record {{ id title due_date record_fields {{ name value }} }} }} }}";
        private const string TableRecordFieldQueryPart = "{{ field_id: \"{0}\", field_value: \"{1}\" }}";

        [Category("Input")]
        [Description("Table ID to have an item created")]
        [RequiredArgument]
        public InArgument<string> TableID { get; set; }

        [Category("Input")]
        [Description("Due Date to be set (defaults to one month from now)")]
        public InArgument<DateTime> DueDate { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for record (Dictionary)")]
        [RequiredArgument]
        [OverloadGroup("Dictionary input")]
        public InArgument<Dictionary<string, object>> DictionaryFields { get; set; }

        [Category("Data Input")]
        [Description("Custom fields for record (DataRow)")]
        [RequiredArgument]
        [OverloadGroup("DataRow input")]
        public InArgument<DataRow> DataRowFields { get; set; }

        [Category("Input")]
        [Description("The ID of the created TableRecord")]
        [RequiredArgument]
        public OutArgument<long> TableRecordID { get; set; }

        public override string SuccessMessage => "Created";

        protected override string GetQuery(CodeActivityContext context)
        {
            string tableId = TableID.Get(context);
            DateTime dueDate = DueDate.Get(context);

            if (dueDate == null || dueDate == DateTime.MinValue)
                dueDate = DateTime.Now.AddMonths(1);
            
            var dict = DictionaryFields.Get(context);
            if (dict == null || dict.Count == 0)
            {
                var dataRow = DataRowFields.Get(context);
                var tempDict = DataRowToDictionary(dataRow);
                if (tempDict != null && dict.Count > 0)
                    dict = tempDict;
            }

            return BuildQuery(tableId, dict, dueDate);
        }

        public static string BuildQuery(string tableId, Dictionary<string, object> customFields, DateTime dueDate)
        {
            string fieldsString = string.Empty;
            if (customFields.Count > 0)
            {
                List<string> fields = new List<string>();
                foreach (var item in customFields)
                {
                    string value = item.Value.ToString();
                    value = PipefyQuery.EscapeStringValue(value);
                    fields.Add(string.Format(TableRecordFieldQueryPart, item.Key, value));
                }
                fieldsString = string.Join(", ", fields);
            }

            return string.Format(CreateTableRecordQuery, tableId, dueDate.ToString("s"), fieldsString);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            JObject joResult = json["data"]["createTableRecord"] as JObject;
            JObject joTableRecord = joResult["table_record"] as JObject;
            long idPipefy = joTableRecord.Value<long>("id");
            TableRecordID.Set(context, idPipefy);
        }

        private Dictionary<string, object> DataRowToDictionary(DataRow row)
        {
            var dict = new Dictionary<string, object>();
            if (row == null || row.Table.Columns.Count == 0)
                return dict;
            
            foreach (DataColumn column in row.Table.Columns)
            {
                string colName = column.ColumnName;
                dict.Add(colName, row[colName]);
            }

            return dict;
        }
    }
}