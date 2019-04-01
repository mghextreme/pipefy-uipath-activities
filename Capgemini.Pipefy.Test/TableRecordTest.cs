using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Capgemini.Pipefy.Table;
using Capgemini.Pipefy.TableRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class TableRecordTest
    {
        private static TestConfiguration testConfig;
        private static Helper.Table simpleTable, customFieldsTable;

        [ClassInitialize]
        public static void TableRecordTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            simpleTable = Helper.Table.CreateTable("SimpleTable");
            customFieldsTable = Helper.Table.CreateTable("CustomFieldsTable");

            // Add Custom Fields

            var numberField = new Helper.CustomField("Code", "number");
            customFieldsTable.CreateField(numberField);
            var stringField = new Helper.CustomField("Description", "short_text");
            customFieldsTable.CreateField(stringField);
            var dateTimeField = new Helper.CustomField("Promo until", "datetime");
            customFieldsTable.CreateField(dateTimeField);
        }

        [ClassCleanup]
        public static void TableRecordTestCleanup()
        {
            simpleTable.Delete();
            customFieldsTable.Delete();
        }

        [TestMethod]
        public void TableRecord_CreateByDictionaryAndDelete_Success()
        {
            var title = "Table Record by Dict - " + DateTime.Now.ToShortDateString();
            var dueDate = DateTime.Now.AddDays(10).Date;

            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            var dictValues = customFieldsTable.GenerateRandomRecordDictionary();
            dict["TableID"] = customFieldsTable.Id;
            dict["Title"] = title;
            dict["DueDate"] = dueDate;
            dict["DictionaryFields"] = dictValues;

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var recordId = (long)result["TableRecordID"];
            Assert.IsTrue(recordId > 0);

            // Get

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act2 = new GetTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            var tableRecord = (JObject)result["TableRecord"];

            Assert.AreEqual(title, tableRecord.Value<string>("title"));
            Assert.AreEqual(dueDate, tableRecord.Value<DateTime>("due_date"));

            var valuesJArray = tableRecord["record_fields"] as JArray;
            var valuesDict = Helper.TableRecord.FieldsJArrayToJObjectDictionary(valuesJArray);
            Assert.AreEqual(dictValues["code"].ToString(), valuesDict["code"].Value<string>("value"));
            Assert.AreEqual(dictValues["description"], valuesDict["description"].Value<string>("value"));

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act3 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act3, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act3.SuccessMessage, result["Status"].ToString());
        }

        [TestMethod]
        public void TableRecord_CreateByDataRowAndDelete_Success()
        {
            var title = "Table Record by DataRow - " + DateTime.Now.ToShortDateString();
            var dueDate = DateTime.Now.AddDays(10).Date;

            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            var dataRowValues = customFieldsTable.GenerateRandomRecordDataRow();
            dict["TableID"] = customFieldsTable.Id;
            dict["Title"] = title;
            dict["DueDate"] = dueDate;
            dict["DataRowFields"] = dataRowValues;

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var recordId = (long)result["TableRecordID"];
            Assert.IsTrue(recordId > 0);

            // Get

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act2 = new GetTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            var tableRecord = (JObject)result["TableRecord"];

            Assert.AreEqual(title, tableRecord.Value<string>("title"));
            Assert.AreEqual(dueDate, tableRecord.Value<DateTime>("due_date"));

            var valuesJArray = tableRecord["record_fields"] as JArray;
            var valuesDict = Helper.TableRecord.FieldsJArrayToJObjectDictionary(valuesJArray);
            Assert.AreEqual(dataRowValues["code"].ToString(), valuesDict["code"].Value<string>("value"));
            Assert.AreEqual(dataRowValues["description"], valuesDict["description"].Value<string>("value"));

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act3 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act3, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act3.SuccessMessage, result["Status"].ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TableRecord_AvoidDictionaryAndDataRow_Exception()
        {
            var testConfig = TestConfiguration.Instance;
            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = customFieldsTable.Id;
            dict["DictionaryFields"] = customFieldsTable.GenerateRandomRecordDictionary();
            dict["DataRowFields"] = customFieldsTable.GenerateRandomRecordDataRow();
            var act = new CreateTableRecord();
            WorkflowInvoker.Invoke(act, dict);
        }
    }
}