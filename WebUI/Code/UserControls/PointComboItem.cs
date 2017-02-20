using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

namespace Orchestrator.WebUI.Controls
{
    /// <summary>
    /// Used by the Point user control - represents a point combo item that has 
    /// three columns: OrganisationName, PointDescription, Address.
    /// </summary>
    public class PointComboItem
    {
        public PointComboItem(DataRow dr)
        {
            const string SEPARATOR = "<br/>";

            this.OrganisationName = dr["OrganisationName"].ToString();
            this.Description = dr["Description"].ToString();

            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(dr["AddressLine1"].ToString()))
            {
                sb.Append(string.Format("{0}{1}", dr["AddressLine1"].ToString(), SEPARATOR));
            }

            if (!string.IsNullOrEmpty(dr["AddressLine2"].ToString()))
            {
                sb.Append(string.Format("{0}{1}", dr["AddressLine2"].ToString(), SEPARATOR));
            }

            if (!string.IsNullOrEmpty(dr["AddressLine3"].ToString()))
            {
                sb.Append(string.Format("{0}{1}", dr["AddressLine3"].ToString(), SEPARATOR));
            }

            if (!string.IsNullOrEmpty(dr["PostTown"].ToString()))
            {
                sb.Append(string.Format("{0}{1}", dr["PostTown"].ToString(), SEPARATOR));
            }

            if (!string.IsNullOrEmpty(dr["County"].ToString()))
            {
                sb.Append(string.Format("{0}{1}", dr["County"].ToString(), SEPARATOR));
            }

            if (!string.IsNullOrEmpty(dr["PostCode"].ToString()))
            {
                sb.Append(dr["PostCode"].ToString());
            }
            if (!string.IsNullOrEmpty(dr["CountryDescription"].ToString()))
            {
                sb.Append(dr["CountryDescription"].ToString());
            }

            this.Address = sb.ToString();

        }

        private string _organisationName;
        public string OrganisationName
        {
            get { return this._organisationName; }
            set { this._organisationName = value; }
        }

        private string _description;
        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        private string _address;
        public string Address
        {
            get { return this._address; }
            set { this._address = value; }
        }

        public string SingleLineText
        {
            get { return String.Format("<table><tr valign='top'><td width='140'>{0}</td><td>{1}</td></table>", this.Description, this.Address); }
        }
    }
}