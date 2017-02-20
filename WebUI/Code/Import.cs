using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;


/// <summary>
/// Summary description for import
/// </summary>
[WebService(Namespace = "http://webservices.orchestrator.co.uk/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Import : System.Web.Services.WebService
{

    public Import()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool ImportCustomers(DataSet ds)
    {

        DataTable dt = ds.Tables[0];
        Orchestrator.Entities.Organisation organisation;
        Orchestrator.Entities.OrganisationLocation ol;
        Orchestrator.Entities.Individual i;
        Orchestrator.Entities.Point point;
        Orchestrator.Entities.Address address;

        Orchestrator.Facade.Organisation facOrganisation = new Orchestrator.Facade.Organisation();

        foreach (DataRow row in dt.Rows)
        {
            organisation = new Orchestrator.Entities.Organisation();
            organisation.OrganisationName = row["NAME"].ToString();
            organisation.OrganisationDisplayName = row["NAME"].ToString();
            organisation.OrganisationType = Orchestrator.eOrganisationType.Client;
            organisation.AccountCode = row["ACCOUNT_REF"].ToString();

            address = new Orchestrator.Entities.Address();
            address.AddressType = Orchestrator.eAddressType.Correspondence;
            address.AddressLine1 = row["ADDRESS_1"].ToString();
            address.AddressLine2 = row["ADDRESS_2"].ToString();
            address.PostTown = row["ADDRESS_3"].ToString();
            address.County = row["ADDRESS_4"].ToString();
            address.PostCode = row["ADDRESS_5"].ToString();
            address.TrafficArea = new Orchestrator.Entities.TrafficArea();
            address.TrafficArea.TrafficAreaId = 0;

            //Resolve The post Town if possible.
            string defaultPostTown = "Glasgow";
            int defaultPostTownId = 9063;

            string postTown = (row["ADDRESS_4"].ToString() == "" ? null : row["ADDRESS_4"].ToString()) ?? (row["ADDRESS_3"].ToString() == "" ? null : row["ADDRESS_3"].ToString()) ?? (row["ADDRESS_2"].ToString() == "" ? null : row["ADDRESS_2"].ToString()) ?? defaultPostTown;

            if (address.PostTown == string.Empty)
                address.PostTown = postTown;


            int? postTownID = null ;
            Orchestrator.Facade.ReferenceData facRef = new Orchestrator.Facade.ReferenceData();
            DataSet dsTown =  facRef.GetTownForTownName(postTown);
            if (dsTown.Tables[0].Rows.Count > 0)
            {
                postTownID = int.Parse(dsTown.Tables[0].Rows[0]["TownId"].ToString());
            }


            ol = new Orchestrator.Entities.OrganisationLocation();
            ol.OrganisationLocationName = "Head Office";
            ol.OrganisationLocationType = Orchestrator.eOrganisationLocationType.HeadOffice;
            

            point = new Orchestrator.Entities.Point();
            point.Description = "Head Office";
            point.Collect = false;
            point.Deliver = false;
            point.Address = address;
            point.PostTown = new Orchestrator.Entities.PostTown();
            point.PostTown.TownName = postTown;
            point.PostTown.TownId = postTownID ?? defaultPostTownId;

            ol.Point = point;

            if (organisation.Locations == null)
                organisation.Locations = new Orchestrator.Entities.OrganisationLocationCollection();

            organisation.Locations.Add(ol);

            i = new Orchestrator.Entities.Individual();
            i.Contacts = new Orchestrator.Entities.ContactCollection();
            i.IndividualType = Orchestrator.eIndividualType.Contact;
            i.Title = Orchestrator.eTitle.Mr;
            if (row["CONTACT_NAME"] != string.Empty)
                if (row["CONTACT_NAME"].ToString().IndexOf(' ') > 0)
                {
                    i.FirstNames = row["CONTACT_NAME"].ToString().Split(' ')[0];
                    i.LastName = row["CONTACT_NAME"].ToString().Split(' ')[1];
                }
                else
                {
                    i.FirstNames = row["CONTACT_NAME"].ToString();
                    i.LastName = string.Empty;
                }

            if (row["TELEPHONE"].ToString() != string.Empty)
            {
                i.Contacts.Add(new Orchestrator.Entities.Contact(Orchestrator.eContactType.Telephone, row["TELEPHONE"].ToString()));
                ol.TelephoneNumber = row["TELEPHONE"].ToString();
            }

            if (row["FAX"].ToString() != string.Empty)
            {
                i.Contacts.Add(new Orchestrator.Entities.Contact(Orchestrator.eContactType.Fax, row["FAX"].ToString()));
                ol.FaxNumber = row["FAX"].ToString();
            }

            if (row["E_MAIL"].ToString() != string.Empty)
                i.Contacts.Add(new Orchestrator.Entities.Contact(Orchestrator.eContactType.Email, row["E_MAIL"].ToString()));

            if (row["CONTACT_NAME"] != string.Empty)
                organisation.IndividualContacts.Add(i);

            Orchestrator.Entities.FacadeResult ret = facOrganisation.Create(organisation, "Orchestrator_Import");
            if (!ret.Success)
                return false;

        }

        return true;

    }

}


