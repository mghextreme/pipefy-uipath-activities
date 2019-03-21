using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Table
{
    /// <summary>
    /// Gets information about multiple Tables in Pipefy.
    /// </summary>
    [Description("Gets information about multiple Tables in Pipefy.")]
    public class GetTables : PipefyQueryActivity
    {
        private const string GetTableRecordQuery = @"tables(ids: [ {0} ]){{ id name description public table_records_count title_field {{ id type unique }} url table_fields {{ id description internal_id type is_multiple required }} }}";

        [Category("Input")]
        [Description("Table IDs to be obtained")]
        [RequiredArgument]
        public InArgument<string[]> TableIDs { get; set; }

        [Category("Output")]
        [Description("Tables obtained (JObject[])")]
        [RequiredArgument]
        public OutArgument<JObject[]> Tables { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var tableIds = TableIDs.Get(context);
            string tablesString = "\"" + string.Join("\", \"", tableIds) + "\"";
            return string.Format(GetTableRecordQuery, tablesString);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var tables = json["data"]["tables"] as JArray;
            var tablesList = new List<JObject>();
            foreach (var item in tables)
                tablesList.Add(item as JObject);
            Tables.Set(context, tablesList.ToArray());
        }
    }
}