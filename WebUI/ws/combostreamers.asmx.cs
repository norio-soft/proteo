using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Data;
using Orchestrator;
using Telerik.Web.UI;
using Orchestrator.WebUI.Controls;


namespace Orchestrator.WebUI.ws

{
    /// <summary>
    /// Summary description for combostreamers
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class combostreamers : System.Web.Services.WebService
    {
        [WebMethod]
        public RadComboBoxItemData[] GetCountries(RadComboBoxContext context)
        {
            Orchestrator.Facade.IReferenceData facRef = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRef.GetAllCountries();
            DataTable dt = ds.Tables[0];

            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            
            foreach(DataRow dr in dt.Rows)
            {
                rcItem = new RadComboBoxItemData();
                rcItem.Text = dr["CountryDescription"].ToString();
                rcItem.Value = dr["CountryId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetPoints(RadComboBoxContext context)
        {
            return GetPointsWithStateFilter(context, false);
        }

        [WebMethod]
        public RadComboBoxItemData[] GetPointsIncludeDeleted(RadComboBoxContext context)
        {
            return GetPointsWithStateFilter(context, true);
        }

    
        [WebMethod]
        public RadComboBoxItemData[] GetPointsWithStateFilter(RadComboBoxContext  context, bool includeDeleted)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
            string filter = context.Text;
            int _identityID = -1;
            //int _townID = ((int)contextDictionary["TownID"]);
            if (contextDictionary.Keys.FirstOrDefault(k=>k == "IdentityID") != null)
                _identityID = ((int)contextDictionary["IdentityID"]);

            // The search text entered by the user ("e.Text") can be split into two regions delimited by a backslash.
            // Any text to the left of the first backslash (or when there is no backslash) should be used to filter the organisation name.
            // Any text to the right of a backslash should be used to filter the point description.
            char filterChar ='\\'; // Backslash character "\"

            string[] filterString = filter.Split(';')[0].Split(filterChar.ToString().ToCharArray());

            DataSet ds = null;
            Orchestrator.Facade.Point facPoint = new Orchestrator.Facade.Point();
            int noOfRowsToReturnPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + noOfRowsToReturnPerRequest;

            if (string.IsNullOrEmpty(filter))
            {
                // Do not filter the point type for the time being - just display 'Any'.
                ds = facPoint.GetAllWithAddress(ePointType.Any, "", "", noOfRowsToReturnPerRequest, includeDeleted);
            }
            else if (filterString.Length == 1)
            {
                // Do not filter the point type for the time being - just display 'Any'.
                // when only one strng is entered, surely the intention is to search on the point description not the organisationname
                ds = facPoint.GetAllWithAddress(ePointType.Any, "", filterString[0], noOfRowsToReturnPerRequest, includeDeleted);
            }
            else if (filterString.Length > 1)
            {
                // Do not filter the point type for the time being - just display 'Any'.
                ds = facPoint.GetAllWithAddress(ePointType.Any, filterString[1], filterString[0], noOfRowsToReturnPerRequest, includeDeleted);
            }

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            foreach (DataRow row in dt.Rows)
            {
                rcItem = new RadComboBoxItemData();
                PointComboItem comboItem = new PointComboItem(row);
                rcItem.Text = comboItem.SingleLineText;
                rcItem.Value = row["IdentityId"].ToString() + "," + row["PointId"];

                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetMultiTrunk(RadComboBoxContext context)
        {
            string multiTrunkDescription = context["FilterString"].ToString();
            List<Entities.MultiTrunk> multiTrunks = null;
            Orchestrator.Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();
            int noOfRowsToReturn = 20;

            multiTrunks = facMultiTrunk.GetForDescriptionFiltered(multiTrunkDescription, true, noOfRowsToReturn);

            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            foreach (Entities.MultiTrunk mt in multiTrunks)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();

                rcItem.Text = mt.HtmlTableFormattedTrunkPointDescriptionsWithTravelTimesAndMultiTrunkDescription;
                rcItem.Value = mt.MultiTrunkId.ToString();

                result.Add(rcItem);
            }
            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetPointOwner(RadComboBoxContext context)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

            Orchestrator.Facade.IReferenceData facReferenceData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllOrganisationsFiltered(context.Text, false);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetClosestTown(RadComboBoxContext context)
        {
           int countryId = 1; //UK
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            try
            {

                string countryIdString = context["CountryId"].ToString();
                countryId = Convert.ToInt32(countryIdString);

                Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
                DataSet ds = facRefData.GetTownForTownNameAndCountry(context.Text, countryId);

                int itemsPerRequest = 20;
                int itemOffset = context.NumberOfItems;
                int endOffset = itemOffset + itemsPerRequest;
                if (endOffset > ds.Tables[0].Rows.Count)
                    endOffset = ds.Tables[0].Rows.Count;

                DataTable dt = ds.Tables[0];
                RadComboBoxItemData rcItem = null;

                for (int i = itemOffset; i < endOffset; i++)
                {
                    rcItem = new RadComboBoxItemData();
                    rcItem.Text = dt.Rows[i]["Description"].ToString();
                    rcItem.Value = dt.Rows[i]["TownId"].ToString();
                    result.Add(rcItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetClosestTownNoCountry(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            try
            {

                Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
                DataSet ds = facRefData.GetTownForTownName(context.Text);

                int itemsPerRequest = 20;
                int itemOffset = context.NumberOfItems;
                int endOffset = itemOffset + itemsPerRequest;
                if (endOffset > ds.Tables[0].Rows.Count)
                    endOffset = ds.Tables[0].Rows.Count;

                DataTable dt = ds.Tables[0];
                RadComboBoxItemData rcItem = null;

                for (int i = itemOffset; i < endOffset; i++)
                {
                    rcItem = new RadComboBoxItemData();
                    rcItem.Text = dt.Rows[i]["Description"].ToString();
                    rcItem.Value = dt.Rows[i]["TownId"].ToString();
                    result.Add(rcItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetTrailers(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            string[] clientArgs = context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;
            controlAreaId = int.Parse(clientArgs[0]);

            string[] taids = clientArgs[1].Split(',');
            int[] trafficAreas = new int[taids.Length];
            for (int i = 0; i < taids.Length; i++)
            {
                trafficAreas[i] = int.Parse(taids[i]);
            }

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(context.Text, eResourceType.Trailer, controlAreaId, trafficAreas, true);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public string GetAddressForPoint(int pointID)
        {
            string address = string.Empty;
            Orchestrator.Facade.Point facPoint = new Orchestrator.Facade.Point();
            Entities.Point point = facPoint.GetPointForPointId(pointID);
            address = point.Address.ToString();
            address = address.Replace("\n", "");
            address = address.Replace("\r", "<br>");
            return address;
        }


        [WebMethod]
        public RadComboBoxItemData[] GetDrivers(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            string[] clientArgs = context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;

            controlAreaId = int.Parse(clientArgs[0]);

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(context.Text, eResourceType.Driver, controlAreaId,false, true);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                rcItem.Attributes.Add("usualVehicle", dt.Rows[i]["RegNo"].ToString());
                rcItem.Attributes.Add("usualVehicleID", dt.Rows[i]["UsualVehicleID"].ToString());
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetDriversForPlanner(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            string[] clientArgs = context["FilterString"].ToString().Split(':');
            int plannerIdentityID = 0;

            plannerIdentityID = int.Parse(clientArgs[0]);
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IDriverRepository>(uow);
                var drivers =
                   from d in repo.GetAll()
                   where d.PlannerIdentityID == plannerIdentityID
                   orderby d.Individual.LastName
                   select new Telerik.Web.UI.RadComboBoxItemData()
                   {
                       Text = d.Individual.FirstNames + " " + d.Individual.LastName,
                       Value = d.ResourceID.ToString()
                   };

                result.AddRange(drivers);

            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetAllDrivers(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(context.Text, eResourceType.Driver, false);

            var itemsPerRequest = context.ContainsKey("ItemsPerRequest") ? (int)context["ItemsPerRequest"] : 20;

            DataTable dt = ds.Tables[0];
            var itemCount = dt.Rows.Count;

            int itemOffset = context.NumberOfItems;
            int endOffset = itemsPerRequest == 0 ? itemCount : itemOffset + itemsPerRequest;

            if (endOffset > itemCount)
                endOffset = itemCount;

            for (int i = itemOffset; i < endOffset; i++)
            {
                var rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetAllDriversAndSubbys(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllDriversFiltered(context.Text);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                rcItem.Attributes.Add("IsSubcontractor", dt.Rows[i]["IsSubcontractor"].ToString());
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetVehicles(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            // If the context contains a "TopItemText" key then add to a the top of the result list an item containing this text (for example "- all -" or "- select -")
            // But only if the list is currently empty. If they have clicked the ShowMoreResultsBox arrow, we don't want to add another "- all -"
            bool hasTopItemText = false;
            var topItemText = context.ContainsKey("TopItemText") ? context["TopItemText"].ToString() : string.Empty;
            hasTopItemText = !string.IsNullOrWhiteSpace(topItemText);                

            if (hasTopItemText && context.NumberOfItems == 0)
                result.Add(new Telerik.Web.UI.RadComboBoxItemData { Text = topItemText });

            string[] clientArgs = context["FilterString"].ToString().Split(':');
            int depotID = int.Parse(clientArgs[0]);

            var query = (context.ContainsKey("Text") && context.Text != topItemText) ? context.Text : string.Empty;

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(query, eResourceType.Vehicle, depotID, false, true);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;

            // If the list contains a Top Item (e.g. -All-) and the list is already populated (the ShowMoreResultsBox has been clicked), do not include the Top Item or the offset will skip a vehicle.
            if (hasTopItemText && context.NumberOfItems > 0)
                itemOffset--;

            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetAllTrailers(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            string[] clientArgs = context["FilterString"].ToString().Split(':');
            int depotID = 0;
            depotID = int.Parse(clientArgs[0]);


            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(context.Text, eResourceType.Trailer,depotID,false, true);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetSubContractors(RadComboBoxContext context)
        {

            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(context.Text, false);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetClients(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();

            // If the context contains a "TopItemText" key then add to a the top of the result list an item containing this text (for example "- all -" or "- select -")
            var topItemText = context.ContainsKey("TopItemText") ? context["TopItemText"].ToString() : string.Empty;

            if (!string.IsNullOrWhiteSpace(topItemText))
                result.Add(new Telerik.Web.UI.RadComboBoxItemData { Text = topItemText });

            bool includeSuspended = false;
            if (context.ContainsKey("DisplaySuspended"))
                includeSuspended = context["DisplaySuspended"] == null ? false : context["DisplaySuspended"].ToString() == "False" ? false : true;

            var query = (context.ContainsKey("Text") && context.Text != topItemText) ? context.Text : string.Empty;

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(query, includeSuspended);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetOrganisations(RadComboBoxContext context)
        {
            List<RadComboBoxItemData> result = new List<RadComboBoxItemData>();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(context.Text, false);

            int itemsPerRequest = 20;
            int itemOffset = context.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItemData rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItemData();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                result.Add(rcItem);
            }

            return result.ToArray();
        }
    }
}
