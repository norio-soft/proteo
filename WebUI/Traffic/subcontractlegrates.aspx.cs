using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Globalization;

using Telerik.Web.UI;

namespace Orchestrator.WebUI
{
    public partial class Traffic_subcontractlegrates : Orchestrator.Base.BasePage
    {
        #region Private Fields

        private int m_jobID = -1;
        private Entities.JobSubContractor _wholeJobSubcontractor = null;
        private Entities.Job _job = null;
        private CultureInfo _culture = null;
        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["JID"]))
            {
                if (int.TryParse(Request.QueryString["JID"], out m_jobID))
                {
                    bool subbedOutWholeJob = false;
                    _job = GetJobEntityFromCache();

                    foreach (Entities.JobSubContractor jsc in _job.SubContractors)
                    {
                        if (jsc.SubContractWholeJob)
                        {
                            _wholeJobSubcontractor = jsc;
                            _culture = new CultureInfo(jsc.LCID);
                            subbedOutWholeJob = true;
                            break;
                        }
                    }

                    if (!IsPostBack)
                    {
                        if (!subbedOutWholeJob)
                        {
                            // Only show the legs and orders grid if not subbed out whole job
                            repLegSubbyRates.Visible = true;

                            Facade.IJob facJob = new Facade.Job();
                            DataTable dt = facJob.GetTrafficSheetForJobId(m_jobID).Tables[1];
                            repLegSubbyRates.DataSource = dt;
                            repLegSubbyRates.DataBind();
                        }
                        else
                        {
                            // ...otherwise, show the whole job rate controls.
                            fldWholeJobSubcontractInformation.Visible = true;
                            repLegSubbyRates.Visible = false;

                            txtWholeJobSubcontractRate.Attributes.Add("jobSubContractID", _wholeJobSubcontractor.JobSubContractID.ToString());
                            txtWholeJobSubcontractRate.Attributes.Add("LCID", _wholeJobSubcontractor.LCID.ToString());

                            txtWholeJobSubcontractRate.Culture = new CultureInfo(_wholeJobSubcontractor.LCID);
                            txtWholeJobSubcontractRate.Text = _wholeJobSubcontractor.ForeignRate.ToString();
                            chkIsAttended.Checked = _wholeJobSubcontractor.IsAttended;
                            txtReference.Text = _wholeJobSubcontractor.Reference;

                            if (_wholeJobSubcontractor.InvoiceID > 0)
                            {
                                // don't allow the user to update information as the invoice has already been generated
                                txtWholeJobSubcontractRate.Enabled = false;
                                chkIsAttended.Enabled = false;
                                txtReference.Enabled = false;
                                btnUpdateRates.Enabled = false;
                            }
                        }
                    }
                }
            }

            this.Master.WizardTitle = "Update Sub-Contract Rates";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            repLegSubbyRates.ItemDataBound += new RepeaterItemEventHandler(repLegSubbyRates_ItemDataBound);

            this.btnUpdateRates.Click += new EventHandler(btnUpdateRates_Click);
        }

        #endregion

        #region Event Handlers

        #region Repeater Events

        void repLegSubbyRates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;
                RadNumericTextBox rntSubContractRate = e.Item.FindControl("rntSubContractRate") as RadNumericTextBox;
                HtmlTableRow rowSubbedOrders = e.Item.FindControl("rowSubbedOrders") as HtmlTableRow;
                Repeater repOrderSubbyRates = e.Item.FindControl("repOrderSubbyRates") as Repeater;
                TextBox txtSubcontractReference = e.Item.FindControl("txtSubcontractReference") as TextBox;
                CheckBox chkIsAttended = e.Item.FindControl("chkIsAttended") as CheckBox;

                if (drv["JobSubContractID"] == DBNull.Value)
                {
                    rntSubContractRate.Visible = false;
                    chkIsAttended.Visible = false;
                    txtSubcontractReference.Visible = false;
                    rowSubbedOrders.Visible = true;
                    Facade.IOrder facOrder = new Facade.Order();
                    repOrderSubbyRates.DataSource = facOrder.GetSubbedOrdersForInstructionID((int)drv["InstructionId"]);
                    repOrderSubbyRates.DataBind();
                }
                else
                {
                    rntSubContractRate.Visible = true;
                    txtSubcontractReference.Visible = true;
                    chkIsAttended.Visible = true;
                    rowSubbedOrders.Visible = false;
                    repOrderSubbyRates.Visible = false;

                    rntSubContractRate.Text = ((decimal)drv["SubbyForeignRateForLeg"]).ToString();

                    int jobSubContractID = (int)drv["JobSubContractID"];
                    Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

                    if (jobSubContractID > 0)
                    {
                        txtSubcontractReference.Text = drv["Reference"].ToString();
                        chkIsAttended.Checked = bool.Parse(drv["IsAttended"].ToString());
                    }

                    // If the rate isn't editabel then the Reference and IsAttended flag isn't editable either.
                    bool rateEditable = facJobSubContractor.IsRateEditable(jobSubContractID);
                    rntSubContractRate.Enabled = rateEditable;
                    txtReference.Enabled = rateEditable;
                    chkIsAttended.Enabled = rateEditable;

                    Entities.JobSubContractor jobSubContract = facJobSubContractor.GetSubContractorForJobSubContractId(jobSubContractID);
                    rntSubContractRate.Culture = new CultureInfo(jobSubContract.LCID);
                }
            }
        }

        protected void repOrderSubbyRates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;
                Repeater repReferences = e.Item.FindControl("repReferences") as Repeater;
                Repeater repOrderSubbyRates = sender as Repeater;
                DataTable referenceData = (repOrderSubbyRates.DataSource as DataSet).Tables[1];
                RadNumericTextBox rntSubContractRate = e.Item.FindControl("rntSubContractRate") as RadNumericTextBox;
                TextBox txtSubcontractReference = e.Item.FindControl("txtSubcontractReference") as TextBox;
                CheckBox chkIsAttended = e.Item.FindControl("chkIsAttended") as CheckBox;

                if (drv["SubbyForeignRate"] != DBNull.Value)
                {
                    rntSubContractRate.Text = ((decimal)drv["SubbyForeignRate"]).ToString();

                    bool canBeChanged = (bool)drv["CanBeChanged"];
                    rntSubContractRate.Enabled = canBeChanged;
                    txtSubcontractReference.Enabled = canBeChanged;
                    chkIsAttended.Enabled = canBeChanged;
                }
                else
                    rntSubContractRate.Text = (new decimal(0)).ToString();

                int orderID = int.Parse(drv["OrderID"].ToString());
                DataRow[] references = referenceData.Select("OrderID = " + orderID.ToString());
                List<DataRow> listReferences = new List<DataRow>(references);

                // Add the customer order number and delivery order number!
                DataRow deliveryOrderNumber = referenceData.NewRow();
                deliveryOrderNumber["OrderID"] = orderID;
                deliveryOrderNumber["Description"] = "Delivery Order Number";
                deliveryOrderNumber["Reference"] = drv["DeliveryOrderNumber"].ToString();
                listReferences.Insert(0, deliveryOrderNumber);
                DataRow customerOrderNumber = referenceData.NewRow();
                customerOrderNumber["OrderID"] = orderID;
                customerOrderNumber["Description"] = "Customer Order Number";
                customerOrderNumber["Reference"] = drv["CustomerOrderNumber"].ToString();
                listReferences.Insert(0, customerOrderNumber);

                int jobSubContractID = (int)drv["JobSubContractID"];
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                Entities.JobSubContractor jobSubContract = facJobSubContractor.GetSubContractorForJobSubContractId(jobSubContractID);
                rntSubContractRate.Culture = new CultureInfo(jobSubContract.LCID);

                if (jobSubContractID > 0)
                {
                    txtSubcontractReference.Text = drv["Reference"].ToString();
                    chkIsAttended.Checked = bool.Parse(drv["IsAttended"].ToString());
                }

                repReferences.DataSource = listReferences;
                repReferences.DataBind();
            }
        }

        protected void repOrderSubbyRates_PreRender(object sender, EventArgs e)
        {
            Repeater repOrderSubbyRates = sender as Repeater;

            if (repOrderSubbyRates.Items.Count == 0)
            {
                repOrderSubbyRates.Visible = false;
                //repOrderSubbyRates.Parent.Parent.Visible = false;
            }
        }

        #endregion

        #region Button Events

        void btnUpdateRates_Click(object sender, EventArgs e)
        {
            if (_wholeJobSubcontractor == null)
            {
                // the JS will not allow non currency figures.
                Facade.IJobSubContractor facSubby = new Facade.Job();
                Entities.Job job = GetJobEntityFromCache();

                int jobSubContractID = 0;
                decimal rate = 0M;
                string userID = this.User.Identity.Name;
                RadNumericTextBox txtRate = null;
                TextBox txtSubcontractReference = null;
                CheckBox chkIsAttended = null;
                HiddenField hidJobSubContractID = null;

                foreach (RepeaterItem legItem in repLegSubbyRates.Items)
                {
                    if (legItem.ItemType == ListItemType.Item || legItem.ItemType == ListItemType.AlternatingItem)
                    {
                        txtRate = legItem.FindControl("rntSubContractRate") as RadNumericTextBox;
                        hidJobSubContractID = legItem.FindControl("hidJobSubContractID") as HiddenField;
                        txtSubcontractReference = legItem.FindControl("txtSubcontractReference") as TextBox;
                        chkIsAttended = legItem.FindControl("chkIsAttended") as CheckBox;

                        if (!string.IsNullOrEmpty(hidJobSubContractID.Value))
                        {
                            jobSubContractID = int.Parse(hidJobSubContractID.Value);
                            if (decimal.TryParse(txtRate.Text, System.Globalization.NumberStyles.Currency, System.Threading.Thread.CurrentThread.CurrentCulture, out rate))
                            {
                                Entities.JobSubContractor jobSubContract = facSubby.GetSubContractorForJobSubContractId(jobSubContractID);
                                jobSubContract.ForeignRate = rate;
                                jobSubContract.IsAttended = chkIsAttended.Checked;
                                jobSubContract.Reference = txtSubcontractReference.Text;
                                facSubby.Update(m_jobID, jobSubContract, _job.LastUpdateDate, userID);
                            }
                        }
                        else
                        {
                            Repeater repOrderSubbyRates = legItem.FindControl("repOrderSubbyRates") as Repeater;
                            if (repOrderSubbyRates.Visible)
                            {
                                foreach (RepeaterItem orderItem in repOrderSubbyRates.Items)
                                {
                                    if (orderItem.ItemType == ListItemType.Item || orderItem.ItemType == ListItemType.AlternatingItem)
                                    {
                                        txtRate = orderItem.FindControl("rntSubContractRate") as RadNumericTextBox;
                                        hidJobSubContractID = orderItem.FindControl("hidJobSubContractID") as HiddenField;
                                        txtSubcontractReference = orderItem.FindControl("txtSubcontractReference") as TextBox;
                                        chkIsAttended = orderItem.FindControl("chkIsAttended") as CheckBox;

                                        if (!string.IsNullOrEmpty(hidJobSubContractID.Value))
                                        {
                                            jobSubContractID = int.Parse(hidJobSubContractID.Value);
                                            if (decimal.TryParse(txtRate.Text, System.Globalization.NumberStyles.Currency, System.Threading.Thread.CurrentThread.CurrentCulture, out rate))
                                            {
                                                Entities.JobSubContractor jobSubContract = facSubby.GetSubContractorForJobSubContractId(jobSubContractID);
                                                jobSubContract.ForeignRate = rate;
                                                jobSubContract.IsAttended = chkIsAttended.Checked;
                                                jobSubContract.Reference = txtSubcontractReference.Text;
                                                facSubby.Update(m_jobID, jobSubContract, _job.LastUpdateDate, userID);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Facade.IJobSubContractor facSub = new Facade.Job();
                job.SubContractors = facSub.GetSubContractorForJobId(_job.JobId);
                AddJobEntityToCache(_job);

            }
            else
            {
                // Save and Hide the Edit Facilities
                decimal rate = 0;

                Facade.IJobSubContractor facSub = new Facade.Job();

                if (Decimal.TryParse(txtWholeJobSubcontractRate.Text, System.Globalization.NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-GB"), out rate))
                {
                    int jobSubContractID = int.Parse(txtWholeJobSubcontractRate.Attributes["jobSubContractID"]);

                    // Set the jobsubcontract information.
                    _wholeJobSubcontractor.ForeignRate = decimal.Parse(txtWholeJobSubcontractRate.Text);
                    _wholeJobSubcontractor.IsAttended = chkIsAttended.Checked;
                    _wholeJobSubcontractor.Reference = txtReference.Text;

                    // Save it.
                    facSub.Update(_wholeJobSubcontractor.JobID, _wholeJobSubcontractor, _job.LastUpdateDate, User.Identity.Name);

                    // Update the cached job.
                    _job.SubContractors = facSub.GetSubContractorForJobId(_job.JobId);
                    AddJobEntityToCache(_job);
                }
            }

            // Refresh the job details page.
            lblInjectScript.Text = "<script>RefreshParentPage();</script>";
        }

        private Entities.Job GetJobEntityFromCache()
        {
            Entities.Job job = (Entities.Job)Cache.Get("JobEntityForJobId" + m_jobID);

            if (job == null)
            {
                Facade.IJob facJob = new Facade.Job();
                Facade.IPCV facPCV = new Facade.PCV();
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

                job = facJob.GetJob(m_jobID, true, true);
                job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobID);
                job.Extras = facJob.GetExtras(m_jobID, true);
                job.PCVs = facPCV.GetForJobId(m_jobID);
                job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobID);
                job.SubContractors = facJobSubContractor.GetSubContractorForJobId(m_jobID);

                AddJobEntityToCache(job);
            }

            return job;
        }

        private void AddJobEntityToCache(Entities.Job job)
        {
            if (Cache.Get("JobEntityForJobId" + m_jobID) != null)
            {
                Cache.Remove("JobEntityForJobId" + m_jobID);
            }

            Cache.Add("JobEntityForJobId" + m_jobID.ToString(),
                        job,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(5),
                        CacheItemPriority.Normal,
                        null);
        }

        #endregion

        #endregion
    }
}