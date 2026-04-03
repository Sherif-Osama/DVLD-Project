using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;
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
        public bool IsDetained { get; private set; }

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
            this.LicenseClassID = licenseDTO.LicenseClassID;
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
        public static Task<DataTable> GetDriverLicensesAsync(int DriverID) => ClsLicensesData.GetDriverLicensesAsync(DriverID);

        private async Task LoadRelatedDataAsync()
        {
            DriverInfo = await ClsDriver.FindAsync(this.DriverID);
            LicenseClassesInfo = await ClsLicenseClasses.FindAsync(this.LicenseClassID);
            IsDetained = await ClsDetainedLicenses.IsLicenseDetainedAsync(this.LicenseID);
        }

        public static async Task<ClsLicenses> FindByApplicationIDAsync(int ApplicationID)
        {
            LicenseDTO LicenseDTO = await ClsLicensesData.FindByApplicationIDAsync(ApplicationID);
            if (LicenseDTO == null) { return null; }

            ClsLicenses LicenseObj = new ClsLicenses(LicenseDTO);

            await LicenseObj.LoadRelatedDataAsync();

            return LicenseObj;
        }

        public static async Task<ClsLicenses> FindAsync(int LicenseID)
        {
            LicenseDTO LicenseDTO = await ClsLicensesData.FindAsync(LicenseID);
            if (LicenseDTO == null) { return null; }

            ClsLicenses LicenseObj = new ClsLicenses(LicenseDTO);

            await LicenseObj.LoadRelatedDataAsync();

            return LicenseObj;
        }

        // Find license by driver and class.
        public static async Task<ClsLicenses> FindAsync(int DriverID, int LicenseClassID)
        {
            LicenseDTO LicenseDTO = await ClsLicensesData.FindAsync(DriverID, LicenseClassID);

            if (LicenseDTO == null) { return null; }

            ClsLicenses LicenseObj = new ClsLicenses(LicenseDTO);

            await LicenseObj.LoadRelatedDataAsync();

            return LicenseObj;
        }

        // Find active license for driver and class.
        public static async Task<ClsLicenses> FindActiveLicenseByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            LicenseDTO LicenseDTO = await ClsLicensesData.FindActiveLicenseByDriverIDAsync(DriverID, LicenseClassID);

            if (LicenseDTO == null) { return null; }

            ClsLicenses LicenseObj = new ClsLicenses(LicenseDTO);

            await LicenseObj.LoadRelatedDataAsync();

            return LicenseObj;
        }
        #endregion

        #region Check License Existence
        // Helpers that query the data layer to check if licenses exist.
        public static async Task<bool> IsLicenseExistAsync(int Person, int LicenseClassID)
        {
            if (LicenseClassID <= 0) { return false; }
            return await ClsLicensesData.IsLicenseExistAsync(Person, LicenseClassID);
        }

        public static async Task<bool> HasLicenseAsync(int DriverID, int LicenseClassID)
        {
            if (DriverID == -1) { return false; }
            return await ClsLicensesData.HasLicenseAsync(DriverID, LicenseClassID);
        }

        public static async Task<bool> HasActiveLicenseAsync(int Driver, int LicenseClassID)
        {
            if (Driver == -1) { return false; }
            return await ClsLicensesData.HasActiveLicenseAsync(Driver, LicenseClassID);
        }

        #endregion
        private async Task<bool> AddNewAsync()
        {
            this.LicenseID = await ClsLicensesData.AddNewAsync(MappingToDTO());
            return this.LicenseID != -1;
        }

        private Task<bool> UpdateAsync() => ClsLicensesData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            if (Mode == EnMode.AddNew)
            {
                return !await HasActiveLicenseAsync(this.DriverID, this.LicenseClassID);
            }
            else if (Mode == EnMode.Update) { return await HasLicenseAsync(this.DriverID, this.LicenseClassID); }

            return false;
        }

        // Save entry point that validates business rules and performs add or update.
        public async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewAsync()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return await UpdateAsync();
                }
            }
            return false;
        }

        #region Renew/Replace License Method
        // Create an application record for renewing a license.
        private async Task<ClsApplications> CreateRenewApplicationAsync(int RenewByUserID)
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
                    PaidFees = Convert.ToSingle((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.RenewDrivingLicense))?.Fees ?? -1),
                    CreatedByUserID = RenewByUserID,
                    ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                };
            }
            return null;
        }

        public async Task<bool> DeactivateCurrentLicenseAsync()
        {
            if (this.IsActive) { return await ClsLicensesData.DeactivateCurrentLicenseAsync(this.LicenseID); }
            return true;
        }
        private async Task<ClsLicenses> CreateNewLicenseAsync(EnIssueReason IssueReason, int CreateByUserID, int ApplicationID, string Note = "")
        {
            ClsLicenses NewLicense = new ClsLicenses();

            NewLicense.ApplicationID = ApplicationID;
            NewLicense.DriverID = this.DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            ClsLicenseClasses NewLicenseClasses = this.LicenseClassesInfo ?? await ClsLicenseClasses.FindAsync(this.LicenseClassID);
            NewLicense.ExpirationDate = DateTime.Now.AddYears(NewLicenseClasses.DefaultValidityLength);
            NewLicense.Notes = Note;
            NewLicense.PaidFees = NewLicenseClasses?.ClassFees ?? -1;
            NewLicense.IsActive = true;
            NewLicense.IssueReason = IssueReason;
            NewLicense.CreatedByUserID = CreateByUserID;

            return NewLicense;
        }

        public async Task<ClsLicenses> RenewLicenseAsync(string Note, int RenewByUserID)
        {
            if (this.ExpirationDate < DateTime.Now)
            {
                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ClsApplications RenewApplication = await CreateRenewApplicationAsync(RenewByUserID);
                    if (RenewApplication != null && await RenewApplication.SaveAsync())
                    {

                        ClsLicenses NewLicense = await CreateNewLicenseAsync(EnIssueReason.Renew, RenewByUserID, RenewApplication.ApplicationID, Note);
                        // Deactivate the old license before saving the new one.
                        if (await this.DeactivateCurrentLicenseAsync())
                        {
                            if (await NewLicense.SaveAsync())
                            {
                                if (await RenewApplication.SetCompleteAsync())
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

        private async Task<ClsApplications> CreateReplacementApplication(int CreateByUserID, EnIssueReason IssueReason)
        {
            if (this.DriverInfo != null)
            {
                ClsApplications ReplacementApplication = new ClsApplications();

                ReplacementApplication.ApplicantPersonID = this.DriverInfo.PersonID;
                ReplacementApplication.ApplicationDate = DateTime.Now;
                ReplacementApplication.ApplicationType = IssueReason == EnIssueReason.LostReplacement ? ClsApplications.EnApplicationType.ReplaceLostDrivingLicense : ClsApplications.EnApplicationType.ReplaceDamagedDrivingLicense;
                ReplacementApplication.ApplicationTypeID = (int)ReplacementApplication.ApplicationType;
                ReplacementApplication.LastStatusDate = DateTime.Now;
                ReplacementApplication.PaidFees = Convert.ToSingle((await ClsApplicationsTypes.FindAsync(ReplacementApplication.ApplicationTypeID))?.Fees ?? -1);
                ReplacementApplication.CreatedByUserID = CreateByUserID;
                ReplacementApplication.ApplicationStatus = ClsApplications.EnApplicationStatus.New;

                return ReplacementApplication;

            }
            return null;
        }

        public async Task<ClsLicenses> ReplaceAsync(EnIssueReason IssueReason, int CreatedByUserID)
        {
            if (this.IsActive)
            {
                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ClsApplications ReplacementAppliction = await CreateReplacementApplication(CreatedByUserID, IssueReason);
                    if (ReplacementAppliction != null && await ReplacementAppliction.SaveAsync())
                    {
                        ClsLicenses NewLicense = await CreateNewLicenseAsync(IssueReason, CreatedByUserID, ReplacementAppliction.ApplicationID);
                        // Deactivate the old license before saving the replacement.
                        if (await this.DeactivateCurrentLicenseAsync())
                        {
                            if (await NewLicense.SaveAsync())
                            {
                                if (await ReplacementAppliction.SetCompleteAsync())
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

        public async Task<int> DetainAsync(float FineFees, int CreatedByUserID)
        {
            if (!this.IsDetained)
            {
                ClsDetainedLicenses DetainedLicense = new ClsDetainedLicenses();
                DetainedLicense.LicenseID = this.LicenseID;
                DetainedLicense.DetainDate = DateTime.Now;
                DetainedLicense.FineFees = Convert.ToSingle(FineFees);
                DetainedLicense.CreatedByUserID = CreatedByUserID;

                if (await DetainedLicense.SaveAsync())
                {
                    return DetainedLicense.DetainID;
                }
                else { return -1; }
            }
            else
            { return -1; }
        }

        private async Task<ClsApplications> CreateReleaseApplicationAsync(int ReleasedByUserID)
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
                    PaidFees = Convert.ToSingle((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense))?.Fees ?? -1),
                    CreatedByUserID = ReleasedByUserID,
                    ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                };
            }
            return null;
        }

        public async Task<bool> ReleaseAsync(int ReleasedByUserID)
        {
            if (this.IsDetained)
            {
                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    ClsApplications ReleaseApplications = await CreateReleaseApplicationAsync(ReleasedByUserID);

                    if (ReleaseApplications != null && await ReleaseApplications.SaveAsync())
                    {
                        ClsDetainedLicenses DetainedLicense = await ClsDetainedLicenses.FindByLicenseIDAsync(this.LicenseID);
                        if (DetainedLicense != null)
                        {
                            if (await DetainedLicense.ReleaseAsync(ReleasedByUserID, ReleaseApplications.ApplicationID))
                            {
                                if (await ReleaseApplications.SetCompleteAsync())
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