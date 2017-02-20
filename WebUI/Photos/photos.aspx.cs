using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Orchestrator.WebUI.Photos
{
    public partial class photos : System.Web.UI.Page
    {
        List<ImageDetails> images = null;
        string url = string.Empty;
        List<DateTime> dates = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (!IsPostBack)
            {
                dates = new List<DateTime>();
                images = new List<ImageDetails>();

                if (!dates.Contains(DateTime.Today))
                {
                    dates.Add(DateTime.Today);
                }


                url = this.Request.Url.AbsoluteUri.Replace("photos.aspx", "pictures");
                string[] files = Directory.GetFiles(Server.MapPath("pictures"));
                
                foreach (var f in files)
                {
                    FileInfo fi = new FileInfo(f);
                    DateTime createDate = fi.CreationTime.Date;
                    if (!dates.Contains(createDate.Date))
                        dates.Add(createDate.Date);

                    if (createDate > DateTime.Today.AddDays(-1))
                    {
                        images.Add(GetImagedetails(url, Path.GetFileName(f)));
                        //images.Add(new ImageDetails(Path.GetFileNameWithoutExtension(f), url + "\\" + Path.GetFileName(f)));
                    }
                }

                
                BindImages();
            }
        }

        private void BindImages()
        {

            if (dates != null)
            {
                cboPictureDates.DataSource = dates.OrderBy(d => d); ;
                cboPictureDates.DataBind();
                if (!IsPostBack)
                {
                    cboPictureDates.Items.FindByText(DateTime.Today.ToShortDateString()).Selected = true;
                }
            }

            lvPhotos.DataSource = images;
            lvPhotos.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cboPictureDates.SelectedIndexChanged += new EventHandler(lstpictureDates_SelectedIndexChanged);
        }

        void lstpictureDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (images != null)
                images.Clear();
            else
                images = new List<ImageDetails>();
            url = this.Request.Url.AbsoluteUri.Replace("photos.aspx", "pictures");
            string[] files = Directory.GetFiles(Server.MapPath("pictures"));
            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);
                DateTime createDate = fi.CreationTime.Date;
                if (createDate.Date == DateTime.Parse(cboPictureDates.SelectedValue).Date)
                {
                    images.Add(GetImagedetails(url, Path.GetFileName(f)));
                    //images.Add(new ImageDetails(Path.GetFileNameWithoutExtension(f), url + "\\" + Path.GetFileName(f)));
                }
            }
            BindImages();
        }

        //DGreg Duffield-Dte2012-07-2412-13-49-Lat52.6053316666667-Lon1.24540833333333
        private ImageDetails GetImagedetails(string url, string fileName)
        {
            
            string driver = fileName.Substring(1, fileName.IndexOf("-") - 1);
            string date = fileName.Substring(fileName.IndexOf("Dte") + 3, (fileName.IndexOf("-Lat") - 4) - fileName.IndexOf("Dte") + 1);
            date = date.Substring(0, 10) + " " + date.Substring(10).Replace("-", ":");
            string latitude = fileName.Substring(fileName.IndexOf("Lat") + 3, (fileName.IndexOf("-Lon") - fileName.IndexOf("Lat")) - 3);
            string longitude = Path.GetFileNameWithoutExtension(fileName).Substring(fileName.IndexOf("Lon") + 3);
            string fileUrl = url + "/" + Path.GetFileName(fileName);

            ImageDetails photo = new ImageDetails();
            photo.Filename = fileName;
            photo.Driver = driver;
            photo.Date = date;
            photo.Latitude = double.Parse(latitude);
            photo.Longitude = double.Parse(longitude);
            photo.Url = fileUrl;

            return photo;
        }
    }

    public class ImageDetails
    {
        public ImageDetails() { }
        public ImageDetails(string filename, string url)
        {
            Filename = filename;
            Url = url;
        }
        public string Filename { get; set; }
        public string Url { get; set; }
        public string Driver { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Date { get; set; }

    }
}