using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy
{
    /// <summary>
    /// Activity for updating a field in a TableReacord in Pipefy.
    /// </summary>
    [Description("Activity for updating a field in a TableReacord in Pipefy.")]
    public class SetTableRecordFieldValue : PipefyQueryActivity
    {
        private const string SetTableRecordFieldValueQuery = "mutation {{ setTableRecordFieldValue(input: {{ table_record_id: \"{0}\" field_id: \"{1}\" value: \"{2}\" }}) {{ table_record {{ id title }} table_record_field {{ value }} }} }}";

        [Category("Input")]
        [Description("TableRecord ID to be updated")]
        [RequiredArgument]
        public InArgument<string> TableRecordID { get; set; }

        [Category("Input")]
        [Description("Fields name to be updated")]
        [RequiredArgument]
        public InArgument<string> FieldName { get; set; }

        [Category("Input")]
        [Description("Value to be placed at the field")]
        [RequiredArgument]
        public InArgument<object> Value { get; set; }
        
        public override string SuccessMessage => "Success";

        protected override string GetQuery(CodeActivityContext context)
        {
            string id = TableRecordID.Get(context);
            string fieldName = FieldName.Get(context);
            object fieldValue = Value.Get(context);

            var query = string.Format(SetTableRecordFieldValueQuery, id, fieldName, fieldValue.ToString());
            return query;
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            
        }
    }
}