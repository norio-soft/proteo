using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using Orchestrator.Repositories;
using Orchestrator.Models;

namespace Orchestrator.WebUI.mwf
{
    public partial class ReceiveMwfPhoto : System.Web.UI.Page
    {
        private string MwfInternalId;
        private string FileName;
        private string FileFormat;
        private string DriverName;
        private string PhotoDateTime;
        private string PhotoComment;
        private string Latitude;
        private string Longitude;
        private string MobileApplicationDataIds;
        private string HEOrderIds;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessReceivedMwfPhoto();
        }

        /// <summary>
        /// Process a photo received from the Tough Touch
        /// </summary>
        private void ProcessReceivedMwfPhoto()
        {
            SetRequestFields();

            if (PhotoHasAlreadyBeenReceived())
                return;

            SavePostedPhoto();

            List<Guid> PhotoMadIds = GetMadIds();
            List<int> PhotoOrderIds = GetOrderIds();

            int? driverId = GetDriverId();

            using (IUnitOfWork uow = DIContainer.CreateUnitOfWork())
            {
                int MwfPhotoId = InsertMwfPhotoTable(uow);

                if (PhotoMadIds == null || PhotoMadIds.Count == 0) // I.e. not associated with any instructions
                {
                    InsertHEPhotoTable(uow, MwfPhotoId, null, null, driverId);
                }
                else
                {
                    int count = 0;
                    foreach (int orderId in PhotoOrderIds)
                    {
                        Guid madId = PhotoMadIds[count];
                        int? instructionId = GetInstructionId(uow, madId);
                        InsertHEPhotoTable(uow, MwfPhotoId, orderId, instructionId, driverId);
                        count++;
                    }
                }

                uow.SaveChanges();
            }
        }

        /// <summary>
        /// Put posted fields into private variables
        /// </summary>
        private void SetRequestFields()
        {
            string parameterToValidate = string.Empty;

            try
            {
                parameterToValidate = "MwfInternalId";
                MwfInternalId = this.Request["MwfInternalId"];

                parameterToValidate = "FileName";
                FileName = this.Request["filename"];

                parameterToValidate = "FileFormat";
                FileFormat = this.Request["fileformat"];

                parameterToValidate = "DriverName";
                DriverName = this.Request["DriverName"];

                parameterToValidate = "PhotoDateTime";
                PhotoDateTime = this.Request["PhotoDateTime"];

                parameterToValidate = "PhotoComment";
                PhotoComment = this.Request["PhotoComment"].Trim();

                parameterToValidate = "Latitude";
                Latitude = this.Request["Latitude"];

                parameterToValidate = "Longitude";
                Longitude = this.Request["Longitude"];

                parameterToValidate = "MobileApplicationDataIds";
                MobileApplicationDataIds = this.Request["MobileApplicationDataIds"];

                parameterToValidate = "HEOrderIds";
                HEOrderIds = this.Request["HEOrderIds"];
            }
            catch (Exception x)
            {
                throw new ApplicationException(String.Format("Unable to receive MWF photos because posted parameter {0} was invalid", parameterToValidate), x);
            }
        }

        /// <summary>
        /// Save the posted photo to disk
        /// </summary>
        private void SavePostedPhoto()
        {
            try
            {
                Uri uri = new Uri(Globals.Configuration.TTPhotoStorageLocation);
                string photoFileFolder = Server.MapPath(uri.AbsolutePath);
                this.Request.Files[0].SaveAs(Path.Combine(photoFileFolder, FileName));
            }
            catch (Exception x)
            {
                throw new ApplicationException(String.Format("Unable to save posted Tough Touch photo {0} to location {1}", FileName, Globals.Configuration.TTPhotoStorageLocation), x);
            }
        }

        /// <summary>
        /// Split out the MobileApplicationIds that were posted across as a comma separated list into a List of Guids
        /// </summary>
        private List<Guid> GetMadIds()
        {
            MobileApplicationDataIds = MobileApplicationDataIds.TrimEnd(','); // Remove trailing comma if there is one
            if (MobileApplicationDataIds.Equals(String.Empty))
                return null;
            else
                return MobileApplicationDataIds.Split(',').ToList().Select(s => Guid.Parse(s)).ToList();
        }

        /// <summary>
        /// Split out the OrderIds that were posted across as a comma separated list into a List of integers
        /// </summary>
        private List<int> GetOrderIds()
        {
            HEOrderIds = HEOrderIds.TrimEnd(','); // Remove trailing comma if there is one
            if (HEOrderIds.Equals(String.Empty))
                return null;
            else
                return HEOrderIds.Split(',').ToList().Select(s => int.Parse(s)).ToList();
        }

        /// <summary>
        /// Look up the driver name that was posted (as a string) and return the id.
        /// </summary>
        private int? GetDriverId()
        {
            if (DriverName.Equals(string.Empty))
                return null;

            Facade.IDriver facDriver = new Facade.Resource();
            DataSet dsDriver = facDriver.GetForName(DriverName);

            if (dsDriver.Tables[0].Rows.Count == 0)
                return null;

            return Convert.ToInt32(dsDriver.Tables[0].Rows[0]["IdentityId"]);
        }

        /// <summary>
        /// Insert the processed fields into the database table mwf_photo
        /// </summary>
        private int InsertMwfPhotoTable(IUnitOfWork uow)
        {
            MWF_Photo mwfPhoto = new MWF_Photo();

            IMWF_PhotoRepository mwf_PhotoRepo = DIContainer.CreateRepository<IMWF_PhotoRepository>(uow);

            mwfPhoto.MwfInternalId = new Guid(MwfInternalId);
            mwfPhoto.PhotoFileName = FileName;
            mwfPhoto.PhotoFileFormat = FileFormat;
            mwfPhoto.DriverName = DriverName;
            mwfPhoto.PhotoDateTime = Convert.ToDateTime(PhotoDateTime);
            mwfPhoto.PhotoComment = PhotoComment;
            mwfPhoto.Latitude = Convert.ToDouble(Latitude);
            mwfPhoto.Longitude = Convert.ToDouble(Longitude);
            mwfPhoto.MobileApplicationDataIds = MobileApplicationDataIds;
            mwfPhoto.HEOrderIds = HEOrderIds;

            mwf_PhotoRepo.Add(mwfPhoto);

            return mwfPhoto.ID;
        }

        /// <summary>
        /// Insert the processed fields into the database table tblPhoto
        /// </summary>
        private void InsertHEPhotoTable(IUnitOfWork uow, int mwfPhotoId, int? orderId, int? instructionId, int? driverId)
        {
            Photo photo = new Photo();

            IPhotoRepository photoRepo = DIContainer.CreateRepository<IPhotoRepository>(uow);

            photo.MwfPhotoID = mwfPhotoId;
            photo.OrderID = orderId;
            photo.InstructionID = instructionId;
            photo.DriverID = driverId;

            photoRepo.Add(photo);
        }

        /// <summary>
        /// Return an instruction id based on a MobileApplicationId
        /// </summary>
        private int? GetInstructionId(IUnitOfWork uow, Guid madId)
        {
            IMWF_InstructionRepository mwf_InstructionRepo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);

            MWF_Instruction instruction = mwf_InstructionRepo.GetForMobileApplicationDataId(madId);

            if (instruction != null)
                return instruction.HEInstructionID;
            else
                return null;
        }

        /// <summary>
        /// Check the Mwf_Photo table to see if the photo has already been sent and processed.
        /// </summary>
        private bool PhotoHasAlreadyBeenReceived()
        {
            using (IUnitOfWork uow = DIContainer.CreateUnitOfWork())
            {
                IMWF_PhotoRepository mwf_PhotoRepo = DIContainer.CreateRepository<IMWF_PhotoRepository>(uow);

                MWF_Photo mwfPhoto = mwf_PhotoRepo.GetMwfPhotoByMadIdAndPhotoName(new Guid(MwfInternalId), FileName);

                if (mwfPhoto != null)
                    return true;
                else
                    return false;
            }
        }

    }
}