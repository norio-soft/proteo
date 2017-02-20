namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

    /// <summary>
    ///	Summary description for Point.
    /// </summary>
    public partial class Point : System.Web.UI.UserControl, IDefaultButton
    {

        #region IDefaultButton
        public System.Web.UI.Control DefaultButton
        {
            get { return this.btnNext; }
        }
        #endregion
        #region Form Elements






        #endregion

        #region Page Variables

        private Entities.Job m_job;
        private int m_jobId;
        private bool m_isUpdate = false;
        private bool m_isAmendment = false;

        private Entities.Instruction m_instruction;
        private int m_instructionIndex;

        // Static variables used to help maintain DbCombo state
        protected int m_identityId = 0;
        protected string m_owner = string.Empty;
        protected int m_townId = 0;
        protected string m_town = String.Empty;
        protected int m_pointId = 0;
        protected string m_point = String.Empty;

        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (m_jobId > 0)
                m_isUpdate = true;

            // Retrieve the job from the session variable
            m_job = (Entities.Job)Session[wizard.C_JOB];

            if (Session[wizard.C_INSTRUCTION_INDEX] != null)
            {
                m_instructionIndex = (int)Session[wizard.C_INSTRUCTION_INDEX];

                if (!m_isUpdate && m_instructionIndex != m_job.Instructions.Count)
                    m_isAmendment = true;
            }

            if (!IsPostBack)
            {
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                if (Session[wizard.C_INSTRUCTION] != null)
                {
                    m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

                    switch ((eInstructionType)m_instruction.InstructionTypeId)
                    {
                        case eInstructionType.Load:
                            // Allow the user to collect from any location if this is a stock movement job.
                            if (m_job.IsStockMovement)
                                m_identityId = 0;
                            else
                                m_identityId = m_job.IdentityId;
                            lblCollection.Visible = true;
                            lblCollectFrom.Visible = true;
                            lblDelivery.Visible = false;
                            lblDeliverTo.Visible = false;
                            lblCollectDrop.Text = "Collection Details";
                            break;
                        case eInstructionType.Drop:
                            m_identityId = m_instruction.ClientsCustomerIdentityID;
                            lblCollection.Visible = false;
                            lblCollectFrom.Visible = false;
                            lblDelivery.Visible = true;
                            lblDeliverTo.Visible = true;
                            lblCollectDrop.Text = "Delivery Details";

                            m_owner = facOrganisation.GetNameForIdentityId(m_identityId);
                            cboOwner.Text = m_owner;
                            cboOwner.SelectedValue = m_identityId.ToString();
                            cboOwner.Visible = false;
                            lblOwner.Visible = true;
                            lblOwner.Text = m_owner;
                            break;
                    }
                }
                else
                {
                    m_instruction = new Entities.Instruction();
                    m_instruction.JobId = m_jobId;

                    if (m_job.Instructions == null)
                        m_job.Instructions = new Entities.InstructionCollection();

                    switch ((ePointType)Session[wizard.C_POINT_TYPE])
                    {
                        case ePointType.Collect:
                            // Allow the user to collect from any location if this is a stock movement job.
                            if (m_job.IsStockMovement)
                                m_identityId = 0;
                            else
                                m_identityId = m_job.IdentityId;

                            m_owner = facOrganisation.GetNameForIdentityId(m_job.IdentityId);
                            cboOwner.Text = m_owner;
                            cboOwner.SelectedValue = m_identityId.ToString();

                            m_instruction.InstructionTypeId = (int)eInstructionType.Load;
                            lblCollection.Visible = true;
                            lblCollectFrom.Visible = true;
                            lblDelivery.Visible = false;
                            lblDeliverTo.Visible = false;
                            lblCollectDrop.Text = "Collection Details";

                            #region Load the Default Collection Point

                            try
                            {
                                Entities.Organisation client = facOrganisation.GetForIdentityId(m_job.IdentityId);
                                Facade.IPoint facPoint = new Facade.Point();
                                Entities.Point defaultPoint = facPoint.GetPointForPointId(client.Defaults[0].DefaultCollectionPointId);
                                cboTown.SelectedValue = defaultPoint.PostTown.TownId.ToString();
                                cboTown.Text = defaultPoint.PostTown.TownName;
                                cboPoint.SelectedValue = defaultPoint.PointId.ToString();
                                cboPoint.Text = defaultPoint.Description;

                                m_identityId = defaultPoint.IdentityId;
                                m_owner = defaultPoint.OrganisationName;
                                m_pointId = defaultPoint.PointId;
                                m_point = defaultPoint.Description;
                                m_townId = defaultPoint.PostTown.TownId;
                                m_town = defaultPoint.PostTown.TownName;

                                cboOwner.Text = m_owner;
                                cboOwner.SelectedValue = m_identityId.ToString();
                            }
                            catch { }

                            #endregion
                            break;
                    }

                    Session[wizard.C_INSTRUCTION] = m_instruction;

                    if (!m_isUpdate)
                        Session[wizard.C_INSTRUCTION_INDEX] = m_job.Instructions.Count;
                }

                if (m_instruction != null && m_instruction.Point != null)
                {
                    cboOwner.SelectedValue = m_instruction.Point.IdentityId.ToString();
                    cboOwner.Text = m_instruction.Point.OrganisationName;

                    cboPoint.SelectedValue = m_instruction.PointID.ToString();
                    cboPoint.Text = m_instruction.Point.Description;

                    cboTown.SelectedValue = m_instruction.Point.PostTown.TownId.ToString();
                    cboTown.Text = m_instruction.Point.PostTown.TownName;
                }
            }
            else
                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

            pnlCreateNewPoint.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            btnBack.Click += new EventHandler(btnBack_Click);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            cfvOwner.ServerValidate += new ServerValidateEventHandler(cfvOwner_ServerValidate);
            this.cboOwner.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboOwner_ItemsRequested);
            this.cboPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);
            this.cboTown.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTown_ItemsRequested);
            this.cboPoint.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboPoint_SelectedIndexChanged);
            this.cboPoint.OnClientItemsRequesting = this.cboPoint.ClientID + "_Requesting";
        }

        void cboOwner_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboOwner.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllOrganisationsFiltered(e.Text, false);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboOwner.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboTown.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetTownForTownName(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["TownId"].ToString();
                cboTown.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cfvOwner_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (cboOwner.SelectedValue == string.Empty)
            {
                // A new organisation is being created.
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Entities.Organisation existingOrganisation = facOrganisation.GetForName(cboOwner.Text);
                args.IsValid = existingOrganisation == null;
            }
        }

        void cboPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            // Set the identity id to use for the point filtering
            int identityId = 0;
            switch ((eInstructionType)m_instruction.InstructionTypeId)
            {
                case eInstructionType.Load:
                    identityId = m_job.IdentityId;
                    break;
                case eInstructionType.Drop:
                    identityId = m_instruction.ClientsCustomerIdentityID;
                    break;
            }

            cboPoint.Items.Clear();
            int townId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString()  != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                if (values.Length > 1)
                {
                    identityId = int.Parse(values[0]);
                    townId = int.Parse(values[1]);
                    if (values.Length > 1 && values[2] != "false")
                    {
                        searchText = values[2];
                    }
                }
                else
                    searchText = e.Context["FilterString"].ToString();
            }
            else
                searchText = e.Text;

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, townId, searchText);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion

        #region Event Handlers & Events

        private void GoToStep(string step)
        {
            string url = "wizard.aspx?step=" + step;

            if (m_isUpdate)
                url += "&jobId=" + m_jobId.ToString();

            Response.Redirect(url);
        }

        void cboPoint_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            // Set the identity id to use for the point filtering
            switch ((eInstructionType)m_instruction.InstructionTypeId)
            {
                case eInstructionType.Load:
                    m_identityId = m_job.IdentityId;
                    break;
                case eInstructionType.Drop:
                    m_identityId = m_instruction.ClientsCustomerIdentityID;
                    break;
            }

            if (cboPoint.SelectedValue != String.Empty)
            {
                Facade.IPoint facPoint = new Facade.Point();
                Entities.Point selectedPoint = facPoint.GetPointForPointId(Convert.ToInt32(cboPoint.SelectedValue));

                m_identityId = selectedPoint.IdentityId;
                m_owner = selectedPoint.OrganisationName;
                m_pointId = selectedPoint.PointId;
                m_point = selectedPoint.Description;
                m_townId = selectedPoint.PostTown.TownId;
                m_town = selectedPoint.PostTown.TownName;

                cboTown.SelectedValue = selectedPoint.PostTown.TownId.ToString();
                cboTown.Text = selectedPoint.PostTown.TownName;

                cboOwner.SelectedValue = selectedPoint.IdentityId.ToString();
                cboOwner.Text = selectedPoint.OrganisationName;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            bool jumpToDetails = Session[wizard.C_JUMP_TO_DETAILS] != null && ((bool)Session[wizard.C_JUMP_TO_DETAILS]);
            Session[wizard.C_JUMP_TO_DETAILS] = false;

            if (jumpToDetails)
                GoToStep("JD");
            else if (m_job.Instructions.Count == 0)
                GoToStep("JR");
            else
                GoToStep("JD");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GoToStep("CANCEL");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (cboPoint.SelectedValue == String.Empty)
                {
                    int ownerID = 0;

                    if (cboOwner.SelectedValue == string.Empty)
                    {
                        // Create a new client customer for the point owner (this can be upgraded to client in the future).
                        Entities.Organisation organisation = new Entities.Organisation();
                        organisation.OrganisationName = cboOwner.Text;
                        organisation.OrganisationType = eOrganisationType.ClientCustomer;
                        Facade.IOrganisation facOrganisation = new Facade.Organisation();
                        Entities.FacadeResult result = facOrganisation.Create(organisation, ((Entities.CustomPrincipal)Page.User).UserName);
                        if (result.Success)
                            ownerID = result.ObjectId;
                        else
                        {
                            // Organisation has already been created by someone else between validation and this call (unlikely)
                            ownerID = facOrganisation.GetForName(cboOwner.Text).IdentityId;
                        }
                    }
                    else
                        ownerID = int.Parse(cboOwner.SelectedValue);

                    // Set the point type, organisation, town, and name for the new point
                    switch ((eInstructionType)m_instruction.InstructionTypeId)
                    {
                        case eInstructionType.Drop:
                            Session[wizard.C_POINT_TYPE] = ePointType.Deliver;
                            Session[wizard.C_POINT_FOR] = m_instruction.ClientsCustomerIdentityID;
                            break;
                        case eInstructionType.Load:
                            Session[wizard.C_POINT_TYPE] = ePointType.Collect;
                            Session[wizard.C_POINT_FOR] = ownerID;
                            break;
                    }

                    Session[wizard.C_INSTRUCTION_INDEX] = m_instructionIndex;
                    Session[wizard.C_POINT_NAME] = cboPoint.Text;
                    Session[wizard.C_TOWN_ID] = Convert.ToInt32(cboTown.SelectedValue);

                    // The point name must be unique for this client
                    bool foundPointName = false;
                    Facade.IPoint facPoint = new Facade.Point();
                    DataSet pointNames = facPoint.GetAllForOrganisation((int)Session[wizard.C_POINT_FOR], ePointType.Any, cboPoint.Text);
                    foreach (DataRow row in pointNames.Tables[0].Rows)
                    {
                        if (((string)row["Description"]) == cboPoint.Text)
                            foundPointName = true;
                    }

                    if (foundPointName)
                    {
                        pnlCreateNewPoint.Visible = true;
                    }
                    else
                    {
                        // Allow the user to create the new point
                        GoToStep("CNP");
                    }
                }
                else
                {
                    int pointId = Convert.ToInt32(cboPoint.SelectedValue);

                    if (m_isUpdate)
                    {
                        if (m_instruction.InstructionID == 0)
                        {
                            // Adding a new instruction

                            Facade.IPoint facPoint = new Facade.Point();
                            Entities.Point selectedPoint = facPoint.GetPointForPointId(pointId);

                            // Add the collect drop point information
                            m_instruction.Point = selectedPoint;
                            m_instruction.PointID = selectedPoint.PointId;
                            m_instruction.InstructionID = m_instruction.InstructionID;

                            Session[wizard.C_INSTRUCTION] = m_instruction;
                        }
                        else
                        {
                            if (pointId != m_instruction.PointID)
                            {
                                Facade.IPoint facPoint = new Facade.Point();
                                Entities.Point selectedPoint = facPoint.GetPointForPointId(pointId);

                                m_instruction.Point = selectedPoint;
                                m_instruction.PointID = selectedPoint.PointId;
                            }

                            // Altering an existing instruction
                            // Cause the first collectdrop to be displayed.
                            if (m_instruction.CollectDrops.Count > 0)
                            {
                                Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[0];
                                Session[wizard.C_COLLECT_DROP_INDEX] = 0;
                            }
                        }
                    }
                    else
                    {
                        if (m_isAmendment)
                        {
                            if (pointId != m_instruction.PointID)
                            {
                                Facade.IPoint facPoint = new Facade.Point();
                                Entities.Point selectedPoint = facPoint.GetPointForPointId(pointId);

                                m_instruction.Point = selectedPoint;
                                m_instruction.PointID = selectedPoint.PointId;
                            }

                            // Cause the first point to be displayed.
                            if (m_instruction.CollectDrops.Count > 0)
                            {
                                Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[0];
                                Session[wizard.C_COLLECT_DROP_INDEX] = 0;
                            }

                            Session[wizard.C_INSTRUCTION] = m_instruction;
                        }
                        else
                        {
                            Facade.IPoint facPoint = new Facade.Point();
                            Entities.Point selectedPoint = facPoint.GetPointForPointId(pointId);

                            // Add the collect drop point information
                            m_instruction.Point = selectedPoint;
                            m_instruction.PointID = selectedPoint.PointId;
                            m_instruction.InstructionID = m_instruction.InstructionID;

                            Session[wizard.C_INSTRUCTION] = m_instruction;
                        }
                    }

                    GoToStep("PD");
                }
            }
        }


        #endregion

        #region Web Form Designer generated code


        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //			this.Init += new EventHandler(Page_Init);
        }

        #endregion
    }
}
