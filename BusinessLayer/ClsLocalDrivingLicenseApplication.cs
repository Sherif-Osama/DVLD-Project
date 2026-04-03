using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLayer
{
    // Business object for a local driving license application.
    // Inherits shared application behavior from ClsApplications.
    public class ClsLocalDrivingLicenseApplication : ClsApplications
    {
        // Primary key for the local driving license application (DB identity).
        public int LocalDrivingLicenseApplicationID { private set; get; }
        // Selected license class for this application (e.g., class 1,2,3).
        public int LicenseClassID { set; get; }
        public ClsLicenseClasses LicenseClassInfo { get; private set; }

        #region Constructors
        // Default ctor for creating a new application in AddNew mode.
        public ClsLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = -1;
            this.ApplicationType = EnApplicationType.NewDrivingLicense;
            this.ApplicationTypeID = (int)ApplicationType;
        }

        // Private ctor used to build the business object from a DTO loaded from the database.
        private ClsLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationDTO LDLADTO) : base(LDLADTO)
        {
            // Map DTO fields to the business object.
            this.LocalDrivingLicenseApplicationID = LDLADTO.LocalDrivingLicenseApplicationID;
            this.LicenseClassID = LDLADTO.LicenseClassID;
            this.ApplicationID = LDLADTO.ApplicationID;
        }
        #endregion

        // Convert this business object into the DTO used by the data layer.
        private LocalDrivingLicenseApplicationDTO MappingToDTO()
        {
            return new LocalDrivingLicenseApplicationDTO
            {
                LocalDrivingLicenseApplicationID = this.LocalDrivingLicenseApplicationID,
                LicenseClassID = this.LicenseClassID,
                ApplicationID = this.ApplicationID,
            };
        }

        // Retrieve all local driving license applications (DataTable from DAL).
        public static Task<DataTable> GetAllLocalDrivingLicenseApplicationAsync() => ClsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplicationAsync();

        #region Find Methods

        private async Task LoadRelatedDataAsync()
        {
            this.ApplicantPersonInfo = await ClsPerson.FindAsync(this.ApplicantPersonID);
            this.CreatedByUserInfo = await ClsUser.FindAsync(this.CreatedByUserID);
            this.LicenseClassInfo = await ClsLicenseClasses.FindAsync(this.LicenseClassID);
            this.ApplicationTypeInfo = await ClsApplicationsTypes.FindAsync(this.ApplicationTypeID);
        }

        // Find an active application for a person + license class.
        public static async Task<ClsLocalDrivingLicenseApplication> FindActiveApplicationAsync(int PersonID, int LicenseClassID)
        {
            LocalDrivingLicenseApplicationDTO LicenseApplicationDTO = await ClsLocalDrivingLicenseApplicationData.FindActiveApplicationAsync(PersonID, LicenseClassID);

            // Return null if not found.
            if (LicenseApplicationDTO == null) { return null; }

            // Build business object from DTO.
            ClsLocalDrivingLicenseApplication LicenseApplicationObj = new ClsLocalDrivingLicenseApplication(LicenseApplicationDTO);

            await LicenseApplicationObj.LoadRelatedDataAsync();

            return LicenseApplicationObj;
        }

        // Find a local driving license application by its ID.
        public static new async Task<ClsLocalDrivingLicenseApplication> FindAsync(int LocalDrivingLicenseApplicationID)
        {
            LocalDrivingLicenseApplicationDTO LicenseApplicationDTO = await ClsLocalDrivingLicenseApplicationData.FindAsync(LocalDrivingLicenseApplicationID);

            if (LicenseApplicationDTO == null) { return null; }

            ClsLocalDrivingLicenseApplication LicenseApplicationObj = new ClsLocalDrivingLicenseApplication(LicenseApplicationDTO);
            await LicenseApplicationObj.LoadRelatedDataAsync();

            return LicenseApplicationObj;
        }

        // Check whether a person already has an active application for a given license class.
        public static Task<bool> HasActiveApplicationAsync(int PersonID, int LicenseClassID) => ClsLocalDrivingLicenseApplicationData.HasActiveApplicationAsync(PersonID, LicenseClassID);
        #endregion

        #region Addnew/Update/Delete Methods
        // Insert a new local driving license application (returns true on success).
        private async Task<bool> AddNewAsync()
        {
            this.LocalDrivingLicenseApplicationID = await ClsLocalDrivingLicenseApplicationData.AddNewAsync(MappingToDTO());

            // Success if DAL returned a valid ID.
            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        // Update existing record via DAL.
        private Task<bool> UpdateAsync() => ClsLocalDrivingLicenseApplicationData.UpdateAsync(MappingToDTO());

        // Delete the application and its shared application record in a transaction.
        public override async Task<bool> DeleteAsync()
        {
            using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Delete local-specific data first.
                if (!await ClsLocalDrivingLicenseApplicationData.DeleteAsync(this.LocalDrivingLicenseApplicationID))
                { return false; }

                // Then delete shared application fields using base.Delete().
                if (await base.DeleteAsync()) { Scope.Complete(); ; return true; }
                return false;
            }
        }

        // Business rules enforced before saving:
        // - When adding: person must not already have a license or an active application for the same class.
        // - When updating: either there is no other active application, or this object's ApplicationID matches the active application.
        private async Task<bool> BusinessRulesAsync()
        {
            if (Mode == EnMode.AddNew)
            {
                return !await ClsLicenses.IsLicenseExistAsync(this.ApplicantPersonID, this.LicenseClassID) && !await HasActiveApplicationAsync(this.ApplicantPersonID, this.LicenseClassID);
            }
            else if (Mode == EnMode.Update)
            {
                ClsLocalDrivingLicenseApplication ActiveApplication = await FindActiveApplicationAsync(this.ApplicantPersonID, this.LicenseClassID);
                return (ActiveApplication == null || this.ApplicationID == ActiveApplication?.ApplicationID);
            }

            return false;
        }

        // Save entry point: validates business rules, saves shared application fields (base.Save),
        // then inserts or updates local-driving-license-specific data within a transaction.
        public override async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                bool IsNew = (this.Mode == EnMode.AddNew);

                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Persist shared application fields first.
                    if (!await base.SaveAsync())
                        return false;

                    // Then persist local-driving-license-specific data.
                    if (IsNew)
                    {
                        if (await AddNewAsync()) { this.Mode = EnMode.Update; Scope.Complete(); return true; } // Switch to Update mode after successful insert
                        return false;
                    }
                    else
                    {
                        if (await UpdateAsync()) { Scope.Complete(); return true; }
                        return false;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Test Methods
        // Helpers to interact with test-related data for this application.

        // Number of passed tests for this application.
        public Task<int> GetPassedTestsCountAsync() => ClsTestData.GetPassedTestsCountForApplicationAsync(this.LocalDrivingLicenseApplicationID);

        // Number of attempts for a specific test type.
        public Task<byte> TestTrialCountAsync(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.TestTrialCountAsync(this.LocalDrivingLicenseApplicationID, TestTypeID);

        // Whether there's an active appointment of the given test type for this application.
        public Task<bool> HasActiveAppointmentAsync(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.HasActiveAppointmentAsync(TestTypeID, this.LocalDrivingLicenseApplicationID);

        // Whether the applicant attended a specific test type.
        public Task<bool> DoesAttendTestTypeAsync(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.DoesAttendTestTypeAsync(TestTypeID, this.LocalDrivingLicenseApplicationID);

        // Whether the applicant has passed the specified test type.
        public Task<bool> HasPassedTestTypeAsync(int TestTypeID) => ClsTest.HasPassedTestTypeAsync(TestTypeID, this.LocalDrivingLicenseApplicationID);
        #endregion

        #region License Methods
        // Create a new ClsDriver record for the applicant (used during issuing a license).
        private async Task<ClsDriver> CreateDriverAsync(int CreatedByUserID)
        {
            ClsDriver Driver = new ClsDriver();

            Driver.PersonID = this.ApplicantPersonID;
            Driver.CreatedByUserID = CreatedByUserID;
            Driver.CreatedDate = DateTime.Now;

            return await Driver.SaveAsync() ? Driver : null;
        }

        // Create and save a new license (first-time issuance) for an existing driver.
        private async Task<bool> CreateLicenseAsync(int DriverID, string Notes, int CreatedByUserID)
        {
            ClsLicenses NewLicense = new ClsLicenses();

            NewLicense.ApplicationID = this.ApplicationID;
            NewLicense.DriverID = DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            ClsLicenseClasses LicenseClasses = this.LicenseClassInfo ?? await ClsLicenseClasses.FindAsync(this.LicenseClassID);
            NewLicense.ExpirationDate = DateTime.Now.AddYears(LicenseClasses?.DefaultValidityLength ?? 0);
            NewLicense.PaidFees = this.PaidFees;
            NewLicense.IsActive = true;
            NewLicense.CreatedByUserID = CreatedByUserID;
            NewLicense.Notes = Notes;
            NewLicense.IssueReason = ClsLicenses.EnIssueReason.FirstTime;

            return await NewLicense.SaveAsync();
        }

        // Quick check if a license already exists for the person and class.
        public Task<bool> IsLicenseIssuedAsync() => ClsLicenses.IsLicenseExistAsync(this.ApplicantPersonID, this.LicenseClassID);

        // Issue a license when the applicant has passed all required tests (3 passes).
        // Creates a driver record if needed, then creates the license and completes the application inside a transaction.
        public async Task<bool> IssueLicenseAsync(string Notes, int CreatedByUserID)
        {
            if (!await IsLicenseIssuedAsync() && await GetPassedTestsCountAsync() == 3)
            {
                ClsDriver Driver = await ClsDriver.FindByPersonIDAsync(this.ApplicantPersonID);

                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Create driver record if none exists.
                    if (Driver == null)
                        Driver = await CreateDriverAsync(CreatedByUserID);

                    if (Driver == null)
                        return false;

                    // Create the license and mark application complete.
                    if (await CreateLicenseAsync(Driver.DriverID, Notes, CreatedByUserID))
                    {
                        if (await this.SetCompleteAsync())
                        { Scope.Complete(); return true; }
                    }
                    return false;
                }
            }
            return false;
        }
        #endregion
    }
}