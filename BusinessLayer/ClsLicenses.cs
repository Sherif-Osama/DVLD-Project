using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Transactions;

namespace BusinessLayer
{
    public class ClsLicenses
    {

        // Reasons a license can be issued.
        public enum EnIssueReason { FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4 };

        // Persistence mode (whether this instance will be inserted or updated).
        public enum EnMode { AddNew = 1, Update = 2 };
        public EnMode Mode { get; private set; }

        // License properties
        public int LicenseID { private set; get; }
        public int ApplicationID { set; get; }
        public int DriverID { set; get; }
        public ClsDriver DriverInfo { get; private set; }
        public int LicenseClassID { set; get; }
        public ClsLicenseClasses LicenseClassesInfo { get; private set; }
        public DateTime IssueDate { set; get; }
        public DateTime ExpirationDate { set; get; }
        public string Notes { set; get; }
        public float PaidFees { set; get; }
        public bool IsActive { set; get; }
        public int CreatedByUserID { get; set; }
        public EnIssueReason IssueReason { set; get; }

        // Human readable issue reason.
        public string IssueReasonText
        {
            get
            {
                switch (IssueReason)
                {
                    case EnIssueReason.FirstTime:
                        return "First Time Issuance";
                    case EnIssueReason.Renew:
                        return "Renewal";
                    case EnIssueReason.DamagedReplacement:
                        return "Damaged Replacement";
                    case EnIssueReason.LostReplacement:
                        return "Lost Replacement";
                    default:
                        return "Unknown";
                }
            }
        }

        public bool IsDetained
        { get => ClsDetainedLicenses.IsLicenseDetained(this.LicenseID); }

        // Default constructor for creating a new license (unsaved).
        public ClsLicenses()
        {
            this.LicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.LicenseClassID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.MinValue;
            this.Notes = string.Empty;
            this.PaidFees = 0;
            this.IsActive = false;
            this.IssueReason = EnIssueReason.FirstTime;
            this.CreatedByUserID = -1;
            this.Mode = EnMode.AddNew;
        }

        // Private constructor to initialize from a DTO (when loading from DB).
        private ClsLicenses(LicenseDTO licenseDTO)
        {
            this.LicenseID = licenseDTO.LicenseID;
            this.ApplicationID = licenseDTO.ApplicationID;
            this.DriverID = licenseDTO.DriverID;
            this.DriverInfo = ClsDriver.Find(this.DriverID);
            this.LicenseClassID = licenseDTO.LicenseClassID;
            this.LicenseClassesInfo = ClsLicenseClasses.Find(this.LicenseClassID);
            this.IssueDate = licenseDTO.IssueDate;
            this.ExpirationDate = licenseDTO.ExpirationDate;
            this.Notes = licenseDTO.Notes;
            this.PaidFees = licenseDTO.PaidFees;
            this.IsActive = licenseDTO.IsActive;
            this.IssueReason = (EnIssueReason)licenseDTO.IssueReason;
            this.CreatedByUserID = licenseDTO.CreatedByUserID;
            this.Mode = EnMode.Update;
        }

        // Map this business object to a DTO for persistence.
        private LicenseDTO MappingToDTO()
        {
            return new LicenseDTO
            {
                LicenseID = this.LicenseID,
                ApplicationID = this.ApplicationID,
                DriverID = this.DriverID,
                LicenseClassID = this.LicenseClassID,
                IssueDate = this.IssueDate,
                ExpirationDate = this.ExpirationDate,
                Notes = this.Notes,
                PaidFees = this.PaidFees,
                IsActive = this.IsActive,
                IssueReason = (byte)this.IssueReason,
                CreatedByUserID = this.CreatedByUserID
            };
        }
        #region Find Methods

        // Data access helpers to fetch driver licenses or specific license records.
        public static DataTable GetDriverLicenses(int DriverID) => ClsLicensesData.GetDriverLicenses(DriverID);

        public static ClsLicenses FindByApplicationID(int ApplicationID)
        {
            LicenseDTO LicenseDTO = ClsLicensesData.FindByApplicationID(ApplicationID);
            if (LicenseDTO == null) { return null; }

            return new ClsLicenses(LicenseDTO);
        }

        public static ClsLicenses Find(int LicenseID)
        {
            LicenseDTO LicenseDTO = ClsLicensesData.Find(LicenseID);
            if (LicenseDTO == null) { return null; }

            return new ClsLicenses(LicenseDTO);
        }

        // Find license by driver and class.
        public static ClsLicenses Find(int DriverID, int LicenseClassID)
        {
            LicenseDTO License = ClsLicensesData.Find(DriverID, LicenseClassID);

            if (License == null) { return null; }

            return new ClsLicenses(License);
        }

        // Find active license for driver and class.
        public static ClsLicenses FindActiveLicenseByDriverID(int DriverID, int LicenseClassID)
        {
            LicenseDTO License = ClsLicensesData.FindActiveLicenseByDriverID(DriverID, LicenseClassID);

            if (License == null) { return null; }

            return new ClsLicenses(License);
        }
        #endregion

        #region Check License Existence
        // Helpers that query the data layer to check if licenses exist.
        public static bool IsLicenseExist(int Person, int LicenseClassID)
        {
            if (LicenseClassID <= 0) { return false; }
            return ClsLicensesData.IsLicenseExist(Person, LicenseClassID);
        }

        public static bool HasLicense(int DriverID, int LicenseClassID)
        {
            if (DriverID == -1) { return false; }
            return ClsLicensesData.HasLicense(DriverID, LicenseClassID);
        }

        public static bool HasActiveLicense(int Driver, int LicenseClassID)
        {
            if (Driver == -1) { return false; }
            return ClsLicensesData.HasActiveLicense(Driver, LicenseClassID);
        }

        #endregion
        private bool AddNew()
        {
            this.LicenseID = ClsLicensesData.AddNew(MappingToDTO());
            return this.LicenseID != -1;
        }

        private bool Update() => ClsLicensesData.Update(MappingToDTO());

        private bool BusinessRules()
        {
            if (Mode == EnMode.AddNew)
            {
                return !HasActiveLicense(this.DriverID, this.LicenseClassID);
            }
            else if (Mode == EnMode.Update) { return HasLicense(this.DriverID, this.LicenseClassID); }

            return false;
        }

        // Save entry point that validates business rules and performs add or update.
        public bool Save()
        {
            if (BusinessRules())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (AddNew()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return Update();
                }
            }
            return false;
        }

        #region Renew/Replace License Method
        // Create an application record for renewing a license.
        private ClsApplications CreateRenewApplication(int RenewByUserID)
        {
            if (this.DriverInfo != null)
            {
                return new ClsApplications
                {
                    ApplicantPersonID = this.DriverInfo.PersonID,
                    ApplicationDate = DateTime.Now,
                    ApplicationType = ClsApplications.EnApplicationType.RenewDrivingLicense,
                    ApplicationTypeID = (int)ClsApplications.EnApplicationType.RenewDrivingLicense,
                    LastStatusDate = DateTime.Now,
                    PaidFees = Convert.ToSingle(ClsApplicationsTypes.Find((int)ClsApplications.EnApplicationType.RenewDrivingLicense)?.Fees ?? -1),
                    CreatedByUserID = RenewByUserID,
                    ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                };
            }
            return null;
        }

        public bool DeactivateCurrentLicense()
        {
            if (this.IsActive) { return ClsLicensesData.DeactivateCurrentLicense(this.LicenseID); }
            return true;
        }

        public ClsLicenses RenewLicense(string Note, int RenewByUserID)
        {
            if (this.ExpirationDate < DateTime.Now)
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ClsApplications RenewApplication = CreateRenewApplication(RenewByUserID);
                    if (RenewApplication != null && RenewApplication.Save())
                    {
                        ClsLicenses NewLicense = new ClsLicenses();

                        NewLicense.ApplicationID = RenewApplication.ApplicationID;
                        NewLicense.DriverID = this.DriverID;
                        NewLicense.LicenseClassID = this.LicenseClassID;
                        NewLicense.IssueDate = DateTime.Now;
                        NewLicense.ExpirationDate = DateTime.Now.AddYears(this.LicenseClassesInfo.DefaultValidityLength);
                        NewLicense.Notes = Note;
                        NewLicense.PaidFees = this.LicenseClassesInfo?.ClassFees ?? -1;
                        NewLicense.IsActive = true;
                        NewLicense.IssueReason = ClsLicenses.EnIssueReason.Renew;
                        NewLicense.CreatedByUserID = this.CreatedByUserID;

                        // Deactivate the old license before saving the new one.
                        if (this.DeactivateCurrentLicense())
                        {
                            if (NewLicense.Save())
                            {
                                if (RenewApplication.SetComplete())
                                { Scope.Complete(); return NewLicense; }
                                else
                                { return null; }
                            }
                        }
                    }
                    else
                    { return null; }
                }
            }
            return null;
        }

        private ClsApplications CreateReplacementApplication(int CreateByUserID, EnIssueReason IssueReason)
        {
            if (this.DriverInfo != null)
            {
                ClsApplications ReplacementApplication = new ClsApplications();

                ReplacementApplication.ApplicantPersonID = this.DriverInfo.PersonID;
                ReplacementApplication.ApplicationDate = DateTime.Now;
                ReplacementApplication.ApplicationType = IssueReason == EnIssueReason.LostReplacement ? ClsApplications.EnApplicationType.ReplaceLostDrivingLicense : ClsApplications.EnApplicationType.ReplaceDamagedDrivingLicense;
                ReplacementApplication.ApplicationTypeID = (int)ReplacementApplication.ApplicationType;
                ReplacementApplication.LastStatusDate = DateTime.Now;
                ReplacementApplication.PaidFees = Convert.ToSingle(ClsApplicationsTypes.Find(ReplacementApplication.ApplicationTypeID)?.Fees ?? -1);
                ReplacementApplication.CreatedByUserID = CreateByUserID;
                ReplacementApplication.ApplicationStatus = ClsApplications.EnApplicationStatus.New;

                return ReplacementApplication;

            }
            return null;
        }

        public ClsLicenses Replace(EnIssueReason IssueReason, int CreatedByUserID)
        {
            if (this.IsActive)
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ClsApplications ReplacementAppliction = CreateReplacementApplication(CreatedByUserID, IssueReason);
                    if (ReplacementAppliction != null && ReplacementAppliction.Save())
                    {
                        ClsLicenses NewLicense = new ClsLicenses();

                        NewLicense.ApplicationID = ReplacementAppliction.ApplicationID;
                        NewLicense.DriverID = this.DriverID;
                        NewLicense.LicenseClassID = this.LicenseClassID;
                        NewLicense.IssueDate = DateTime.Now;

                        NewLicense.ExpirationDate = this.ExpirationDate;
                        NewLicense.Notes = this.Notes;
                        NewLicense.PaidFees = this.PaidFees;
                        NewLicense.IsActive = this.IsActive;
                        NewLicense.IssueReason = IssueReason;
                        NewLicense.CreatedByUserID = this.CreatedByUserID;


                        // Deactivate the old license before saving the replacement.
                        if (this.DeactivateCurrentLicense())
                        {
                            if (NewLicense.Save())
                            {
                                if (ReplacementAppliction.SetComplete())
                                { Scope.Complete(); return NewLicense; }
                                else
                                { return null; }
                            }
                        }
                    }
                    else
                    { return null; }
                }
            }
            return null;
        }
        #endregion

        #region detain/Release License Method

        public int Detain(float FineFees, int CreatedByUserID)
        {
            if (!this.IsDetained)
            {
                ClsDetainedLicenses DetainedLicense = new ClsDetainedLicenses();
                DetainedLicense.LicenseID = this.LicenseID;
                DetainedLicense.DetainDate = DateTime.Now;
                DetainedLicense.FineFees = Convert.ToSingle(FineFees);
                DetainedLicense.CreatedByUserID = CreatedByUserID;

                if (DetainedLicense.Save())
                {
                    return DetainedLicense.DetainID;
                }
                else { return -1; }
            }
            else
            { return -1; }
        }

        private ClsApplications CreateReleaseApplication(int ReleasedByUserID)
        {
            if (this.DriverInfo != null)
            {
                return new ClsApplications
                {
                    ApplicantPersonID = this.DriverInfo.PersonID,
                    ApplicationDate = DateTime.Now,
                    ApplicationType = ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense,
                    ApplicationTypeID = (int)ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense,
                    LastStatusDate = DateTime.Now,
                    PaidFees = Convert.ToSingle(ClsApplicationsTypes.Find((int)ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense)?.Fees ?? -1),
                    CreatedByUserID = ReleasedByUserID,
                    ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                };
            }
            return null;
        }

        public bool Release(int ReleasedByUserID)
        {
            if (this.IsDetained)
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ClsApplications ReleaseApplications = CreateReleaseApplication(ReleasedByUserID);

                    if (ReleaseApplications != null && ReleaseApplications.Save())
                    {
                        ClsDetainedLicenses DetainedLicense = ClsDetainedLicenses.FindByLicenseID(this.LicenseID);
                        if (DetainedLicense != null)
                        {
                            if (DetainedLicense.Release(ReleasedByUserID, ReleaseApplications.ApplicationID))
                            {
                                if (ReleaseApplications.SetComplete())
                                { Scope.Complete(); return true; }
                                else { return false; }
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}