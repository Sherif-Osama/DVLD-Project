using DataAccessLayer;
using DTO;
using System;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ClsApplications
    {
        // Internal mode to indicate whether this instance should be inserted or updated.
        protected enum EnMode { AddNew = 1, Update = 2 };
        public enum EnApplicationType
        {
            NewDrivingLicense = 1, RenewDrivingLicense = 2, ReplaceLostDrivingLicense = 3,
            ReplaceDamagedDrivingLicense = 4, ReleaseDetainedDrivingLicense = 5, NewInternationalLicense = 6, RetakeTest = 7
        };

        // Public enum describing possible application statuses.
        public enum EnApplicationStatus { New = 1, Cancelled = 2, Completed = 3 };
        public EnApplicationStatus ApplicationStatus { get; set; }
        protected EnMode Mode { get; set; }
        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }

        public ClsPerson ApplicantPersonInfo { get; protected set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public ClsApplicationsTypes ApplicationTypeInfo { get; protected set; }
        public EnApplicationType ApplicationType { get; set; }


        // Returns a human-readable string for the current ApplicationStatus.
        public string StatusText
        {
            get
            {
                switch (ApplicationStatus)
                {
                    case EnApplicationStatus.New:
                        return "New";
                    case EnApplicationStatus.Cancelled:
                        return "Cancelled";
                    case EnApplicationStatus.Completed:
                        return "Completed";
                    default:
                        return "Unknown";
                }
            }
        }
        public DateTime LastStatusDate { get; set; }
        public float PaidFees { get; set; }
        public int CreatedByUserID { get; set; }

        public ClsUser CreatedByUserInfo { get; protected set; }

        #region Constructors
        // Default constructor: initialize defaults and mark as AddNew for insertion.
        public ClsApplications()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.LastStatusDate = DateTime.Now;
            this.PaidFees = -1;
            this.CreatedByUserID = -1;
            this.ApplicationStatus = EnApplicationStatus.New;
            this.Mode = EnMode.AddNew;
        }

        // Create business object from DTO and set mode to Update (existing record).
        protected ClsApplications(ApplicationDTO Application)
        {
            this.ApplicationID = Application.ApplicationID;
            this.ApplicantPersonID = Application.ApplicantPersonID;
            this.ApplicationDate = Application.ApplicationDate;
            this.ApplicationTypeID = Application.ApplicationTypeID;
            this.ApplicationType = (EnApplicationType)ApplicationTypeID;
            this.ApplicationStatus = (EnApplicationStatus)Application.Status;
            this.LastStatusDate = Application.LastStatusDate;
            this.PaidFees = Application.PaidFees;
            this.CreatedByUserID = Application.CreatedByUserID;
            this.Mode = EnMode.Update;
        }
        #endregion

        // Map this business object into an ApplicationDTO for data-layer operations.
        private ApplicationDTO MappingToDTO()
        {
            return new ApplicationDTO
            {
                ApplicationID = this.ApplicationID,
                ApplicantPersonID = this.ApplicantPersonID,
                ApplicationDate = this.ApplicationDate,
                ApplicationTypeID = this.ApplicationTypeID,
                Status = (byte)this.ApplicationStatus,
                LastStatusDate = this.LastStatusDate,
                PaidFees = this.PaidFees,
                CreatedByUserID = this.CreatedByUserID
            };
        }

        #region Find Method
        private async Task LoadRelatedDataAsync()
        {
            this.ApplicantPersonInfo = await ClsPerson.FindAsync(this.ApplicantPersonID);
            this.ApplicationTypeInfo = await ClsApplicationsTypes.FindAsync(this.ApplicationTypeID);
            this.CreatedByUserInfo = await ClsUser.FindAsync(this.CreatedByUserID);
        }

        // Retrieve an application by ID from the data layer; return business object or null.
        public static async Task<ClsApplications> FindAsync(int ApplicationID)
        {
            ApplicationDTO ApplicationDTO = await ClsApplicationsData.FindAsync(ApplicationID);

            if (ApplicationDTO == null) { return null; }

            ClsApplications ApplicationObj = new ClsApplications(ApplicationDTO);

            await ApplicationObj.LoadRelatedDataAsync();

            return ApplicationObj;
        }

        //Find Application for a Person by Appliction Type
        public static async Task<ClsApplications> FindActiveApplicationTypeAsync(int ApplicantPersonID, int ApplicationTypeID)
        {
            ApplicationDTO ApplicationDTO = await ClsApplicationsData.FindActiveApplicationTypeAsync(ApplicantPersonID, ApplicationTypeID);

            if (ApplicationDTO == null) { return null; }

            ClsApplications ApplicationObj = new ClsApplications(ApplicationDTO);

            await ApplicationObj.LoadRelatedDataAsync();

            return ApplicationObj;
        }
        #endregion

        #region AddNew/Update/Delete Methods
        public virtual Task<bool> DeleteAsync() => ClsApplicationsData.DeleteAsync(this.ApplicationID);

        private async Task<bool> ChangeStatusAsync(EnApplicationStatus newStatus)
        {
            if (await ClsApplicationsData.UpdateStatusAsync(this.ApplicationID, (short)newStatus))
            {
                this.ApplicationStatus = newStatus;
                this.LastStatusDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public Task<bool> CancelAsync() => ChangeStatusAsync(EnApplicationStatus.Cancelled);
        public Task<bool> SetCompleteAsync() => ChangeStatusAsync(EnApplicationStatus.Completed);

        // Insert this instance as a new record and update ApplicationID; return success.
        private async Task<bool> AddNewAsync()
        {
            this.ApplicationID = await ClsApplicationsData.AddNewAsync(MappingToDTO());

            return (this.ApplicationID != -1);
        }

        // Update existing record in the data layer using the mapped DTO; return success.
        private Task<bool> UpdateAsync() => ClsApplicationsData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            if (Mode == EnMode.AddNew)
            {
                if (this.ApplicationType == EnApplicationType.RetakeTest || this.ApplicationType == EnApplicationType.NewDrivingLicense)
                    return true;

                return (await FindActiveApplicationTypeAsync(this.ApplicantPersonID, this.ApplicationTypeID) == null);
            }
            else if (Mode == EnMode.Update)
            { return (this.ApplicationStatus != EnApplicationStatus.Cancelled); }

            return false;
        }

        // Persist this object: insert if AddNew (and switch to Update on success), otherwise update.
        public virtual async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewAsync()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return await UpdateAsync();
                    default: return false;
                }
            }
            return false;
        }
        #endregion
    }
}