using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Attachment
{
    /// <summary>
    /// Uploads an attachment to Pipefy.
    /// </summary>
    [Description("Uploads an attachment to Pipefy.")]
    public class UploadAttachment : PipefyQueryActivity
    {
        private const string CreatePresignedUrlQuery = "mutation {{ createPresignedUrl(input: {{ organizationId: {0} fileName: \"{1}\" contentType: \"{2}\" }}){{ url }} }}";

        [Category("Input")]
        [Description("ID of the Organization to place the file")]
        [RequiredArgument]
        public InArgument<long> OrganizationID { get; set; }

        [Category("Input")]
        [Description("FileInfo of the file to be uploaded")]
        [RequiredArgument]
        public InArgument<FileInfo> FileInfo { get; set; }

        [Category("Output")]
        [Description("Private URL of the uploaded file")]
        public OutArgument<string> FileUrl { get; set; }

        public override string SuccessMessage => "Uploaded";

        private FileInfo fileInfo;
        private string contentType;
        private string amazonUrl;

        protected override void SafeExecute(CodeActivityContext context)
        {
            // Create URL

            string bearer = Bearer.Get(context);
            var queryText = GetQuery(context);
            var query = new PipefyQuery(queryText, bearer);

            int timeout = Timeout.Get(context);
            query.SetTimeout(timeout);

            string result = query.Execute();
            CheckStatusCode(query.StatusCode);

            JObject json = ParseJson(result);
            ParseResult(context, json["data"] as JObject);

            // Upload File

            UploadFile(fileInfo);
            var resultUrl = new Uri(amazonUrl);
            FileUrl.Set(context, resultUrl.LocalPath.TrimStart('\\', '/'));
        }

        protected override string GetQuery(CodeActivityContext context)
        {
            long organization = OrganizationID.Get(context);
            fileInfo = FileInfo.Get(context);

            if (!fileInfo.Exists)
                throw new ArgumentException("The file doesn't exist.");

            contentType = MimeMapping.GetMimeMapping(fileInfo.Name);
            return string.Format(CreatePresignedUrlQuery, organization, fileInfo.Name, contentType);
        }

        protected override void ParseResult(CodeActivityContext context, JObject json)
        {
            amazonUrl = json["createPresignedUrl"].Value<string>("url");
        }

        private void UploadFile(FileInfo fileInfo)
        {
            // https://docs.aws.amazon.com/AmazonS3/latest/dev/UploadObjectPreSignedURLDotNetSDK.html
            HttpWebRequest httpRequest = WebRequest.Create(amazonUrl) as HttpWebRequest;
            httpRequest.Method = "PUT";
            httpRequest.ContentType = contentType;
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[8000];
                var filePath = fileInfo.FullName;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
        }
    }
}