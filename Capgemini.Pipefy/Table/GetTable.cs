using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Table
{
    /// <summary>
    /// Gets detailed information about a Tables in Pipefy.
    /// </summary>
    [Description("Gets detailed information about a Table in Pipefy.")]
    public class GetTable : PipefyQueryActivity
    {
        private const string GetTableQuery = @"query {{ table(id: ""{0}""){{ description id name public statuses {{ id name }} table_fields {{ description id internal_id type required label }} table_records_count title_field {{ id label internal_id type }} url }} }}";

        [Category("Input")]
        [Description("ID of the Table to be obtained")]
        [RequiredArgument]
        public InArgument<string> TableID { get; set; }

        [Category("Output")]
        [Description("Table obtained (JObject)")]
        [RequiredArgument]
        public OutArgument<JObject> Table { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var tableId = TableID.Get(context);
            return string.Format(GetTableQuery, tableId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var table = json["data"]["table"] as JObject;
            Table.Set(context, table);
        }
    }
}