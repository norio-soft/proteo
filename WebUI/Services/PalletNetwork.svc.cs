using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.ComponentModel;
using Orchestrator.EF;

namespace Orchestrator.WebUI.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PalletNetwork
    {
        [OperationContract]
        public List<PalletForceOrder> GetOrdersForPalletNetworkExport(DateTime trunkDate)
        {
            // Search for orders based on Date Range Status and text
            // Determine the parameters
            List<int> orderStatusIDs = new List<int>();
            orderStatusIDs.Add((int)eOrderStatus.Approved);
            orderStatusIDs.Add((int)eOrderStatus.Delivered);
            orderStatusIDs.Add((int)eOrderStatus.Invoiced);

            // Set the Business Types
            List<int> BusinessTypes = new List<int>();
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessTypes = facBusinessType.GetAll();

            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                BusinessTypes.Add(int.Parse(row["BusinessTypeID"].ToString()));
            // Retrieve the client id, resource id, and sub-contractor identity id.
            int clientID = 0;
            int resourceID = 0;
            int subContractorIdentityID = Globals.Configuration.PalletNetworkID;

            int collectionPointId = 0;
            int deliveryPointId = 0;

            int goodsType = 0;


            Facade.IOrder facOrder = new Facade.Order();
            DataSet orderData = null;
            orderData = facOrder.Search(orderStatusIDs, trunkDate, trunkDate, String.Empty, false, false, true, false, clientID, resourceID, subContractorIdentityID, BusinessTypes, collectionPointId, deliveryPointId, goodsType);
            List<PalletForceOrder> orders = new List<PalletForceOrder>();

            orders = (from row in orderData.Tables[0].AsEnumerable()
                      select new PalletForceOrder()
                      {
                          OrderID = row.Field<int>("OrderID"),
                          JobID = row.Field<int>("JobID"),
                          CustomerOrganisationName = row.Field<string>("CustomerOrganisationName"),
                          DeliveryPointDescription = row.Field<string>("DeliveryPointDescription"),
                          DeliveryOrderNumber = row.Field<string>("DeliveryOrderNumber"),
                          CustomerOrderNumber = row.Field<string>("CustomerOrderNumber"),
                          NoPallets = row.Field<int>("NoPallets"),
                          PalletSpaces = row.Field<decimal>("PalletSpaces"),
                          Weight = row.Field<decimal>("Weight"),
                          HalfPallets = row.Field<int>("HalfPallets"),
                          QtrPallets = row.Field<int>("QtrPallets"),
                          FullPallets = row.Field<int>("FullPallets"),
                          OverPallets = row.Field<int>("OverPallets"),
                          MessageStateID = row.IsNull("MessageStateID") ? (int?)null : row.Field<int>("MessageStateID"),
                          DeliveryDateTimeLabel = GetTime(row.Field<DateTime>("DeliveryFromDateTime"), row.Field<DateTime>("DeliveryDateTime")),
                          Surcharges = GetSurcharges(row.Field<int>("OrderID"))
                      }).ToList();

            return orders;

        }

        [OperationContract]
        public List<Surcharge> GetSurcharges()
        {
            Orchestrator.Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            List<Entities.ExtraType> extraTypes = facExtraType.GetForIsDisplayedOnAddUpdateOrder();

            List<Surcharge> retVal = new List<Surcharge>();

            foreach (var extraType in extraTypes)
            {
                var currentSurCharge = new Surcharge() { ExtraTypeID = extraType.ExtraTypeId, Description = extraType.ShortDescription };
                retVal.Add(currentSurCharge);
            }
            return retVal;
        }

        [OperationContract]
        public bool UpdatePalletForceOrder(PalletForceOrder inputOrder, string userName)
        {
            foreach(Surcharge surcharge in inputOrder.Surcharges)
            {
                if(surcharge.IsSelected)
                    AddSurcharge(inputOrder, surcharge.ExtraTypeID, userName);
                else
                    RemoveSurcharge(inputOrder, surcharge.ExtraTypeID, userName);
            }

            Orchestrator.EF.DataContext DB = Orchestrator.EF.DataContext.Current;
            var order = DB.OrderSet.Include("JobSubContract").FirstOrDefault(o => o.OrderId == inputOrder.OrderID);
            var vigoOrder = DB.VigoOrderSet.FirstOrDefault(o => o.OrderId == inputOrder.OrderID);
            List<Orchestrator.EF.VigoOrderExtra> surcharges = DB.VigoOrderExtraSet.Where(o => o.VigoOrder.OrderId == inputOrder.OrderID).ToList();

            // Update the Order
            order.NoPallets = (int)inputOrder.NoPallets;
            order.PalletSpaces = inputOrder.PalletSpaces;
            order.Weight = inputOrder.Weight;
            order.LastUpdateUserID = userName;



            // Update the Vigo Order
            vigoOrder.FullPallets = inputOrder.FullPallets;
            vigoOrder.QtrPallets = inputOrder.QtrPallets;
            vigoOrder.HalfPallets = inputOrder.HalfPallets;
            vigoOrder.OverPallets = inputOrder.OverPallets;


            // Update the Extras


            DB.SaveChanges();

            Facade.IJobSubContractor jobSubContractor = new Facade.Job();
            if (order != null && order.JobSubContract.JobSubContractID > 0)
            {
                Entities.JobSubContractor js = jobSubContractor.GetSubContractorForJobSubContractId(order.JobSubContract.JobSubContractID);
                jobSubContractor.UpdateSubContractorCostsForOrders(new List<int>() { order.OrderId }, js, userName);
            }
            return true;
        }

        [OperationContract]
        public bool AddSurcharge(PalletForceOrder order, int extraTypeID, string userName)
        {
            EF.DataContext DB = EF.DataContext.Current;
            VigoOrder vo = DB.VigoOrderSet.Include("VigoOrderExtras").FirstOrDefault(o => o.OrderId == order.OrderID);
            var extraTypes = DB.ExtraTypeSet.ToList();

            if (vo.VigoOrderExtras.FirstOrDefault(e => e.ExtraType.ExtraTypeId == extraTypeID) == null)
            {
                var extraType = extraTypes.FirstOrDefault(et => et.ExtraTypeId == extraTypeID);
                if (extraType != null)
                {
                    VigoOrderExtra extra = new VigoOrderExtra();
                    extra.ExtraType = extraType;
                    vo.VigoOrderExtras.Add(extra);
                }
            }
            DB.SaveChanges();
            return true;
        }

        [OperationContract]
        public bool RemoveSurcharge(PalletForceOrder order, int extraTypeID, string userName)
        {
            try
            {
                EF.DataContext DB = EF.DataContext.Current;
                VigoOrder vo = DB.VigoOrderSet.Include("VigoOrderExtras").Include("VigoOrderExtras.ExtraType").FirstOrDefault(o => o.OrderId == order.OrderID);

                if (vo.VigoOrderExtras.FirstOrDefault(e => e.ExtraType.ExtraTypeId == extraTypeID) != null)
                {
                    VigoOrderExtra extra = vo.VigoOrderExtras.FirstOrDefault(e => e.ExtraType.ExtraTypeId == extraTypeID);
                    vo.VigoOrderExtras.Remove(extra);
                    DB.DeleteObject(extra);
                }
                DB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        [OperationContract]
        public List<int> ExportOrders(List<int> orderIDs, string userName)
        {
            Facade.ExportOrder facExportOrder = new Facade.ExportOrder();
            List<int> retVal = facExportOrder.Create(orderIDs, userName);
            return retVal;
        }

        #region Private Methods
        private string GetTime(DateTime fromDateTime, DateTime toDateTime)
        {

            string retVal = string.Empty;

            if (fromDateTime == toDateTime)
            {
                // Timed booking... only show a single date.
                retVal = GetDate(toDateTime, false);
            }
            else
            {
                // If the times span from mignight to 23:59 on the same day then 
                // it's an 'anytime' window.
                if (fromDateTime.Date == toDateTime.Date && fromDateTime.Hour == 0 && fromDateTime.Minute == 0 && toDateTime.Hour == 23 && toDateTime.Minute == 59)
                {
                    // It's anytime
                    retVal = GetDate(toDateTime, true);
                }
                else
                {
                    // It's a booking window
                    retVal = GetDate(fromDateTime, false) + " to " + GetDate(toDateTime, false);
                }
            }

            return retVal;
        }

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        private List<Surcharge> GetSurcharges(int OrderID)
        {
            EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == OrderID);

            Orchestrator.Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            List<Entities.ExtraType> extraTypes = facExtraType.GetForIsDisplayedOnAddUpdateOrder();

            List<Surcharge> retVal = new List<Surcharge>();

            foreach (var extraType in extraTypes)
            {
                var currentSurCharge = new Surcharge() { ExtraTypeID = extraType.ExtraTypeId, Description = extraType.ShortDescription };
                if (vigoOrder.VigoOrderExtras.FirstOrDefault(e => e.ExtraType.ExtraTypeId == extraType.ExtraTypeId) != null)
                {
                    currentSurCharge.IsSelected = true;

                }
                retVal.Add(currentSurCharge);
            }

            return retVal;
        }
        #endregion
        // Add more operations here and mark them with [OperationContract]
    }
    public class PalletForceOrder : INotifyPropertyChanged
    {


        private bool isSelected;
        /// <summary>
        /// This is used by the UI only please ignore
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
            }
        }




        private int orderID;
        public int OrderID
        {
            get { return orderID; }
            set
            {
                orderID = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("OrderID"));
            }
        }

        private int jobID;
        public int JobID
        {
            get { return jobID; }
            set
            {
                jobID = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("JobID"));
            }
        }

        private string customerOrganisationName;
        public string CustomerOrganisationName
        {
            get { return customerOrganisationName; }
            set
            {
                customerOrganisationName = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("CustomerOrganisationName"));
            }
        }

        private string deliveryPointDescription;
        public string DeliveryPointDescription
        {
            get { return deliveryPointDescription; }
            set
            {
                deliveryPointDescription = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("DeliveryPointDescription"));
            }
        }

        private string deliveryDateTimeLabel;
        public string DeliveryDateTimeLabel
        {
            get { return deliveryDateTimeLabel; }
            set
            {
                deliveryDateTimeLabel = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("DeliveryDateTimeLabel"));
            }
        }

        private string deliveryOrderNumber;
        public string DeliveryOrderNumber
        {
            get { return deliveryOrderNumber; }
            set
            {
                deliveryOrderNumber = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("DeliveryOrderNumber"));
            }
        }

        private string customerOrderNumber;
        public string CustomerOrderNumber
        {
            get { return customerOrderNumber; }
            set
            {
                customerOrderNumber = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("CustomerOrderNumber"));
            }
        }

        private decimal noPallets;
        public decimal NoPallets
        {
            get { return noPallets; }
            set
            {
                noPallets = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("NoPallets"));
            }
        }

        private decimal palletSpaces;
        public decimal PalletSpaces
        {
            get { return palletSpaces; }
            set
            {
                palletSpaces = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("PalletSpaces"));
            }
        }

        private decimal weight;
        public decimal Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("Weight"));
            }
        }

        private int qtrPallets;
        public int QtrPallets
        {
            get { return qtrPallets; }
            set
            {
                qtrPallets = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("QtrPallets"));
            }
        }

        private int halfPallets;
        public int HalfPallets
        {
            get { return halfPallets; }
            set
            {
                halfPallets = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("HalfPallets"));
            }
        }

        private int fullPallets;
        public int FullPallets
        {
            get { return fullPallets; }
            set
            {
                fullPallets = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("FullPallets"));
            }
        }

        private int overPallets;
        public int OverPallets
        {
            get { return overPallets; }
            set
            {
                overPallets = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("OverPallets"));
            }
        }

        private int? messageStateID;
        public int? MessageStateID
        {
            get { return messageStateID; }
            set
            {
                messageStateID = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("MessageStateID"));
            }
        }

        private List<Surcharge> surcharges;
        public List<Surcharge> Surcharges
        {
            get { return surcharges; }
            set
            {
                surcharges = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("Surcharges"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }
        #endregion
    }

    public class Surcharge
    {
        public int ExtraTypeID { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }

}
