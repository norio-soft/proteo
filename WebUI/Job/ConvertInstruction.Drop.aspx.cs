using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Job
{
    public partial class ConvertInstruction
    {
        string errorMessage;
        private void BindDropInstruction(Entities.Instruction instruction)
        {
            // The instruction to convert is currently a drop instruction so will be
            // converted to a trunk instruction (most likely for cross-docking).
            // Performing this action is only permissable if the new movement represented
            // by each order's involvement on the run can sensibly be attached to end of the
            // collection run for the order.  This is so that the chain for each order is maintained.

            mvConvertInstruction.SetActiveView(vwConvertDrop);

            var facJob = new Facade.Job();
            var facOrganisation = new Facade.Organisation();

            var job = facJob.GetJob(instruction.JobId, true, true);

            grdOrdersOnDrop.DataSource = from o in instruction.CollectDrops.Cast<Entities.CollectDrop>().Select(cd => cd.Order)
                                         let ci = (from i in job.Instructions where i.InstructionTypeId == (int)eInstructionType.Load && i.CollectDrops.Cast<Entities.CollectDrop>().FirstOrDefault(cd => cd.OrderID == o.OrderID) != null select i).Single()
                                         let client = facOrganisation.GetForIdentityId(o.CustomerIdentityID)
                                         select new ConvertInstructionOrderData()
                                         {
                                             OrderID = o.OrderID,
                                             CustomerOrganisationName = client.OrganisationName,
                                             BusinessTypeDescription = GetBusinessTypeDescription(o.BusinessTypeID),
                                             OrderServiceLevel = o.OrderServiceLevel,
                                             CustomerOrderNumber = o.CustomerOrderNumber,
                                             DeliveryOrderNumber = o.DeliveryOrderNumber,
                                             Message = GetDropConversionMessage(ci, instruction, o),
                                             CollectionPointID = ci.PointID,
                                             CollectionPointDescription = ci.Point.Description,
                                             CollectionDateTime = ci.BookedDateTime,
                                             CollectionIsAnyTime = ci.IsAnyTime,
                                             DeliveryPointID = instruction.PointID,
                                             DeliveryPointDescription = instruction.Point.Description,
                                             DeliveryDateTime = instruction.BookedDateTime,
                                             DeliveryIsAnyTime = instruction.IsAnyTime,
                                             DeliveringResource = GetAssignedResource(job, instruction, o),
                                             LCID = o.LCID,
                                             ForeignRate = o.ForeignRate
                                         };
            
            grdOrdersOnDrop.DataBind();
        }

        private bool CanBeRemovedFromDrop(Entities.Instruction dropInstruction, Entities.Order order)
        {
            string message = String.Empty;
            return CanBeRemovedFromDrop(dropInstruction, order, out message);
        }

        private bool CanBeRemovedFromDrop(Entities.Instruction dropInstruction, Entities.Order order, out string message)
        {
            var messageFragments = new List<string>();
            

            // This order can only be removed from this delivery and placed on a trunk if:
            //  * This would not result in the breaking of the chain - to test this the order's collection run delivery point must match the delivery run collection point.
            //  * No refusals have been logged against this instruction - it is possible to remove the call-in but keep the refusal.
            
            if (order.CollectionRunDeliveryPointID != order.DeliveryRunCollectionPointID)
            {
                messageFragments.Add("The Order's Collection Point does not match the Collection Point on the Run.");
                
            }
                        
            var facGoodsRefusal = new Facade.GoodsRefusal();
            var dsGoodsRefusal = facGoodsRefusal.GetRefusalsForInstructionIdAndOrderId(dropInstruction.InstructionID, order.OrderID);
            if (dsGoodsRefusal.Tables[0].Rows.Count > 0)
            {
                messageFragments.Add("Refusals Logged");
            }

            if (messageFragments.Count > 0)
            {
                message = String.Format("This Order cannot be Converted - {0}", String.Join(", ", messageFragments.ToArray()));
                grdOrdersOnDrop.ItemStyle.BackColor = System.Drawing.Color.FromName("FireBrick");
                grdOrdersOnDrop.ItemStyle.ForeColor = System.Drawing.Color.White;
            }
            else
                message = String.Empty;
            errorMessage = message;
            return messageFragments.Count == 0;
        }

        private string GetDropConversionMessage(Entities.Instruction loadInstruction, Entities.Instruction dropInstruction, Entities.Order order)
        {
            string message = String.Empty;
            CanBeRemovedFromDrop(dropInstruction, order, out message);
            
            return message;
            
        }

        private void grdOrdersOnDrop_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item)
            {
                var dataItem = (ConvertInstructionOrderData)((GridDataItem)e.Item).DataItem;

                // Display the rate
                var lblRate = (ITextControl)e.Item.FindControl("lblRate");
                CultureInfo culture = new CultureInfo(dataItem.LCID);
                lblRate.Text = dataItem.ForeignRate.ToString("C", culture);
            }
        }

        private void cfvPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ucPoint.PointID > 0;
        }

        //private void MessageBox(string msg)
        //{
        //    Label lbl = new Label();
        //    lbl.Text = "<script language='javascript'>" + Environment.NewLine + "window.alert('" + msg + "')</script>";
        //    Page.Controls.Add(lbl);
        //}

        private void btnConfirmDropConversion_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var facPoint = new Facade.Point();

                // Get the point to trunk to.
                var trunkPoint = facPoint.GetPointForPointId(ucPoint.PointID); ;

                // Configure the instructions.
                var dropInstruction = GetInstruction();
                Entities.Instruction trunkInstruction = new Entities.Instruction(eInstructionType.Trunk, dropInstruction.JobId, String.Empty);
                trunkInstruction.BookedDateTime = dropInstruction.BookedDateTime;
                trunkInstruction.IsAnyTime = dropInstruction.IsAnyTime;
                trunkInstruction.PointID = trunkPoint.PointId;
                trunkInstruction.Point = trunkPoint;
                trunkInstruction.CollectDrops = new Entities.CollectDropCollection();
                trunkInstruction.ClientsCustomerIdentityID = trunkPoint.IdentityId;

                // Process the collect drops.
                for (int collectDropIndex = 0; collectDropIndex < dropInstruction.CollectDrops.Count; collectDropIndex++)
                {
                    var cd = dropInstruction.CollectDrops[collectDropIndex];

                    // If the order can be moved from the drop to the trunk do so.
                    if (CanBeRemovedFromDrop(dropInstruction, cd.Order))
                    {
                        cd.CollectDropId = 0;
                        cd.InstructionID = trunkInstruction.InstructionID;
                        cd.OrderAction = eOrderAction.Cross_Dock;
                        trunkInstruction.CollectDrops.Add(cd);
                        dropInstruction.CollectDrops.RemoveAt(collectDropIndex);
                        collectDropIndex--;
                    }
                }

                if (trunkInstruction.CollectDrops.Count > 0)
                {
                    // Alter the job.
                    Facade.IJob facJob = new Facade.Job();
                    var job = facJob.GetJob(dropInstruction.JobId, true, true);
                    var instructions = new List<Entities.Instruction>() { dropInstruction, trunkInstruction };
                    Entities.CustomPrincipal user = Page.User as Entities.CustomPrincipal;
                    var result = facJob.AmendInstructions(job, instructions, eLegTimeAlterationMode.Minimal, user.UserName);

                    if (result.Success)
                    {
                        // Close this window and refresh the parent window.
                        this.ReturnValue = "refresh";
                        this.Close();
                    }
                }
                else
                {
                    // No action to take - close this window.
                    if (String.IsNullOrEmpty(errorMessage))
                    {
                        this.ReturnValue = "refresh";
                        this.Close();
                    }
                    else
                    {
                        //System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Please see the Message in the " +
                        //"Table for more information", "Error",
                        //    System.Windows.Forms.MessageBoxButtons.OK);
                        ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Please see the message in the Table for more information');", true);


                    }
                }
            }
        }
    }
}