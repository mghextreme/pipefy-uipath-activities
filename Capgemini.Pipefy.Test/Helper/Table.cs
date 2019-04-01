using System;
using System.Collections.Generic;
using System.Data;

namespace Capgemini.Pipefy.Test.Helper
{
    internal class Table
    {
        private const string CreateTableQuery = "mutation {{ createTable(input: {{ organization_id: {0} name: {1} public: {2} }}){{ table {{ id name url }} }} }}";
        private const string DeleteTableQuery = "mutation {{ deleteTable(input: {{ id: {0} }}){{ success }} }}";
        private const string CreateTableFieldQuery = "mutation {{ createTableField(input: {{ table_id: {0} label: {1} type: {2} required: {3} }}){{ table_field {{ id is_multiple label required type }} }} }}";

        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public bool Public { get; protected set; }
        public ICollection<CustomField> Fields { get; protected set; }

        internal Table()
        {
            Fields = new List<CustomField>();
        }

        public void Delete()
        {
            DeleteTable(this);
        }

        public Dictionary<string, object> GenerateRandomRecordDictionary()
        {
            var fields = new Dictionary<string, object>();
            foreach (var item in Fields)
                fields.Add(item.Id, item.GetRandomValue());
            return fields;
        }

        public DataRow GenerateRandomRecordDataRow()
        {
            var dataTable = new DataTable(Name);
            
            foreach (var item in Fields)
                dataTable.Columns.Add(item.Id);

            var row = dataTable.NewRow();
            foreach (var item in Fields)
                row[item.Id] = item.GetRandomValue();
            dataTable.Rows.Add(row);

            return row;
        }

        internal CustomField CreateField(CustomField field)
        {
            var addedField = CreateTableField(this, field);
            Fields.Add(addedField);
            return addedField;
        }

        #region Static

        internal static Table CreateTable(string name)
        {
            var newTable = new Table(){
                Name = name,
                Public = true
            };

            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();
            var orgId = testConfig.GetCustomConfig("OrganizationID");

            var queryString = string.Format(CreateTableQuery, orgId, newTable.Name.ToQueryValue(), newTable.Public.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var table = resultObj["data"]["createTable"]["table"];
            newTable.Id = table.Value<string>("id");

            return newTable;
        }

        internal static CustomField CreateTableField(Table table, CustomField field)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(CreateTableFieldQuery, table.Id.ToQueryValue(), field.Label.ToQueryValue(), field.Type.ToQueryValue(), field.IsRequired.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);

            var resultField = resultObj["data"]["createTableField"]["table_field"];
            field.Id = resultField.Value<string>("id");
            field.IsMultiple = resultField.Value<bool>("is_multiple");
            field.SetParentTable(table);
            return field;
        }

        internal static void DeleteTable(Table table)
        {
            var testConfig = TestConfiguration.Instance;
            var bearer = testConfig.GetBearer();

            var queryString = string.Format(DeleteTableQuery, table.Id.ToQueryValue());
            var query = new PipefyQuery(queryString, bearer);
            var result = query.Execute();
            var resultObj = PipefyQuery.ParseJson(result);
            var success = resultObj["data"]["deleteTable"].Value<bool>("success");
            if (!success)
                throw new ApplicationException($"Couldn't delete table {table.Name}.");
        }

        #endregion Static
    }
}