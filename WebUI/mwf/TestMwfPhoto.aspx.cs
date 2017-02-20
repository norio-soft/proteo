using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.mwf
{
    public partial class TestMwfPhoto : System.Web.UI.Page
    {
        private const string DRIVER_NAME = "Atest Atest";
        private const string PHOTO_DATE_TIME = "2013-05-01 14:18:15";
        private const string PHOTO_COMMENT = "Test photo comment";
        private const string MWF_INTERNAL_ID = "9245fe4a-d402-451c-b9ed-9c1a04247482";
        private const string LATITUDE = "52.6282166666667";
        private const string LONGITUDE = "1.28824";
        private const string MAD_IDS = "";
        private const string ORDER_IDS = "";
        //private const string MAD_IDS = "b9337070-5fb9-482c-b2d6-17d5277daea5,";
        //private const string ORDER_IDS = "238443,";
        private const string PHOTO_FILE_FORMAT = "jpg";

        private static readonly Encoding encoding = Encoding.UTF8;

        protected void Page_Load(object sender, EventArgs e)
        {
            var imageFilePath = Server.MapPath("~/mwf/testmwfphoto.jpg");
            var statusCode = this.PostImage(imageFilePath);

            if (statusCode == HttpStatusCode.OK)
                this.lblResult.Text = string.Format("A test image has been posted and Http status code 200 was returned.  Please check the MWF_Photo table: Driver Name: {0}, Comment: {1}.", DRIVER_NAME, PHOTO_COMMENT);
            else
                this.lblResult.Text = string.Format("The test image was not posted successfully.  Http status code {0} was returned.", (int)statusCode);
        }

        /// <summary>
        /// Post the test photo to ReceiveMwfPhoto.aspx
        /// Set the constants above before posting the photo.
        /// </summary>
        private HttpStatusCode PostImage(string imageFilePath)
        {
            using (var image = new Bitmap(imageFilePath))
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);

                string userAgent = "ToughTough";

                string photoFileName = Path.GetFileName(imageFilePath);
                var postParameters = new Dictionary<string, object>();
                postParameters.Add("filename", photoFileName);
                postParameters.Add("fileformat", PHOTO_FILE_FORMAT);
                postParameters.Add("file", new FileParameter(ms.ToArray()));
                postParameters.Add("MwfInternalId", MWF_INTERNAL_ID);
                postParameters.Add("DriverName", DRIVER_NAME);
                postParameters.Add("PhotoComment", PHOTO_COMMENT);
                postParameters.Add("PhotoDateTime", PHOTO_DATE_TIME);
                postParameters.Add("Latitude", LATITUDE);
                postParameters.Add("Longitude", LONGITUDE);
                postParameters.Add("MobileApplicationDataIds", MAD_IDS);
                postParameters.Add("HEOrderIds", ORDER_IDS);

                var url = Request.Url.GetLeftPart(UriPartial.Authority) + "/mwf/receivemwfphoto.aspx";

                using (HttpWebResponse webResponse = MultipartFormDataPost(url, userAgent, postParameters))
                using (var responsereader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string fullResponse = responsereader.ReadToEnd();
                    return webResponse.StatusCode;
                }
            }
        }

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            var request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
                throw new NullReferenceException("request is not a http request");

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.ContentLength = formData.Length;
            request.Timeout = 100000;

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            using (var formDataStream = new System.IO.MemoryStream())
            {
                bool needsCLRF = false;

                foreach (var param in postParameters)
                {

                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    needsCLRF = true;

                    if (param.Value is FileParameter)
                    {
                        FileParameter fileToUpload = (FileParameter)param.Value;

                        // Add just the first part of this param, since we will write the file data directly to the Stream
                        string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                            boundary,
                            param.Key,
                            fileToUpload.FileName ?? param.Key,
                            fileToUpload.ContentType ?? "application/octet-stream");

                        formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                    }
                    else
                    {
                        string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                            boundary,
                            param.Key,
                            param.Value);
                        formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                    }
                }

                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                byte[] formData = new byte[formDataStream.Length];
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Close();

                return formData;
            }
        }

    }

    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

}