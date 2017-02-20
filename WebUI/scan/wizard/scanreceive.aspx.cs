using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

using Orchestrator;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class scanreceive : System.Web.UI.Page
    {
        private PdfDocument _pdfDocument = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //If the http post file upload is being used then the PODs must be on this server
                //so we can append etc.
                //TODO However this needs to be tested

                // get the existing file if neccessary
                
                if (Request.QueryString["AppendOrReplace"] != null && Request.QueryString["AppendOrReplace"].ToString().Contains("A"))
                {
                    // get the ScannedForm record so that we can retrieve the existing document and append to it.
                    BusinessLogicLayer.Form busForm = new Orchestrator.BusinessLogicLayer.Form();
                    Entities.Scan scanForm = busForm.GetForScannedFormId(int.Parse(Request.QueryString["ScannedFormId"].ToString()));

                    if (scanForm != null)
                    {
                        _pdfDocument = PdfReader.Open(Path.Combine(Server.MapPath("~/PDFS"), scanForm.ScannedFormPDF),PdfDocumentOpenMode.Import);
                    }
                }

                HttpFileCollection files = HttpContext.Current.Request.Files;
                HttpPostedFile postedFile = files[0];

                BufferedStream bs = null;
                byte[] fileOutput = null;

                try
                {
                    if (postedFile.InputStream != null)
                    {
                        bs = new BufferedStream(postedFile.InputStream);
                        int length = Convert.ToInt32(bs.Length);
                        fileOutput = new byte[length];
                        bs.Read(fileOutput, 0, length);
                    }
                }
                finally
                {
                    bs.Close();
                    bs.Dispose();
                }

                if (fileOutput.Length > 0)
                {
                    string basePath = Server.MapPath("~/PDFS");
                    if (!basePath.EndsWith("\\"))
                        basePath += "\\";

                    bool writeError = false;

                    string pathAndFileName = Path.Combine(basePath, postedFile.FileName);
                    string path = pathAndFileName.Replace(Path.GetFileName(pathAndFileName), "");

                    MemoryStream ms = new MemoryStream(fileOutput);

                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (!writeError)
                    {
                        try 
                        {
                            // create new pdf
                            MemoryStream previousFileStream = new MemoryStream(fileOutput);

                            PdfDocument newPdfDoc = PdfReader.Open(previousFileStream, PdfDocumentOpenMode.Modify);
                           
                            // _pdf will be null unless we are appending to an existing doc
                            if (_pdfDocument == null)
                                newPdfDoc.Save(pathAndFileName);
                            else
                            {
                                foreach(PdfPage page in newPdfDoc.Pages)
                                    _pdfDocument.AddPage(page);

                                _pdfDocument.Save(pathAndFileName);
                            }
                            //File.WriteAllBytes(string.Format("{0}\\{1}", path, fileName), ms.ToArray()); 
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    ms.Close();
                    ms.Dispose();
                }
            }
        }
    }
}
