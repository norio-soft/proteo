using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Telerik.Web.UI;
using System.Web.Script.Services;

namespace Orchestrator.WebUI.Tariff
{
    [ScriptService]
    public class Tariffs : System.Web.Services.WebService
    {
        public enum eLookupType { ClientOnly, SubContractorOnly, All };

        [WebMethod]
        public RadComboBoxItemData[] GetTariffs(object context)
        {
            try
            {
                var result = new List<RadComboBoxItemData>();

                var lookupType = eLookupType.ClientOnly;

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    IEnumerable<RelaxedKeyValuePair<int, string>> query = null;

                    switch (lookupType)
                    {
                        case eLookupType.ClientOnly:
                            // Only show non-subcontractor tariffs.
                            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                            query = from t in repo.GetAll()
                                    orderby t.Description ascending
                                    select new RelaxedKeyValuePair<int, string>() { Key = t.TariffID, Value = t.Description };
                            break;
                        case eLookupType.SubContractorOnly:
                            break;
                        case eLookupType.All:
                            break;
                    }

                    if (query != null)
                    {
                        result.AddRange(query.Select(tariffData => new RadComboBoxItemData
                        {
                            Value = tariffData.Key.ToString(),
                            Text = tariffData.Value
                        }));
                    }
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public RateWithSurcharges GetRateWithSurcharges(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime)
        {
            try
            {
                return RequestRateWithSurcharges(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, false);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public RateWithSurcharges GetRateWithSurchargesForNetwork(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime, int fullPallets, int halfPallets, int quarterPallets, int oversizePallets)
        {
            try
            {
                return RequestWithSurchargesForNetwork(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, fullPallets, halfPallets, quarterPallets, oversizePallets, false);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public RateWithSurcharges GetRatewithClientSurcharges(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime)
        {
            try
            {
                return RequestRateWithSurcharges(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, true);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public RateWithSurcharges GetRateWithClientSurchargesForNetwork(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime, int fullPallets, int halfPallets, int quarterPallets, int oversizePallets)
        {
            try
            {
                return RequestWithSurchargesForNetwork(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, fullPallets, halfPallets, quarterPallets, oversizePallets, true);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        #region Private Methods

        private RateWithSurcharges RequestRateWithSurcharges(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime, bool isClient)
        {
            if (this.User != null && this.User is Orchestrator.Entities.CustomPrincipal)
            {
                IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;

                Facade.IOrder facOrder = new Facade.Order();
                Repositories.DTOs.RateInformation rateInformation = facOrder.GetRate(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, true, out surcharges, isClient);

                if (rateInformation != null)
                {
                    return new RateWithSurcharges(rateInformation, surcharges);
                }
                else
                    return null;
            }
            else
            {
                // Do not allow the user to retrieve the rate.
                throw new System.Security.SecurityException("Your current credentials do not have adequate permissions for this operation");
            }
        }

        private RateWithSurcharges RequestWithSurchargesForNetwork(int customerIdentityID, int businessTypeID, int orderInstructionID, int collectionPointID, int deliveryPointID, int palletTypeID, decimal palletSpaces, int weight, int goodsTypeID, int orderServiceLeveID, DateTime collectionDateTime, DateTime deliveryDateTime, int fullPallets, int halfPallets, int quarterPallets, int oversizePallets, bool isClient)
        {
            if (this.User != null && this.User is Orchestrator.Entities.CustomPrincipal)
            {
                IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;

                Facade.IOrder facOrder = new Facade.Order();
                Repositories.DTOs.RateInformation rateInformation = facOrder.GetRate(customerIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLeveID, collectionDateTime, deliveryDateTime, true, out surcharges, fullPallets, halfPallets, quarterPallets, oversizePallets, isClient);

                if (rateInformation != null)
                {
                    return new RateWithSurcharges(rateInformation, surcharges);
                }
                else
                    return null;
            }
            else
            {
                // Do not allow the user to retrieve the rate.
                throw new System.Security.SecurityException("Your current credentials do not have adequate permissions for this operation");
            }
        }

        #endregion

        /// <summary>
        /// This class is needed to allow LINQ to Entities to return an object that is like a KeyValuePair but allows the 
        /// Key to be set after the object has been constructed.  Normally you would use the constructor, but that is not
        /// allowed - throws a "Only parameterless constructors and initializers are supported by LINQ to Entities." exception.
        /// </summary>
        /// <typeparam name="K">The type of the Key</typeparam>
        /// <typeparam name="V">The type of the Value</typeparam>
        private class RelaxedKeyValuePair<K, V>
        {
            public K Key { get; set; }
            public V Value { get; set; }
        }
    }

    /// <summary>
    /// Used to allow the user to retrieve a rate along with any surcharges that can be selected.
    /// </summary>
    public class RateWithSurcharges
    {
        private Repositories.DTOs.RateInformation m_rateInformation;
        private List<Repositories.DTOs.RateSurcharge> m_surcharges = new List<Repositories.DTOs.RateSurcharge>();

        public dynamic Rate
        {
            get
            {
                return new
                {
                    Rate = m_rateInformation.Rate,
                    ForeignRate = m_rateInformation.ForeignRate,
                    TariffDescription = m_rateInformation.TariffDescription,
                    TariffTableDescription = m_rateInformation.TariffTableDescription,
                };
            }
        }

        public dynamic Surcharges
        {
            get
            {
                return m_surcharges.Select(s => new
                {
                    ExtraTypeID = s.ExtraTypeID,
                    Rate = s.Rate,
                    ForeignRate = s.ForeignRate,
                    DisplayRate = s.DisplayRate,
                    Description = s.Description,
                });
            }
        }

        public RateWithSurcharges(Repositories.DTOs.RateInformation rateInformation)
        {
            m_rateInformation = rateInformation;
        }

        public RateWithSurcharges(Repositories.DTOs.RateInformation rateInformation, IEnumerable<Repositories.DTOs.RateSurcharge> surcharges)
            : this(rateInformation)
        {
            if (surcharges != null)
                m_surcharges = new List<Repositories.DTOs.RateSurcharge>(surcharges);
        }
    }

}