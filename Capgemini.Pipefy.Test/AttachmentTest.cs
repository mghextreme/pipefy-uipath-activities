using System;
using System.Activities;
using System.IO;
using System.Linq;
using Capgemini.Pipefy.Attachment;
using Capgemini.Pipefy.Card;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    [TestClass]
    public class AttachmentTest
    {
        [TestMethod]
        public void UploadFile_SimpleText_Success()
        {
            // Upload file

            var config = TestConfiguration.Instance.Configuration;
            var orgId = config["organization"].Value<long>("id");
            var fileInfo = new FileInfo("TestFiles/simple-text.txt");

            var dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["OrganizationID"] = orgId;
            dict["FileInfo"] = fileInfo;

            var act = new UploadAttachment();
            var result = WorkflowInvoker.Invoke(act, dict);
            var uploadedUrl = (string)result["FileUrl"];

            Assert.IsTrue((bool)result["Success"]);
            Assert.IsFalse(string.IsNullOrWhiteSpace(uploadedUrl));
            uploadedUrl = uploadedUrl.Trim('/', '\\');

            // Update card field

            dict = TestConfiguration.Instance.GetDefaultActivityArguments();
            dict["CardID"] = 24591712;
            dict["FieldID"] = "attachment";
            dict["Value"] = new string[]{ uploadedUrl };

            var act2 = new UpdateCardField();
            result = WorkflowInvoker.Invoke(act2, dict);
            Assert.IsTrue((bool)result["Success"]);
        }
    }
}