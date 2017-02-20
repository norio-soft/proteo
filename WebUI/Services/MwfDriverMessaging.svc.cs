using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Orchestrator.Models;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Services
{

    [ServiceContract(Namespace = "Orchestrator.WebUI.Services")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MwfDriverMessaging
    {

        [OperationContract]
        public void SendMessage(IEnumerable<int> driverIDs, DateTime? messageDateTime, int? pointID, int? heJobID, string message)
        {
            if (driverIDs == null || !driverIDs.Any())
                throw new ApplicationException("No driver IDs passed to SendMessage web service method");

            var instructionsToCommunicate = new Dictionary<MWF_Instruction, MWFDeviceDataActionEnum>(driverIDs.Count());
            
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var jobRepo = DIContainer.CreateRepository<IMWF_JobRepository>(uow);

                foreach (var driverID in driverIDs)
                {
                    Models.MWF_Job job = null;

                    if (heJobID.HasValue)
                        job = jobRepo.FindForHaulierEnterpriseJob(heJobID.Value);

                    if (job == null)
                    {
                        job = new MWF_Job { InternalID = new Guid(), Title = "Message", HEJobID = heJobID };
                        jobRepo.Add(job);
                    }

                    var instruction = new MWF_Instruction
                    {
                        InternalID = Guid.Empty,
                        IsChanged = true,
                        InstructionType = MWFInstructionTypeEnum.OrderMessage,
                        ArriveDateTime = (messageDateTime ?? DateTime.Today).ToLocalTime(),
                        PointID = pointID,
                        DriverID = driverID,
                        CommunicationStatus = MWFCommunicationStatusEnum.NewInformationNeedsCommunicating,
                    };
                    Facade.MWF.ICommunication facComm = new Facade.MWF.Communication();

                    instruction.DeviceData = facComm.GenerateDeviceDataForNonOrderInstruction(job.ID, job.Title, instruction.InstructionType, instruction.PointID, instruction.ArriveDateTime, instruction.DepartDateTime, 1, "ADD", message);
                    
                    job.Instructions.Add(instruction);
                    instructionsToCommunicate.Add(instruction, MWFDeviceDataActionEnum.Add);
                }

                uow.SaveChanges();

                MobileWorkerFlow.MWFServicesCommunication.Client.CommunicateInstructions(new Guid(Orchestrator.Globals.Configuration.BlueSphereCustomerId), instructionsToCommunicate);
                uow.SaveChanges();
            }
        }

    }

}
