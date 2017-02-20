using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Orchestrator.WebUI.Services
{

    [ServiceContract(Namespace = "Orchestrator.WebUI.Services")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Allocation
    {

        [OperationContract]
        public string GetConsortiumMemberToAllocate(int? orderGroupID, int? orderID, int customerIdentityID, int collectionPointID, int deliveryPointID, DateTime collectionDateTime, DateTime deliveryDateTime)
        {
            Facade.IOrder facOrder = new Facade.Order();
            var consortiumMember = facOrder.GetConsortiumMemberToAllocate(orderGroupID, orderID, customerIdentityID, collectionPointID, deliveryPointID, collectionDateTime, deliveryDateTime);
            return consortiumMember == null ? null : string.Format("{0},{1}", consortiumMember.IdentityId, consortiumMember.OrganisationName);
        }

        /// <summary>
        /// Subcontract all allocated orders for the specified client
        /// </summary>
        /// <returns>A string containing the number of orders that have been subcontracted followed by a comma and then a list of business rule violations (if any)</returns>
        [OperationContract]
        public string SubcontractAllocatedOrders(int customerIdentityID, string userID)
        {
            var exceptionHandler = new Func<int, IEnumerable<int>, Exception, bool>((jobID, orderIDs, ex) =>
            {
                WcfServiceError.LogException(new Exception(
                    string.Format(
                        "Subcontracting allocated orders failed for order(s) {0} on run {1}.",
                        string.Join(", ", orderIDs.Select(o => o.ToString()).ToArray()),
                        jobID),
                    ex));

                return false; // Return false so the exception will be rethrown and therefore reported back to the user as well as logged
            });

            IEnumerable<string> businessRuleInfringements = null;

            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
            int orderCount = facJobSubContractor.SubcontractAllAllocatedOrders(
                customerIdentityID,
                exceptionHandler,
                out businessRuleInfringements,
                userID);

            try
            {
                var retVal = new
                {
                    OrderCount = orderCount,
                    BusinessRuleInfringements = string.Join(Environment.NewLine, businessRuleInfringements.ToArray())
                };

                return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(retVal);
            }
            catch (Exception ex)
            {
                WcfServiceError.LogException(ex);
                throw;
            }
        }

    }

}
