using DataAccessLayer;
using DTO;
using System;

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

        public ClsPerson ApplicantPersonInfo { get; private set; }
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

        public ClsUser CreatedByUserInfo { get; private set; }

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
            this.ApplicantPersonInfo = ClsPerson.Find(this.ApplicantPersonID);
            this.ApplicationDate = Application.ApplicationDate;
            this.ApplicationTypeID = Application.ApplicationTypeID;
            this.ApplicationType = (EnApplicationType)ApplicationTypeID;
            this.ApplicationTypeInfo = ClsApplicationsTypes.Find(ApplicationTypeID);
            this.ApplicationStatus = (EnApplicationStatus)Application.Status;
            this.LastStatusDate = Application.LastStatusDate;
            this.PaidFees = Application.PaidFees;
            this.CreatedByUserID = Application.CreatedByUserID;
            this.CreatedByUserInfo = ClsUser.Find(this.CreatedByUserID);
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
        // Retrieve an application by ID from the data layer; return business object or null.
        public static ClsApplications Find(int ApplicationID)
        {
            ApplicationDTO Application = ClsApplicationsData.Find(ApplicationID);

            if (Application == null) { return null; }

            return new ClsApplications(Application);
        }

        //Find Application for a Person by Appliction Type
        public static ClsApplications FindActiveApplicationType(int ApplicantPersonID, int ApplicationTypeID)
        {
            ApplicationDTO Application = ClsApplicationsData.FindActiveApplicationType(ApplicantPersonID, ApplicationTypeID);

            if (Application == null) { return null; }

            return new ClsApplications(Application);
        }
        #endregion

        #region AddNew/Update/Delete Methods
        public virtual bool Delete() => ClsApplicationsData.Delete(this.ApplicationID);

        private bool ChangeStatus(EnApplicationStatus newStatus)
        {
            if (ClsApplicationsData.UpdateStatus(this.ApplicationID, (short)newStatus))
            {
                this.ApplicationStatus = newStatus;
                this.LastStatusDate = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool Cancel() => ChangeStatus(EnApplicationStatus.Cancelled);
        public bool SetComplete() => ChangeStatus(EnApplicationStatus.Completed);

        // Insert this instance as a new record and update ApplicationID; return success.
        private bool AddNew()
        {
            this.ApplicationID = ClsApplicationsData.AddNew(MappingToDTO());

            return (this.ApplicationID != -1);
        }

        // Update existing record in the data layer using the mapped DTO; return success.
        private bool Update() => ClsApplicationsData.Update(MappingToDTO());

        private bool BusinessRules()
        {
            if (Mode == EnMode.AddNew)
            {
                if (this.ApplicationType == EnApplicationType.RetakeTest || this.ApplicationType == EnApplicationType.NewDrivingLicense)
                    return true;

                return (FindActiveApplicationType(this.ApplicantPersonID, this.ApplicationTypeID) == null);
            }
            else if (Mode == EnMode.Update)
            { return (this.ApplicationStatus != EnApplicationStatus.Cancelled); }

            return false;
        }

        // Persist this object: insert if AddNew (and switch to Update on success), otherwise update.
        public virtual bool Save()
        {
            if (BusinessRules())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (AddNew()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return Update();
                    default: return false;
                }
            }
            return false;
        }
        #endregion
    }
}