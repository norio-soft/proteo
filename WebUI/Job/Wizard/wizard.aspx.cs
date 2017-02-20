using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Job.Wizard
{
	/// <summary>
	/// Provides a wizard based approach to adding and updating job information.
	/// </summary>
	public partial class wizard : Orchestrator.Base.BasePage
	{
		#region Constants

		#region Session Variable Names

		// Job Entity
		public const string C_JOB = "C_JOB";

		// Point Type
        public const string C_POINT_TYPE = "C_POINT_TYPE";

		// Instruction
        public const string C_INSTRUCTION = "C_INSTRUCTION";
        public const string C_INSTRUCTION_INDEX = "C_INSTRUCTION_INDEX";

		// Current Collect Drop
        public const string C_COLLECT_DROP = "C_COLLECT_DROP";
        public const string C_COLLECT_DROP_INDEX = "C_COLLECT_DROP_INDEX";

		// Client Name (when creating a new client / client's customer)
        public const string C_CLIENT_NAME = "C_CLIENT_NAME";
		// JR Customer Identity Id (when creating a new client's customer)
        public const string C_CUSTOMER_OF = "C_CUSTOMER_OF";
		
		// Client Identity Id (when creating a new point)
        public const string C_POINT_FOR = "C_POINT_CLIENT_ID";
		// Town Id (when creating a new point)
        public const string C_TOWN_ID = "C_TOWN_ID";
		// Town Point Name (when creating a new point)
        public const string C_POINT_NAME = "C_POINT_NAME";

		// Jump to Details Section
        public const string C_JUMP_TO_DETAILS = "C_JUMP_TO_DETAILS";

        // Collection and Delivery Point Section (for adding Rates)
        public const string C_COLLECTION_POINT = "C_COLLECTION_POINT";
        public const string C_COLLECTION_POINT_ID = "C_COLLECTION_POINT_ID";
        public const string C_DELIVERY_POINT = "C_DELIVERY_POINT";
        public const string C_DELIVERY_POINT_ID = "C_DELIVERY_POINT_ID";

        // Groupage Section
        public const string C_ADDED_ORDERS = "C_ADDED_ORDERS";
        public const string C_REMOVED_ORDERS = "C_REMOVED_ORDERS";

		#endregion

        public const string C_CONFIRM_MESSAGE = @"javascript:return(confirm('If you close this window, all your changes will be lost.  Click OK to accept this or click Cancel, and proceed to the end of the wizard to save your changes.'))";

		#endregion

		#region Form Elements


		#endregion

		#region Page Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			int jobId = 0;
			string step = "SC";
		
			if (Request.QueryString["jobId"] != null)
				jobId = Int32.Parse(Request.QueryString["JobId"]);

			if (Request.QueryString["isStock"] != null)
				bool.Parse(Request.QueryString["isStock"]);

			if (jobId > 0)
				step = "JD";

			if (Request.QueryString["step"] != null)
				step = Request.QueryString["step"];
			else if (!IsPostBack)
			{
				// Clear the session variables
				Session[wizard.C_CLIENT_NAME] = null;
				Session[wizard.C_COLLECT_DROP] = null;
				Session[wizard.C_COLLECT_DROP_INDEX] = null;
				Session[wizard.C_CONFIRM_MESSAGE] = null;
				Session[wizard.C_CUSTOMER_OF] = null;
				Session[wizard.C_INSTRUCTION] = null;
				Session[wizard.C_INSTRUCTION_INDEX] = null;
				Session[wizard.C_JOB] = null;
				Session[wizard.C_JUMP_TO_DETAILS] = null;
				Session[wizard.C_POINT_FOR] = null;
				Session[wizard.C_POINT_NAME] = null;
				Session[wizard.C_POINT_TYPE] = null;
				Session[wizard.C_TOWN_ID] = null;
			}

			LoadPanel(step);
		}

		#endregion

		#region Event Handlers & Methods
		/// <summary>
		/// Loads the appropriate user control into the panel for the current step.
		/// </summary>
		/// <param name="step">The step as is currently specified via the QueryString.</param>
		private void LoadPanel(string step)
		{
			Control userControl = null;

			switch (step.ToUpper())
			{
				case "SC":	// Select Client
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/Client.ascx");
					break;
				case "JT":	// Specify the Job Type
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/JobType.ascx");
					break;
				case "JR":	// Enter Job References
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/JobReferences.ascx");
					break;
				case "P":	// Specify the Collect/Drop Point
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/Point.ascx");
					break;
				case "PD":	// Configure a Docket for the Collect/Drop Point
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/Docket.ascx");
					break;
				case "PC":	// Enter the Collect/Drop Charge information
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/InstructionPricing.ascx");
					break;
				case "CC":	// Specify the Drop Client's Customer
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/ClientsCustomer.ascx");
					break;
				case "JD":	// Display the details of the Job
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.GeneralUsage);
					userControl = LoadControl("UserControls/Details.ascx");
					break;

				// -- Supporting Panels
				case "CNP":	// Create new Point
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
					userControl = LoadControl("UserControls/AddPoint.ascx");
					break;
                case "AR":  // Add a rate
                    Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
                    userControl = LoadControl("UserControls/AddRate.ascx");
                    break;

                // -- Groupage Panels
                case "SGO": // Specify Groupage Order
                    Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
                    userControl = LoadControl("UserControls/InstructionOrders.ascx");
                    break;
                case "SGOH": // Specify Groupage Order Handling
                    Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
                    userControl = LoadControl("UserControls/OrderHandling.ascx");
                    break;

				// -- Special Events
				case "PR":	// The user has attempted to edit a Pallet Return Job
					break;
				case "GR":	// The user has attempted to edit a Goods Return Job
					break;
				case "NP":	// The user has attempted to edit a Non-Planning Job (i.e. Demurrage Collection or Self Bill Remainder Collection)
					break;
				case "CANCEL":	// Cancel
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.GeneralUsage);
					ClearSessionVariables();

					pnlContent.Visible = false;
					pnlCancel.Visible = true;
					break;
			}

            if (userControl != null)
            {
                pnlContent.Controls.Add(userControl);
                if (userControl is IDefaultButton)
                {
                    this.Form.DefaultButton = ((IDefaultButton)userControl).DefaultButton.UniqueID;
                    //Page.RegisterHiddenField("__EVENTTARGET", ((IDefaultButton)userControl).DefaultButton.UniqueID);
                }
                
            }
		}

        private void ClearSessionVariables()
        {
            Session[C_JOB] = null;
            Session[C_POINT_TYPE] = null;
            Session[C_INSTRUCTION] = null;
            Session[C_CLIENT_NAME] = null;
            Session[C_CUSTOMER_OF] = null;
            Session[C_POINT_FOR] = null;
            Session[C_TOWN_ID] = null;
            Session[C_POINT_NAME] = null;
            Session[C_INSTRUCTION_INDEX] = null;
            Session[C_COLLECT_DROP] = null;
            Session[C_COLLECT_DROP_INDEX] = null;
            Session[C_JUMP_TO_DETAILS] = null;
            Session[C_COLLECTION_POINT] = null;
            Session[C_COLLECTION_POINT_ID] = null;
            Session[C_DELIVERY_POINT] = null;
            Session[C_DELIVERY_POINT_ID] = null;
            Session[C_ADDED_ORDERS] = null;
            Session[C_REMOVED_ORDERS] = null;
        }

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this.Page);
            }
        }
	}
}
