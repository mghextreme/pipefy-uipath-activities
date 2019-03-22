using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.TableRecord
{
    /// <summary>
    /// Updates a field value in a TableRecord in Pipefy.
    /// </summary>
    [Description("Updates a field value in a TableRecord in Pipefy.")]
    public class SetTableRecordFieldValue : PipefyQueryActivity
    {
        private const string SetTableRecordFieldValueQuery = "mutation {{ setTableRecordFieldValue(input: {{ table_record_id: \"{0}\" field_id: \"{1}\" value: {2} }}) {{ table_record {{ id title }} table_record_field {{ value }} }} }}";

        [Category("Input")]
        [Description("TableRecord ID to be updated")]
        [RequiredArgument]
        public InArgument<long> TableRecordID { get; set; }

        [Category("Input")]
        [Description("ID of the field to be updated")]
        [RequiredArgument]
        public InArgument<string> FieldID { get; set; }

        [Category("Input")]
        [Description("Value to be placed at the field")]
        [RequiredArgument]
        public InArgument<object> Value { get; set; }

        public override string SuccessMessage => "Updated";

        protected override string GetQuery(CodeActivityContext context)
        {
            long id = TableRecordID.Get(context);
            string fieldId = FieldID.Get(context);
            object fieldValue = Value.Get(context);

            return string.Format(SetTableRecordFieldValueQuery, id, fieldId, "\"" + fieldValue.ToString() + "\"");
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            
        }
    }
}