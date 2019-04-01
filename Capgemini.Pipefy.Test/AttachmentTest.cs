using System;
using System.Activities;
using System.IO;
using System.Linq;
using Capgemini.Pipefy.Attachment;
using Capgemini.Pipefy.TableRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class AttachmentTest
    {
        private static TestConfiguration testConfig;
        private static Helper.Table attachmentTable;

        [ClassInitialize]
        public static void AttachmentTestInitialize(TestContext context)
        {
            testConfig = TestConfiguration.Instance;
            attachmentTable = Helper.Table.CreateTable("AtachmentTable");
            var attachmentField = new Helper.CustomField("File", "attachment");
            attachmentTable.CreateField(attachmentField);
        }

        [ClassCleanup]
        public static void AttachmentTestCleanup()
        {
            attachmentTable.Delete();
        }

        [TestMethod]
        public void UploadFile_TableRecordFileInfo_Success()
        {
            // Create

            var dict = testConfig.GetDefaultActivityArguments();
            dict["TableID"] = attachmentTable.Id;
            dict["Title"] = "Upload TextFile Test Record";

            var act = new CreateTableRecord();

            var result = WorkflowInvoker.Invoke(act, dict);
            Assert.IsTrue((bool)result["Success"]);
            var recordId = (long)result["TableRecordID"];
            Assert.IsTrue(recordId > 0);

            // Upload file

            var fileInfo = new FileInfo("TestFiles/simple-text.txt");

            dict = testConfig.GetDefaultActivityArguments();
            dict["OrganizationID"] = testConfig.GetCustomConfig("OrganizationID");
            dict["FileInfo"] = fileInfo;

            var act2 = new UploadAttachment();
            result = WorkflowInvoker.Invoke(act2, dict);
            var uploadedUrl = (string)result["FileUrl"];

            Assert.IsTrue((bool)result["Success"]);
            Assert.IsFalse(string.IsNullOrWhiteSpace(uploadedUrl));

            // Update TableRecord field

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;
            dict["FieldID"] = "file";
            dict["Value"] = new string[]{ uploadedUrl };

            var act3 = new SetTableRecordFieldValue();
            result = WorkflowInvoker.Invoke(act3, dict);
            Assert.IsTrue((bool)result["Success"]);

            // Get

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act4 = new GetTableRecord();
            result = WorkflowInvoker.Invoke(act4, dict);
            Assert.IsTrue((bool)result["Success"]);

            var tableRecord = (JObject)result["TableRecord"];
            var valuesJArray = tableRecord["record_fields"] as JArray;
            var valuesDict = Helper.TableRecord.FieldsJArrayToJObjectDictionary(valuesJArray);
            Assert.AreEqual(uploadedUrl, valuesDict["file"]["array_value"].First.Value<string>());

            // Delete

            dict = testConfig.GetDefaultActivityArguments();
            dict["TableRecordID"] = recordId;

            var act5 = new DeleteTableRecord();
            result = WorkflowInvoker.Invoke(act5, dict);
            Assert.IsTrue((bool)result["Success"]);
            Assert.AreEqual(act5.SuccessMessage, result["Status"].ToString());
        }
    }
}