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
        private void BindTrunkInstruction(Entities.Instruction instruction)
        {
            // The instruction to convert is currently a trunk instruction so will
            // either be converted to a drop instruction or the point will be changed
            // so that the orders involved will be taken to a different location.
            // In the case when instruction will be converted to a drop instruction,
            // this is only possible for the orders that have not already been planned
            // for delivery.
            // In the case of changing the trunk location, this can only be done for the
            // end of the collection run, otherwise the order's collection chain will be broken.

            mvConvertInstruction.SetActiveView(vwConvertTrunk);

            var facJob = new Facade.Job();
            var facOrganisation = new Facade.Organisation();

            var job = facJob.GetJob(instruction.JobId, true, true);

            grdOrdersOnTrunk.DataSource = from o in instruction.CollectDrops.Cast<Entities.CollectDrop>().Select(cd => cd.Order)
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
                                              Message = GetTrunkConversionMessage(ci, instruction, o),
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

        private bool CanBeRemovedFromTrunk(Entities.Instruction trunkInstruction, Entities.Order order)
        {
            string message = String.Empty;
            return CanBeRemovedFromTrunk(trunkInstruction, order, out message);
        }

        private bool CanBeRemovedFromTrunk(Entities.Instruction trunkInstruction, Entities.Order order, out string message)
        {
            var messageFragments = new List<string>();

            // This order can only be removed from this trunk and placed on a delivery if:
            //  * The order hasn't already been planned for delivery on another job.

            if (order.PlannedForDelivery)
            {
                messageFragments.Add("Already planned for delivery");
            }

            if (messageFragments.Count > 0)
                message = String.Format("Excluded - {0}", String.Join(", ", messageFragments.ToArray()));
            else
                message = String.Empty;

            return messageFragments.Count == 0;
        }

        private string GetTrunkConversionMessage(Entities.Instruction loadInstruction, Entities.Instruction trunkInstruction, Entities.Order order)
        {
            string message = String.Empty;
            CanBeRemovedFromTrunk(trunkInstruction, order, out message);
            return message;
        }

        private void grdOrdersOnTrunk_ItemDataBound(object sender, GridItemEventArgs e)
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

        void btnConfirmTrunkConversion_Click(object sender, EventArgs e)
        {
            var dropInstructions = new Dictionary<int, Entities.Instruction>();
            var facPoint = new Facade.Point();

            // Configure the instructions.
            var trunkInstruction = GetInstruction();

            // Process the collect drops.
            for (int collectDropIndex = 0; collectDropIndex < trunkInstruction.CollectDrops.Count; collectDropIndex++)
            {
                var cd = trunkInstruction.CollectDrops[collectDropIndex];

                // If the order can be moved from the drop to the trunk do so.
                if (CanBeRemovedFromTrunk(trunkInstruction, cd.Order))
                {
                    Entities.Instruction dropInstruction;
                    if (!dropInstructions.TryGetValue(cd.Order.DeliveryPointID, out dropInstruction))
                    {
                        var deliveryPoint = facPoint.GetPointForPointId(cd.Order.DeliveryPointID);
                        dropInstruction = new Entities.Instruction(eInstructionType.Drop, trunkInstruction.JobId, String.Empty);
                        dropInstruction.BookedDateTime = trunkInstruction.BookedDateTime;
                        dropInstruction.IsAnyTime = trunkInstruction.IsAnyTime;
                        dropInstruction.PointID = deliveryPoint.PointId;
                        dropInstruction.Point = deliveryPoint;
                        dropInstruction.CollectDrops = new Entities.CollectDropCollection();
                        dropInstruction.ClientsCustomerIdentityID = deliveryPoint.IdentityId;
                        dropInstructions.Add(cd.Order.DeliveryPointID, dropInstruction);
                    }

                    cd.CollectDropId = 0;
                    cd.InstructionID = dropInstruction.InstructionID;
                    cd.OrderAction = eOrderAction.Cross_Dock;
                    dropInstruction.CollectDrops.Add(cd);
                    trunkInstruction.CollectDrops.RemoveAt(collectDropIndex);
                    collectDropIndex--;
                }
            }

            if (dropInstructions.Count > 0)
            {
                // Alter the job.
                Facade.IJob facJob = new Facade.Job();
                var job = facJob.GetJob(trunkInstruction.JobId, true, true);
                var instructions = new List<Entities.Instruction>() { trunkInstruction };
                instructions.AddRange(dropInstructions.Values);
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
                this.ReturnValue = "refresh";
                this.Close();
            }
        }
    }
}