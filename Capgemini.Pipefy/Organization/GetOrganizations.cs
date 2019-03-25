using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Organization
{
    /// <summary>
    /// Gets information on multiple Organizations.
    /// </summary>
    [Description("Gets information on multiple Organizations.")]
    public class GetOrganizations : PipefyQueryActivity
    {
        private const string GetOrganizationsQuery = "query {{ organizations(ids: {0}){{ id name role }} }}";

        [Category("Input")]
        [Description("IDs of the Organizations to be obtained")]
        public InArgument<long[]> OrganizationIDs { get; set; }

        [Category("Output")]
        [Description("Organizations obtained (JObject)")]
        public OutArgument<JObject[]> Organizations { get; set; }

        protected override string GetQuery(CodeActivityContext context)
        {
            var orgId = OrganizationIDs.Get(context);
            if (orgId == null)
                orgId = new long[0];

            return string.Format(GetOrganizationsQuery, orgId);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            var orgs = json["organizations"] as JArray;
            var orgsList = new List<JObject>();
            foreach (var item in orgs)
                orgsList.Add(item as JObject);
            Organizations.Set(context, orgsList.ToArray());
        }
    }
}