using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.User
{
    /// <summary>
    /// Gets detailes about the current user.
    /// </summary>
    [Description("Gets detailes about the current user.")]
    public class Me : PipefyQueryActivity
    {
        private const string MeQuery = "query { me { avatarUrl email id locale name timeZone username } }";

        [Category("Output")]
        [Description("User obtained (JObject)")]
        public OutArgument<JObject> User { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            return MeQuery;
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var me = json["me"] as JObject;
            User.Set(context, me);
        }
    }
}