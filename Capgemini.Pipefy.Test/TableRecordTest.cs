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
        private static Dictionary<string, string> customFields;

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
            var testConfig = TestConfiguration.Instance;
            var title = "Table Record by Dict - " + DateTime.Now.ToShortDateString();

            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = customFieldsTable.Id;
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;
            dict["DictionaryFields"] = customFieldsTable.GenerateRandomRecordDictionary();

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.IsTrue((long)result["TableRecordID"] > 0);

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = result["TableRecordID"];

            var act2 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
        }

        [TestMethod]
        public void TableRecord_CreateByDataRowAndDelete_Success()
        {
            var testConfig = TestConfiguration.Instance;
            var title = "Table Record by DataRow - " + DateTime.Now.ToShortDateString();

            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = customFieldsTable.Id;
            dict["Title"] = title;
            dict["DueDate"] = DateTime.Now.AddDays(10).Date;
            dict["DataRowFields"] = customFieldsTable.GenerateRandomRecordDataRow();

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.IsTrue((long)result["TableRecordID"] > 0);

            // Delete

            dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["TableRecordID"] = result["TableRecordID"];

            var act2 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act2.SuccessMessage, result["Status"].ToString());
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