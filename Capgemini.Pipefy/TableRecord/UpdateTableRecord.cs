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
        protected override string GetQuery(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            throw new NotImplementedException();
        }
    }
}