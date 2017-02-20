using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Services
{

    [ServiceContract(Namespace = "Orchestrator.WebUI.Services")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class VehiclePlanning
    {

        public class Vehicle
        {
            public int ResourceID { get; set; }
            public string RegistrationNumber { get; set; }
        }

        public class Driver
        {
            public int ResourceID { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class ResourceUnit
        {
            public int ResourceUnitID { get; set; }
            public string Description { get; set; }
            public string Driver { get; set; }
            public string Vehicle { get; set; }
            public string Trailer { get; set; }
        }

        // Another order Object but only for Pre Planning Purposes (for now)
        public class Order
        {
            public int OrderID { get; set; }
            public string CustomerName { get; set; }
            public string CollectionPoint { get; set; }
            public string DeliveryPoint { get; set; }
            public DateTime CollectionDate { get; set; }
            public DateTime DeliveryDate { get; set; }
            public decimal Weight { get; set; }
            public string Manifest { get; set; }
            public int? CollectionLCID { get; set; }
            public int? DeliveryLCID { get; set; }
            public bool IsImport { get; set; }
            public bool IsExport { get; set; }
            public bool IsUK { get; set; }
            public decimal? Rate { get; set; }
            public int RateLCID { get; set; }
            public string CollectionTown { get; set; }
            public string  DeliveryTown { get; set; }
            public int GoodsTypeID { get; set; }
            public DateTime CreateDate { get; set; }
        }

        [OperationContract]
        public IEnumerable<Vehicle> GetAllVehicles()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IVehicleRepository>(uow);

                var retVal = repo.GetAll()
                    .OrderBy(v => v.RegNo)
                    .Select(v => new Vehicle { ResourceID = v.ResourceID, RegistrationNumber = v.RegNo }).ToList();

                return retVal;
            }
        }

        [OperationContract]
        public IEnumerable<Orchestrator.Repositories.DTOs.PlanningResourceUnit> GetAllResourceUnits()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IResourceUnitRepository>(uow);

                var retVal = repo.GetForPlanning()
                    .OrderBy(v => v.ResourceUnitTypeID)
                    .Select(o => o).ToList();

                return retVal;
            }
        }

        [OperationContract]
        public IEnumerable<Repositories.DTOs.VehicleOrderPlan> GetVehicleOrderPlans(DateTime dateFrom, DateTime dateTo)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrderRepository>(uow);
                var retVal = repo.GetOrderVehiclePlanningForDateRange(dateFrom, dateTo).ToList();
                return retVal;
            }
        }

        [OperationContract]
        public void OrderPrePlanCreate(int unplannedOrderID, int resourceUnitID, DateTime arrivalDateTime)
        {
            //TODO: need to ensure that this order is not already planned or pre-planned
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrderRepository>(uow);
                var order = repo.Find(unplannedOrderID);
                var deldate = arrivalDateTime.AddDays(order.DeliveryDateTime.Subtract(order.CollectionDateTime).TotalDays);
                repo.AddOrderPrePlan(unplannedOrderID, resourceUnitID, arrivalDateTime, deldate);
                uow.SaveChanges();
            }
        }

        [OperationContract(Name="OrderPrePlanCreate2")]
        public void OrderPrePlanCreate(int unplannedOrderID, int resourceUnitID, DateTime arrivalDateTime, DateTime deliveryDate)
        {
            //TODO: need to ensure that this order is not already planned or pre-planned
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrderRepository>(uow);
                repo.AddOrderPrePlan(unplannedOrderID, resourceUnitID, arrivalDateTime, deliveryDate);
                uow.SaveChanges();
            }
        }

        [OperationContract]
        public void OrderPlanChange(int orderID, int instructionTypeID, int? instructionID, int vehicleResourceID, DateTime arrivalDateTime)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                if (instructionID.HasValue)
                {
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Facade.IJob facJob = new Facade.Job();

                    var instructionRepo = DIContainer.CreateRepository<IInstructionRepository>(uow);
                    var jobID = instructionRepo.Find(instructionID.Value).JobID;

                    var job = facJob.GetJob(jobID, true, true);
                    var instruction = job.Instructions.Find(i => i.InstructionID == instructionID.Value);

                    var changeTimes = instruction.PlannedArrivalDateTime != arrivalDateTime;
                    var changeVehicle = instruction.Vehicle.ResourceId != vehicleResourceID;

                    var userName = HttpContext.Current.User.Identity.Name;

                    if (changeVehicle)
                    {
                        var result = facInstruction.PlanInstruction(new[] { instructionID.Value }, jobID, -1, vehicleResourceID, -1, job.LastUpdateDate, userName);

                        if (!result.Success)
                            throw new ApplicationException(
                                string.Format(
                                    "Could not update vehicle on run {0} due to the following infringements: {1}",
                                    jobID,
                                    string.Join("\n", result.Infringements.Select(i => i.Description))));
                    }

                    if (changeTimes)
                    {
                        instruction.PlannedArrivalDateTime = arrivalDateTime;
                        instruction.PlannedDepartureDateTime = arrivalDateTime.AddMinutes(15);
                        var result = facJob.UpdatePlannedTimes(jobID, job.Instructions, job.LastUpdateDate, userName);

                        if (!result.Success)
                            throw new ApplicationException(
                                string.Format(
                                    "Could not update planned times on run {0} due to the following infringements: {1}",
                                    jobID,
                                    string.Join("\n", result.Infringements.Select(i => i.Description))));
                    }
                }
                else
                {
                    var repo = DIContainer.CreateRepository<IOrderRepository>(uow);
                    repo.AddUpdateOrderPrePlan(orderID, (eInstructionType)instructionTypeID, vehicleResourceID, arrivalDateTime);
                    uow.SaveChanges();
                }
            }
        }

        [OperationContract]
        public void OrderPrePlanDelete(int orderID)
        {
            //TODO: need to ensure that this order is not already planned or pre-planned
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrderRepository>(uow);
                repo.RemoveOrderPrePlan(orderID);
                uow.SaveChanges();
            }
        }

        [OperationContract]
        public IEnumerable<Order> GetOrdersForPrePlanning(int filteroption)
        {
            //filteroptions 
            // 0 all
            // 1 import
            // 2 export
            //3 UK Only

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrderRepository>(uow);

                var retVal = repo.GetOrdersForPrePlanning()
                    .OrderBy(o => o.CreateDate)
                    .Select(o => new Order
                        {
                            OrderID = o.OrderID,
                            CustomerName = o.CustomerOrganisation.OrganisationName,
                            CollectionDate = o.CollectionDateTime,
                            DeliveryDate = o.DeliveryDateTime,
                            CollectionPoint = o.CollectionPoint.Description,
                            DeliveryPoint = o.DeliveryPoint.Description,
                            Weight = o.Weight,
                            Manifest = o.DeliveryOrderNumber,
                            CollectionLCID = o.CollectionPoint.Address.Country.DefaultLCID,
                            DeliveryLCID = o.DeliveryPoint.Address.Country.DefaultLCID,
                            IsImport = o.CollectionPoint.Address.Country.DefaultLCID.HasValue ? o.CollectionPoint.Address.Country.DefaultLCID.Value != 2057 : false,
                            IsExport = o.CollectionPoint.Address.Country.DefaultLCID.HasValue ? o.DeliveryPoint.Address.Country.DefaultLCID.Value != 2057 : false,
                            IsUK = o.CollectionPoint.Address.Country.DefaultLCID.HasValue ? o.CollectionPoint.Address.Country.DefaultLCID.Value == 2057 && o.DeliveryPoint.Address.Country.DefaultLCID.Value == 2057 : true,
                            Rate = o.ForeignRate,
                            RateLCID = o.LCID.HasValue ? o.LCID.Value : 2057,
                            CollectionTown = o.CollectionPoint.Address.PostTown,
                            DeliveryTown = o.DeliveryPoint.Address.PostTown,
                            GoodsTypeID = o.GoodsTypeID,
                            CreateDate = o.CreateDate,
                        }).Where(q => filteroption == 0 ||
                                     filteroption == 1 && q.IsImport ||
                                     filteroption == 2 && q.IsExport ||
                                     filteroption == 3 && q.IsUK).ToList();

                return retVal;

            }
        }

        [OperationContract]
        public IEnumerable<Repositories.DTOs.Schedule> GetSchedules(DateTime dateFrom, DateTime dateTo)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IResourceUnitRepository>(uow);
                var retVal = repo.GetSchedules(dateFrom, dateTo);
                return retVal.ToList();
            }
        }

    }

}
