using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.TableRecord
{
    /// <summary>
    /// Deletes a TableRecord in a Pipefy Table.
    /// </summary>
    [Description("Deletes a TableRecord in a Pipefy Table.")]
    public class DeleteTableRecord : PipefyQueryActivity
    {
        private const string DeleteTableRecordQuery = @"mutation {{ deleteTableRecord(input: {{ id: ""{0}"" }}){{ success }} }}";

        [Category("Input")]
        [Description("ID of the TableRecord to be deleted")]
        [RequiredArgument]
        public InArgument<long> TableRecordID { get; set; }

        public override string SuccessMessage => "Deleted";

        protected override string GetQuery(CodeActivityContext context)
        {
            long recordId = TableRecordID.Get(context);
            return string.Format(DeleteTableRecordQuery, recordId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            bool success = json["deleteTableRecord"].Value<bool>("success");
            if (!success)
                throw new PipefyException("Couldn't delete record");
        }
    }
}