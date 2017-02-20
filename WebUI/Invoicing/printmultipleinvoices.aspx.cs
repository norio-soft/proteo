using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Configuration;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class printmultipleinvoices : System.Web.UI.Page
    {
        private List<int> invoiceIDs = null;
        public List<int> InvoiceIDs
        {
            get
            {
                if (invoiceIDs == null)
                {
                    // Get from the querystring and parse into a list;
                    string input = Request.QueryString["i"];
                    List<string> ids = input.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    invoiceIDs = ids.ConvertAll<int>((s) => int.Parse(s));
                }
                return invoiceIDs;
            }
        }

        public string GetURLForPrint
        {
            get
            {
                    return Request.Url.ToString() + "&generate";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.ToString().Contains("generate"))
                CombinePDFs();

            
        }

        private void CombinePDFs()
        {
            // Load the Invoices to get the reference to the Invoice PDF
            List<Entities.Invoice> invoices = new List<Orchestrator.Entities.Invoice>();
            Facade.IInvoice facInvoice = new Facade.Invoice();
            foreach (int id in InvoiceIDs)
            {
                invoices.Add(facInvoice.GetForInvoiceId(id));
            }


            PdfDocument combinedPDF = new PdfDocument();
            PdfDocument pdf;
            // Combine the PDFS into a single document
            foreach (var invoice in invoices)
            {
#if DEBUG
                using (FileStream fs = File.OpenRead(ConfigurationManager.AppSettings["GeneratedPDFRoot"] + invoice.PDFLocation))
                {
                   pdf = PdfReader.Open(fs,PdfDocumentOpenMode.Import);
                }
#else
                using (FileStream fs = File.OpenRead(Server.MapPath(invoice.PDFLocation)))
                {
                    pdf = PdfReader.Open(fs,PdfDocumentOpenMode.Import);
                }
#endif
                foreach(PdfPage page in pdf.Pages)
                    combinedPDF.AddPage(page);
            }
            
            // Stream this to the IFrame
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";

            // this makes a new window appear: Response.AddHeader("content-disposition","attachment; filename=MyPDF.PDF");
            Response.AddHeader("content-disposition", "inline; filename=Invoices.PDF");

            MemoryStream binaryData = new MemoryStream();
            combinedPDF.Save(binaryData);

            Response.BinaryWrite(binaryData.ToArray());
        }
    }
}
