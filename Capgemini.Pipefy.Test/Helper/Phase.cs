using System;
using System.Collections.Generic;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class Phase
    {
        private const string CreatePhaseQuery = "mutation {{ createPhase(input: {{ pipe_id: {0} name: {1} done: {2} }}){{ phase {{ created_at done id name }} }} }}";
        private const string DeletePhaseQuery = "mutation {{ deletePhase(input: {{ id: {0} }}){{ success }} }}";
        private const string CreatePhaseFieldQuery = "mutation {{ createPhaseField(input: {{ phase_id: {0} label: {1} type: {2} required: {3} }}){{ phase_field {{ id is_multiple label required type }} }} }}";

        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public bool Done { get; protected set; }
        public Pipe Pipe { get; protected set; }
        public ICollection<CustomField> Fields { get; protected set; }

        internal void SetParentPipe(Pipe pipe)
        {
            Pipe = pipe;
        }

        internal void Delete()
        {
            DeletePhase(this);
        }

        internal CustomField CreateField(CustomField field)
        {
            var addedField = CreatePhaseField(this, field);
            Fields.Add(addedField);
            return addedField;
        }

        #region Static

        internal static Phase CreatePhase(Pipe pipe, string name, bool done = false)
        {
            var newPhase = new Phase(){
                Name = name,
                Done = done
            };

            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(CreatePhaseQuery, pipe.Id, newPhase.Name.ToQueryValue(), newPhase.Done.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var phase = resultObj["data"]["createPhase"]["phase"];
            newPhase.Id = phase.Value<long>("id");

            return newPhase;
        }

        internal static CustomField CreatePhaseField(Phase phase, CustomField field)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(CreatePhaseFieldQuery, phase.Id.ToQueryValue(), field.Label.ToQueryValue(), field.Type.ToQueryValue(), field.IsRequired.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var resultField = resultObj["data"]["createPhaseField"]["phase_field"];
            field.Id = resultField.Value<string>("id");
            field.IsMultiple = resultField.Value<bool>("is_multiple");
            field.SetParentPhase(phase);
            return field;
        }

        internal static void DeletePhase(Phase phase)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(DeletePhaseQuery, phase.Id.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);
            var success = resultObj["data"]["deletePhase"].Value<bool>("success");
            if (!success)
                throw new ApplicationException($"Couldn't delete phase {phase.Name}.");
        }

        #endregion Static
    }
}