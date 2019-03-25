using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Organization
{
    /// <summary>
    /// Gets detailed information on a Phase.
    /// </summary>
    [Description("Gets detailed information on a Phase.")]
    public class GetOrganization : PipefyQueryActivity
    {
        private const string GetOrganizationQuery = "query {{ phase(id: {0}){{ id name pipes {{ cards_count description id name opened_cards_count public role }} role tables {{ edges {{ node {{ id name public url }} }} }} users {{ email id name }} }} }}";

        [Category("Input")]
        [Description("ID of the Organization to be obtained")]
        [RequiredArgument]
        public InArgument<long> OrganizationID { get; set; }

        [Category("Output")]
        [Description("Organization obtained (JObject)")]
        public OutArgument<JObject> Organization { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var orgId = OrganizationID.Get(context);
            return string.Format(GetOrganizationQuery, orgId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var org = json["phase"] as JObject;
            Organization.Set(context, org);
        }
    }
}