using System;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class Organization
    {
        private const string CreateOrganizationQuery = "mutation { createOrganization(input: { name: {1} industry: {0} }){ organization { created_at id name role } } }";
        private const string DeleteOrganizationQuery = "mutation {{ deleteOrganization(input: {{ id: {0} }}){{ success }} }}";

        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public string Industry { get; protected set; }
        public string Role { get; protected set; }

        internal Organization() { }

        public void Delete()
        {
            DeleteOrganization(this);
        }

        #region Static

        internal static Organization CreateOrganization(string name, string industry = "others")
        {
            var newOrg = new Organization(){
                Name = name,
                Industry = industry
            };

            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(CreateOrganizationQuery, newOrg.Name.ToQueryValue(), newOrg.Industry.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var org = resultObj["data"]["createOrganization"]["organization"];
            newOrg.Id = org.Value<long>("id");
            newOrg.Role = org.Value<string>("role");

            return newOrg;
        }

        internal static void DeleteOrganization(Organization org)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(DeleteOrganizationQuery, org.Id.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var success = resultObj["data"]["deleteOrganization"].Value<bool>("success");
            if (!success)
                throw new ApplicationException($"Couldn't delete organization {org.Name}.");
        }

        #endregion Static
    }
}