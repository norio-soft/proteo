using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Drawing;
using Orchestrator.WebUI.Services;
using Orchestrator.WebUI.Code.components;
using Orchestrator.Models;
using Orchestrator.Repositories;
using Orchestrator.BusinessLogicLayer;
using Orchestrator.Exceptions;

namespace Orchestrator.WebUI.CAN
{
    public partial class AddUpdateOrgUnit : Orchestrator.Base.BasePage
    {
        #region private classes

        //Class to the hold the data for the list
        private class DisplayResource
        {
            public int ResourceId { get; set; }
            public string Description { get; set; }
            public string GpsUnitId { get; set; }
        }

        #endregion

        #region private members

        private List<EF.OrgUnit> _orgUnits;
        private List<EF.Resource> _resources;
        private List<EF.OrgUnitResource> _orgUnitResources;
        private List<FleetViewTreeViewNode> _allVehicles = new List<FleetViewTreeViewNode>();

        #endregion

        #region page init

        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!this.IsPostBack && !this.IsCallback)
            {
                LoadOrgUnitTree();
                LoadResourceList(); 
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            OrgUnitTree.NodeDataBound += new RadTreeViewEventHandler(OrgUnitTree_NodeDataBound);
            OrgUnitTree.NodeEdit += new RadTreeViewEditEventHandler(OrgUnitTree_NodeEdit);
            OrgUnitTree.NodeClick += new RadTreeViewEventHandler(OrgUnitTree_NodeClick);

            VehiclesRadioButton.CheckedChanged += new EventHandler(ResourceRadioButton_CheckedChanged);
            DriversRadioButton.CheckedChanged += new EventHandler(ResourceRadioButton_CheckedChanged);
            cbOnlyOrphans.CheckedChanged += new EventHandler(OrhpanCheckBox_CheckedChanged);
            btnSaveChanges.Click += new EventHandler(btnSaveChanges_Click);
            this.btnAddNode.Click += new EventHandler(btnAddNode_Click);
            this.btnDeleteNode.Click += new EventHandler(btnDeleteNode_Click);
        }

        #endregion

        #region event handlers

        void OrhpanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadResourceList();
        }

        void btnDeleteNode_Click(object sender, EventArgs e)
        {
            errorText.InnerText = "";
            errorText.Visible = false;

            if (OrgUnitTree.SelectedNode != null && int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]) > 0)
            {
                int orgUnitID = int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]);
                EF.OrgUnit orgUnit = EF.DataContext.Current.OrgUnits.Include("OrgUnitResources").First(ou => ou.OrgUnitId == orgUnitID);

                var resourcesToUpdate = EF.DataContext.Current.OrgUnitResources.Include("Resource").Include("OrgUnitresources.resource.ResourceType");
                var driversToUpdate = resourcesToUpdate.Where(x => x.OrgUnitId == orgUnitID && x.Resource.ResourceType.ResourceTypeId == (int)eResourceType.Driver).Select(d => d.ResourceId).ToList();
                var vehiclesToUpdate = resourcesToUpdate.Where(x => x.OrgUnitId == orgUnitID && x.Resource.ResourceType.ResourceTypeId == (int)eResourceType.Vehicle).Select(v => v.ResourceId).ToList();

                if (orgUnit != null)
                {
                    EF.DataContext.Current.DeleteObject(orgUnit);
                    EF.DataContext.Current.SaveChanges();
                }

                UpdateThirdPartyTelematicsDrivers(driversToUpdate);
                UpdateThirdPartyTelematicsVehicles(vehiclesToUpdate);

            }
            LoadOrgUnitTree();
            LoadResourceList();



        }

        void btnAddNode_Click(object sender, EventArgs e)
        {

            if (txtNodeName.Text.Length == 0) return;

            // If the user has not specified a Node, select the top level node by default.
            if (OrgUnitTree.SelectedNode == null)
                OrgUnitTree.Nodes[0].Selected = true;

            EF.OrgUnit newUnit = new EF.OrgUnit();
            newUnit.Name = txtNodeName.Text;
            newUnit.ParentOrgUnitId = int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]);
            newUnit.CreateDate = DateTime.Now;
            newUnit.CreateUserId = Page.User.Identity.Name;
            newUnit.LastUpdateDate = DateTime.Now;
            newUnit.LastUpdateUserId = Page.User.Identity.Name;
            EF.DataContext.Current.AddToOrgUnits(newUnit);
            EF.DataContext.Current.SaveChanges();

            txtNodeName.Text = string.Empty;
            LoadOrgUnitTree();
            LoadResourceList();
        }

        void OrgUnitTree_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            LoadOrgUnitTree();
            LoadResourceList();
        }

        void ResourceListBox_ItemDataBound(object sender, RadListBoxItemEventArgs e)
        {
            DisplayResource res = e.Item.DataItem as DisplayResource;
            if (!String.IsNullOrEmpty(res.GpsUnitId))
                e.Item.Text += " *";
        }

        void ResourceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (VehiclesRadioButton.Checked)
            {
                txtResourceType.Text = "Vehicles not in this Group";
                txtResourceTypeList.Text = "Vehicles in this Group";
            }
            else
            {
                txtResourceType.Text = "Drivers not in this Group";
                txtResourceTypeList.Text = "Drivers in this Group";
            }
     
            LoadResourceList();
            LoadOrgUnitTree();
        } 

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {

            errorText.InnerText = "";
            errorText.Visible = false;

            // get the index of the selected node as this might not be saved
            string selectedNodeText = string.Empty;
            int orgUnitID = 0;
            if (OrgUnitTree.SelectedNode != null)
            {
                int selectedNodeIndex = this.OrgUnitTree.SelectedNode.Index;
                selectedNodeText = this.OrgUnitTree.SelectedNode.Text;
                
                if (this.OrgUnitTree.SelectedNode.HasAttributes)
                {
                    int.TryParse(this.OrgUnitTree.SelectedNode.Attributes["OrgUnitID"], out orgUnitID);
                }
            }

            _orgUnits = EF.DataContext.Current.OrgUnits.Include("OrgUnitResources").ToList();
            _resources = EF.DataContext.Current.Resources.Include("OrgUnitResources").ToList();



            UpdateOrgUnits(null, OrgUnitTree.Nodes, false);

            _orgUnits = EF.DataContext.Current.OrgUnits.Include("OrgUnitResources")
                                                       .Include("OrgUnitResources.Resource")
                                                       .Include("OrgUnitresources.resource.ResourceType").ToList();
            _resources = EF.DataContext.Current.Resources.Include("OrgUnitResources").ToList();


            List<int> modifiedResourceIDs = new List<int>();

            if (!string.IsNullOrEmpty(selectedNodeText))
            {
                EF.OrgUnit orgUnit;
                if (orgUnitID > 0)
                {
                    orgUnit = _orgUnits.First(ou => ou.OrgUnitId == orgUnitID);
                    modifiedResourceIDs.AddRange(RemoveResourcesFromOrgUnit(orgUnit));                   
                }
                else
                {
                    orgUnit = _orgUnits.First(ou => ou.Name == selectedNodeText);
                }

                 modifiedResourceIDs.AddRange(AddResourcesToOrgUnit(orgUnit));

            }
            //Save the changes
            EF.DataContext.Current.SaveChanges();
            //Reload the data
            LoadResourceList();
            LoadOrgUnitTree();

            if ((VehiclesRadioButton.Checked))
            {
                UpdateThirdPartyTelematicsVehicles(modifiedResourceIDs);
            }
            else
            {
                UpdateThirdPartyTelematicsDrivers(modifiedResourceIDs);
            }

        }

        /// <summary>
        /// Finds the resource units which have been newly added into an org unit and
        /// creates links in the OrgUnitResource table to that effect
        /// Returns the IDs of resources added
        /// </summary>
        /// <param name="orgUnit"></param>
        private IEnumerable<int> AddResourcesToOrgUnit(EF.OrgUnit orgUnit)
        {
            List<int> addedResourceIDs = new List<int>();

            foreach (RadListBoxItem item in lbResourcesInGroup.Items)
            {
                int resourceId = int.Parse(item.Value);
                EF.Resource resource = _resources.FirstOrDefault(r => r.ResourceId == resourceId);
                //If the resource was found and its OrgUnit has changed, update it
                if (resource != null && resource.OrgUnitResources == null || !resource.OrgUnitResources.Any(or => or.OrgUnitId == orgUnit.OrgUnitId))
                {
                    EF.OrgUnitResource our = new EF.OrgUnitResource();
                    our.OrgUnit = orgUnit;
                    our.Resource = resource;
                    our.CreateUserId = this.Page.User.Identity.Name;
                    our.CreateDateTime = DateTime.Now;
                    our.LastUpdateDate = DateTime.Now;
                    our.LastUpdateUserId = this.Page.User.Identity.Name;
                    addedResourceIDs.Add(resourceId);

                    EF.DataContext.Current.SaveChanges();
                }
            }

            return addedResourceIDs;
        }

        /// <summary>
        /// Finds the resource units which have been newly removed from an org unit
        /// and removes links in the OrgUnitResource table to that effect.
        /// Returns the IDs of resources removed
        /// </summary>
        /// <param name="orgUnit"></param>
        private IEnumerable<int> RemoveResourcesFromOrgUnit(EF.OrgUnit orgUnit)
        {

            var selectedResourceTypeid = VehiclesRadioButton.Checked ? 1 : 3;
            var selectedResourceType = new EF.ResourceType() { ResourceTypeId = selectedResourceTypeid };

            var resourcesInDB = orgUnit.OrgUnitResources.Where(ou => ou.Resource.ResourceType.ResourceTypeId == selectedResourceTypeid && ou.OrgUnitId == orgUnit.OrgUnitId).ToList();

            var idsOfResourcesNotInThisGroup = lbResourcesNotInGroup.Items.Select(x => int.Parse(x.Value));
            var resourcesToRemove = resourcesInDB.Where(x => idsOfResourcesNotInThisGroup.Contains(x.ResourceId));

            foreach (var item in resourcesToRemove)
            {
                orgUnit.OrgUnitResources.Remove(item);
            }
            EF.DataContext.Current.SaveChanges();

            return resourcesToRemove.Select(x => x.ResourceId);
        }


        #endregion


        #region context menu

        protected void OrgUnitTree_NodeEdit(object sender, RadTreeNodeEditEventArgs e)
        {
            errorText.InnerText = "";
            errorText.Visible = false;

            //It seems that it is not enough for the edit to just happen client side.
            //If this is not done then the changed text reverts after a postback
            RadTreeNode nodeEdited = e.Node;
            nodeEdited.Text = e.Text;

            int orgUnitID = int.Parse(nodeEdited.Attributes["OrgUnitId"]);
            EF.OrgUnit orgUnit = EF.DataContext.Current.OrgUnits.Include("OrgUnitResources").First(ou => ou.OrgUnitId == orgUnitID);

            var resourcesToUpdate = EF.DataContext.Current.OrgUnitResources.Include("Resource").Include("OrgUnitresources.resource.ResourceType");
            var driversToUpdate = resourcesToUpdate.Where(x => x.OrgUnitId == orgUnitID && x.Resource.ResourceType.ResourceTypeId == (int)eResourceType.Driver).Select(d => d.ResourceId).ToList();
            var vehiclesToUpdate = resourcesToUpdate.Where(x => x.OrgUnitId == orgUnitID && x.Resource.ResourceType.ResourceTypeId == (int)eResourceType.Vehicle).Select(v => v.ResourceId).ToList();

            UpdateThirdPartyTelematicsDrivers(driversToUpdate);
            UpdateThirdPartyTelematicsVehicles(vehiclesToUpdate);

        }

       

        private void RemoveResources(RadTreeNode parentNode)
        {
            _orgUnits = EF.DataContext.Current.OrgUnits.Include("OrgUnitResources").ToList();
            _resources = EF.DataContext.Current.Resources.Include("OrgUnitResources").ToList();

            RemoveResources(parentNode, null);

            //Save the chnages
            EF.DataContext.Current.SaveChanges();
        }
    
        private void RemoveResources(RadTreeNode parentNode, int? insertAfter)
        {
            //Tghe node being deleted may be a Resource so do it for the parent first
            if (!RemoveResource(parentNode, insertAfter))
            {
                //Use a copy of the nodes coleection so that we can remove from the parent node
                //without getting an error
                List<RadTreeNode> nodes = new List<RadTreeNode>();
                foreach (RadTreeNode node in parentNode.Nodes)
                    nodes.Add(node);

                //foreach (RadTreeNode node in nodes)
                foreach (RadTreeNode node in nodes)
                {
                    RemoveResources(node, insertAfter);
                }
            }
        }

        private bool RemoveResource(RadTreeNode node, int? insertAfter)
        {
            TreeDataNodeType dataNodeType = (TreeDataNodeType)Enum.Parse(typeof(TreeDataNodeType), node.Attributes["DataNodeType"], true);

            //If the node is a resource add it to the list and remove it from the tree
            //otherwise process its children
            if (dataNodeType != TreeDataNodeType.OrgUnit)
            {
                int orgUnitId = Convert.ToInt32(node.ParentNode.Attributes["OrgUnitId"]);
                int resourceId = Convert.ToInt32(node.Attributes["ResourceId"]);

                EF.Resource resource = _resources.FirstOrDefault(r => r.ResourceId == resourceId); 
                if(resource != null)
                {
                    EF.OrgUnitResource orgResource = resource.OrgUnitResources.FirstOrDefault(or => or.OrgUnitId == orgUnitId);
                    if(orgResource != null)
                        EF.DataContext.Current.DeleteObject(orgResource);
                }

                node.Remove();
                return true;
            }

            return false;
        }

        #endregion

        #region load data
        
        private void LoadOrgUnitTree()
        {
            List<TreeDataNode> dataNodes = new List<TreeDataNode>();
            int selectedOrgUnitID = 0;
            string selectedNodePath = string.Empty;
            if (OrgUnitTree.SelectedNode != null)
            {
                selectedNodePath = OrgUnitTree.SelectedNode.Text;
                selectedOrgUnitID = int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]);
            }

            //Get the OrgUnits
            var orgUnits = (from ou in EF.DataContext.Current.OrgUnits
                            orderby ou.Name
                            select new TreeDataNode
                            {
                                OrgUnitId = ou.OrgUnitId,
                                ParentOrgUnitId = ou.ParentOrgUnitId,
                                Text = ou.Name
                            }).ToList();

            //Have to set this after the LINQ as DataNodeType is not a primitive type
            orgUnits.ForEach(ou => ou.Type = TreeDataNodeType.OrgUnit);
            
            //If no OrgUnits have been created create a new root
            //(An empty database should always have a root 
            if (orgUnits.Count == 0)
            {
                orgUnits.Add(new TreeDataNode
                    {
                        Text = "Your Company or Group",
                        Type = TreeDataNodeType.OrgUnit
                    });
            }

            dataNodes.AddRange(orgUnits);

            //Bind the data to the tree
            OrgUnitTree.DataSource = dataNodes;
            OrgUnitTree.DataBind();
            OrgUnitTree.ExpandAllNodes();
            if (selectedNodePath != string.Empty)
            {
                try
                {
                    OrgUnitTree.FindNodeByText(selectedNodePath).Selected = true;
                }
                catch { }
            }
            else
            {
                OrgUnitTree.Nodes[0].Selected = true;
                selectedNodePath = OrgUnitTree.SelectedNode.Text;
                selectedOrgUnitID = int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]);
            }

            if (selectedOrgUnitID > 0)
            {
                //If selected, get the Vehicles
                if (VehiclesRadioButton.Checked)
                {
                    var vehicles = (
                        from v in EF.DataContext.Current.Vehicles
                        join our in EF.DataContext.Current.OrgUnitResources on v.ResourceId equals our.ResourceId
                        join r in EF.DataContext.Current.Resources on v.ResourceId equals r.ResourceId
                        where v.Resource.OrgUnitResources.Count > 0 && our.OrgUnitId == selectedOrgUnitID && r.ResourceStatus.ResourceStatusId != 2


                        orderby v.RegNo
                        select new TreeDataNode
                        {
                            ResourceId = v.ResourceId,
                            ParentOrgUnitId = our.OrgUnitId,
                            Text = v.Resource.GPSUnitID != "" ? v.RegNo + " *" : v.RegNo,
                        }).ToList();

                    vehicles.ForEach(ou => ou.Type = TreeDataNodeType.Vehicle);
                    lbResourcesInGroup.DataSource = vehicles;
                }

                //If selected, get the Drivers
                if (DriversRadioButton.Checked)
                {
                    var drivers = (
                        from d in EF.DataContext.Current.Drivers
                        join our in EF.DataContext.Current.OrgUnitResources on d.ResourceId equals our.ResourceId
                        join r in EF.DataContext.Current.Resources on d.ResourceId equals r.ResourceId
                        where d.Resource.OrgUnitResources.Count > 0 && our.OrgUnitId == selectedOrgUnitID && r.ResourceStatus.ResourceStatusId != 2
                        orderby d.Individual.LastName, d.Individual.FirstNames
                        select new TreeDataNode
                        {
                            ResourceId = d.ResourceId,
                            ParentOrgUnitId = our.OrgUnitId,
                            Text = d.Individual.LastName + ", " + d.Individual.FirstNames
                        }).ToList();

                    drivers.ForEach(ou => ou.Type = TreeDataNodeType.Driver);
                    lbResourcesInGroup.DataSource = drivers;
                }

                lbResourcesInGroup.DataTextField = "Text";
                lbResourcesInGroup.DataValueField = "ResourceID";
                lbResourcesInGroup.DataBind();
            }

        }

        void OrgUnitTree_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            TreeDataNode dataNode = (TreeDataNode)e.Node.DataItem;

            if (dataNode.Type == TreeDataNodeType.OrgUnit)
                e.Node.Attributes["OrgUnitId"] = dataNode.OrgUnitId.ToString();
            else
                e.Node.Attributes["ResourceId"] = dataNode.ResourceId.ToString();

            e.Node.Attributes["DataNodeType"] = dataNode.Type.ToString();
        }

        private void LoadResourceList()
        {
            List<DisplayResource> resources = null;
            int selectedOrgUnitID = 0;
            string selectedNodePath = string.Empty;

            if (OrgUnitTree.SelectedNode != null)
            {
                selectedNodePath = OrgUnitTree.SelectedNode.Text;
                selectedOrgUnitID = int.Parse(OrgUnitTree.SelectedNode.Attributes["OrgUnitId"]);
            }

            if (VehiclesRadioButton.Checked)
            {
                resources = (
                    from v in EF.DataContext.Current.Vehicles.Include("Resource")
                    where v.Resource.ResourceStatus.ResourceStatusId != 2 &&
                        (
                            (!cbOnlyOrphans.Checked && !EF.DataContext.Current.OrgUnitResources.Any(our => our.OrgUnitId == selectedOrgUnitID && our.ResourceId == v.ResourceId))
                            || 
                            (cbOnlyOrphans.Checked && !EF.DataContext.Current.OrgUnitResources.Any(our => our.ResourceId == v.ResourceId))
                        )
                    orderby v.RegNo
                    select new DisplayResource
                    {
                        ResourceId = v.ResourceId,
                        Description = v.Resource.GPSUnitID != "" ? v.RegNo + " *" : v.RegNo,
                        GpsUnitId = v.Resource.GPSUnitID
                    }
                ).ToList();
            }

            if (DriversRadioButton.Checked)
            {
                resources = (
                    from d in EF.DataContext.Current.Drivers.Include("Resource")
                    where d.Resource.ResourceStatus.ResourceStatusId != 2 &&
                        (
                            (!cbOnlyOrphans.Checked && !EF.DataContext.Current.OrgUnitResources.Any(our => our.OrgUnitId == selectedOrgUnitID && our.ResourceId == d.ResourceId))
                            ||
                            (cbOnlyOrphans.Checked && !EF.DataContext.Current.OrgUnitResources.Any(our => our.ResourceId == d.ResourceId))
                        )
                    orderby d.Individual.LastName, d.Individual.FirstNames 
                    select new DisplayResource
                    {
                        ResourceId = d.ResourceId,
                        Description = d.Individual.LastName + ", " + d.Individual.FirstNames,
                        GpsUnitId = String.Empty
                    }
                ).ToList();
            }
            lbResourcesNotInGroup.Items.Clear();
            lbResourcesNotInGroup.DataSource = resources;
            lbResourcesNotInGroup.DataBind();

        }

        #endregion

        #region private methods


        private void UpdateOrgUnits(int? ParentOrgUnitId, RadTreeNodeCollection Nodes, bool delete)
        {
            //Ideally OrgUnit would have an Entity Reference to itself which would mean a sub tree could be deleted
            //by simply deleting it's the root node and we would not need to SaveChanges for every new node just to get
            //the OrgUnitId to pass down the tree. However, this jhas not been done as the RadTree requires a ParentId and Id
            //to do its binding. EF4 will fix this by allowing us to have both an FK Id AND an Entity Reference
            foreach (RadTreeNode node in Nodes)
            {
                TreeDataNodeType dataNodeType = (TreeDataNodeType)Enum.Parse(typeof(TreeDataNodeType), node.Attributes["DataNodeType"], true);
                
                //If Node is an OrgUnit
                if (dataNodeType == TreeDataNodeType.OrgUnit)
                {
                    int orgUnitId = 0;
                    EF.OrgUnit orgUnit = null;

                    //New nodes do not have a OrgUnitId
                    if (!string.IsNullOrEmpty(node.Attributes["OrgUnitId"]))
                    {
                        orgUnitId = int.Parse(node.Attributes["OrgUnitId"]);
                        orgUnit = _orgUnits.FirstOrDefault(ou => ou.OrgUnitId == orgUnitId);
                    }

                    //If the node has been deleted or one of its ancestors, delete it if it is NOT new
                    //then delete its children
                    if (node.Visible == false || delete)
                    {
                        if (orgUnit != null)
                        {
                            foreach (EF.OrgUnitResource our in orgUnit.OrgUnitResources)
                                EF.DataContext.Current.DeleteObject(our);

                            EF.DataContext.Current.DeleteObject(orgUnit);
                            EF.DataContext.Current.SaveChanges();
                        }
                    }
                    //If not deleting, then add or update
                    else
                    {
                        //Create the OrgUnit if it's new
                        if (orgUnit == null)
                        {
                            orgUnit = new EF.OrgUnit();
                            EF.DataContext.Current.AddToOrgUnits(orgUnit);
                            _orgUnits.Add(orgUnit);
                        }

                        //Update changed properties
                        if (orgUnit.ParentOrgUnitId != ParentOrgUnitId)
                            orgUnit.ParentOrgUnitId = ParentOrgUnitId;

                        if (orgUnit.Name != node.Text)
                            orgUnit.Name = node.Text;

                        EF.DataContext.Current.SaveChanges();

                        UpdateOrgUnits(orgUnit.OrgUnitId, node.Nodes, false);
                    }
                }
                else
                { 
                    
                }
            }

        }

        /// <summary>
        /// Updates any thirdpaty telematics solutions with modified vehicle details
        /// </summary>
        /// <param name="modifiedResourceIDs"></param>
        private void UpdateThirdPartyTelematicsVehicles(IEnumerable<int> modifiedResourceIDs)
        {
            if (!Globals.Configuration.SendUpdatesToThirdPartyTelematics || !modifiedResourceIDs.Any())
                return;

            IEnumerable<Models.Vehicle> resourcesToUpdate;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IVehicleRepository>(uow);
                resourcesToUpdate = repo.GetAll().Where(r => modifiedResourceIDs.Contains(r.ResourceID) && r.Resource.TelematicsSolution != null && r.Resource.TelematicsSolution != eTelematicsSolution.FleetMetrik).ToList();

                try 
                {
                    if (resourcesToUpdate.Any())
                    {
                        UpdateMicroliseVehicles(resourcesToUpdate.Where(r => r.Resource.TelematicsSolution == eTelematicsSolution.Microlise && !string.IsNullOrEmpty(r.Resource.GPSUnitID)));                    
                        // Other telemetics solutions can go here
                    }
                }
                catch (ThirdPartyTelematicsException ex)
                {
                    errorText.InnerText = "Unable to update thirdparty telematics solutions. Modified drivers/vehicles may now give inaccurate GPS data.";
                    errorText.Visible = true;
                }

            }

        }

        /// <summary>
        /// Updates any thirdpaty telematics solutions with modified driver details
        /// </summary>
        /// <param name="modifiedResourceIDs"></param>
        private void UpdateThirdPartyTelematicsDrivers(IEnumerable<int> modifiedResourceIDs)
        {
            if (!Globals.Configuration.SendUpdatesToThirdPartyTelematics || !modifiedResourceIDs.Any())
                return;

            IEnumerable<Models.Driver> resourcesToUpdate;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IDriverRepository>(uow);
                resourcesToUpdate = repo.GetAll().Where(r => modifiedResourceIDs.Contains(r.ResourceID) && r.Resource.TelematicsSolution != null && r.Resource.TelematicsSolution != eTelematicsSolution.FleetMetrik).ToList();

                try
                {
                    if (resourcesToUpdate.Any())
                    {
                        UpdateMicroliseDrivers(resourcesToUpdate.Where(r => r.Resource.TelematicsSolution == eTelematicsSolution.Microlise && !String.IsNullOrEmpty(r.DigitalTachoCardID)));
                        // Other telemetics solutions can go here
                    }
                }
                catch (ThirdPartyTelematicsException ex)
                {
                    errorText.InnerText = "Unable to update thirdparty telematics solutions. Modified drivers/vehicles may now give inaccurate GPS data.";
                    errorText.Visible = true;
                }

            }

        }

        private void UpdateMicroliseVehicles(IEnumerable<Models.Vehicle> vehicles)
        {
            MicroliseIntegration microlise = new MicroliseIntegration();
            microlise.UpdateVehicles(vehicles);
        }

        private void UpdateMicroliseDrivers(IEnumerable<Models.Driver> drivers)
        {
            MicroliseIntegration microlise = new MicroliseIntegration();
            microlise.UpdateDrivers(drivers);
        }
  
        #endregion

    }
}