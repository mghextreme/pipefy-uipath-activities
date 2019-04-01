using System;
using System.Collections.Generic;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class Pipe
    {
        private const string CreatePipeQuery = "mutation {{ createPipe(input: {{ organization_id: {0} name: {1} }}){{ pipe {{ id name }} }} }}";
        private const string DeletePipeQuery = "mutation {{ deletePipe(input: {{ id: {0} }}){{ success }} }}";

        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public ICollection<Phase> Phases { get; protected set; }

        private Pipe()
        {
            Phases = new List<Phase>();
        }

        public void Delete()
        {
            DeletePipe(this);
        }

        #region Static

        internal static Pipe CreatePipe(string name)
        {
            var newTable = new Pipe(){
                Name = name
            };

            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();
            var orgId = testConfig.GetCustomConfig("OrganizationID");

            var queryString = string.Format(CreatePipeQuery, orgId, newTable.Name.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var table = resultObj["data"]["createPipe"]["pipe"];
            newTable.Id = table.Value<long>("id");

            return newTable;
        }

        internal static void DeletePipe(Pipe pipe)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(DeletePipeQuery, pipe.Id.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);
            var success = resultObj["data"]["deletePipe"].Value<bool>("success");
            if (!success)
                throw new ApplicationException($"Couldn't delete pipe {pipe.Name}.");
        }

        #endregion Static
    }
}