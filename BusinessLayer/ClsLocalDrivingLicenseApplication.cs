using DataAccessLayer;
using DTO;
using System;
using System.Data;
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
            this.ApplicationTypeInfo = ClsApplicationsTypes.Find(this.ApplicationTypeID);
        }

        // Private ctor used to build the business object from a DTO loaded from the database.
        private ClsLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationDTO LDLADTO) : base(LDLADTO)
        {
            // Map DTO fields to the business object.
            this.LocalDrivingLicenseApplicationID = LDLADTO.LocalDrivingLicenseApplicationID;
            this.LicenseClassID = LDLADTO.LicenseClassID;
            this.LicenseClassInfo = ClsLicenseClasses.Find(this.LicenseClassID);
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
        public static DataTable GetAllLocalDrivingLicenseApplication() => ClsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplication();

        #region Find Methods
        // Find an active application for a person + license class.
        public static ClsLocalDrivingLicenseApplication FindActiveApplication(int PersonID, int LicenseClassID)
        {
            LocalDrivingLicenseApplicationDTO licenseApplicationDTO = ClsLocalDrivingLicenseApplicationData.FindActiveApplication(PersonID, LicenseClassID);

            // Return null if not found.
            if (licenseApplicationDTO == null) { return null; }

            // Build business object from DTO.
            return new ClsLocalDrivingLicenseApplication(licenseApplicationDTO);
        }

        // Find a local driving license application by its ID.
        public static new ClsLocalDrivingLicenseApplication Find(int LocalDrivingLicenseApplicationID)
        {
            LocalDrivingLicenseApplicationDTO licenseApplicationDTO = ClsLocalDrivingLicenseApplicationData.Find(LocalDrivingLicenseApplicationID);

            if (licenseApplicationDTO == null) { return null; }

            return new ClsLocalDrivingLicenseApplication(licenseApplicationDTO);
        }

        // Check whether a person already has an active application for a given license class.
        public static bool HasActiveApplication(int PersonID, int LicenseClassID) => ClsLocalDrivingLicenseApplicationData.HasActiveApplication(PersonID, LicenseClassID);
        #endregion

        #region Addnew/Update/Delete Methods
        // Insert a new local driving license application (returns true on success).
        private bool AddNew()
        {
            this.LocalDrivingLicenseApplicationID = ClsLocalDrivingLicenseApplicationData.AddNew(MappingToDTO());

            // Success if DAL returned a valid ID.
            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        // Update existing record via DAL.
        private bool Update() => ClsLocalDrivingLicenseApplicationData.Update(MappingToDTO());

        // Delete the application and its shared application record in a transaction.
        public override bool Delete()
        {
            using (TransactionScope Scope = new TransactionScope())
            {
                // Delete local-specific data first.
                if (!ClsLocalDrivingLicenseApplicationData.Delete(this.LocalDrivingLicenseApplicationID))
                { return false; }

                // Then delete shared application fields using base.Delete().
                if (base.Delete()) { Scope.Complete(); ; return true; }

                return false;
            }
        }

        // Business rules enforced before saving:
        // - When adding: person must not already have a license or an active application for the same class.
        // - When updating: either there is no other active application, or this object's ApplicationID matches the active application.
        private bool BusinessRules()
        {
            if (Mode == EnMode.AddNew)
            {
                return !ClsLicenses.IsLicenseExist(this.ApplicantPersonID, this.LicenseClassID) && !HasActiveApplication(this.ApplicantPersonID, this.LicenseClassID);
            }
            else if (Mode == EnMode.Update)
            {
                ClsLocalDrivingLicenseApplication ActiveApplication = FindActiveApplication(this.ApplicantPersonID, this.LicenseClassID);
                return (ActiveApplication == null || this.ApplicationID == ActiveApplication?.ApplicationID);
            }

            return false;
        }

        // Save entry point: validates business rules, saves shared application fields (base.Save),
        // then inserts or updates local-driving-license-specific data within a transaction.
        public override bool Save()
        {
            if (BusinessRules())
            {
                bool IsNew = (this.Mode == EnMode.AddNew);

                using (TransactionScope Scope = new TransactionScope())
                {
                    // Persist shared application fields first.
                    if (!base.Save())
                        return false;

                    // Then persist local-driving-license-specific data.
                    if (IsNew)
                    {
                        if (AddNew()) { this.Mode = EnMode.Update; Scope.Complete(); return true; } // Switch to Update mode after successful insert
                        return false;
                    }
                    else
                    {
                        if (Update()) { Scope.Complete(); return true; }
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
        public int GetPassedTestsCount() => ClsTestData.GetPassedTestsCountForApplication(this.LocalDrivingLicenseApplicationID);

        // Number of attempts for a specific test type.
        public byte TestTrialCount(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.TestTrialCount(this.LocalDrivingLicenseApplicationID, TestTypeID);

        // Whether there's an active appointment of the given test type for this application.
        public bool HasActiveAppointment(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.HasActiveAppointment(TestTypeID, this.LocalDrivingLicenseApplicationID);

        // Whether the applicant attended a specific test type.
        public bool DoesAttendTestType(int TestTypeID) => ClsLocalDrivingLicenseApplicationData.DoesAttendTestType(TestTypeID, this.LocalDrivingLicenseApplicationID);

        // Whether the applicant has passed the specified test type.
        public bool HasPassedTestType(int TestTypeID) => ClsTest.HasPassedTestType(TestTypeID, this.LocalDrivingLicenseApplicationID);
        #endregion

        #region License Methods
        // Create a new ClsDriver record for the applicant (used during issuing a license).
        private bool CreateDriver(ref ClsDriver Driver, int CreatedByUserID)
        {
            Driver = new ClsDriver();

            Driver.PersonID = this.ApplicantPersonID;
            Driver.CreatedByUserID = CreatedByUserID;
            Driver.CreatedDate = DateTime.Now;

            return Driver.Save();
        }

        // Create and save a new license (first-time issuance) for an existing driver.
        private bool CreateLicense(int DriverID, string Notes, int CreatedByUserID)
        {
            ClsLicenses NewLicense = new ClsLicenses();

            NewLicense.ApplicationID = this.ApplicationID;
            NewLicense.DriverID = DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            NewLicense.ExpirationDate = DateTime.Now.AddYears(this.LicenseClassInfo?.DefaultValidityLength ?? 0);
            NewLicense.PaidFees = this.PaidFees;
            NewLicense.IsActive = true;
            NewLicense.CreatedByUserID = CreatedByUserID;
            NewLicense.Notes = Notes;
            NewLicense.IssueReason = ClsLicenses.EnIssueReason.FirstTime;

            return NewLicense.Save();
        }

        // Quick check if a license already exists for the person and class.
        public bool IsLicenseIssued() => ClsLicenses.IsLicenseExist(this.ApplicantPersonID, this.LicenseClassID);

        // Issue a license when the applicant has passed all required tests (3 passes).
        // Creates a driver record if needed, then creates the license and completes the application inside a transaction.
        public bool IssueLicense(string Notes, int CreatedByUserID)
        {
            if (!IsLicenseIssued() && GetPassedTestsCount() == 3)
            {
                ClsDriver Driver = ClsDriver.FindByPersonID(this.ApplicantPersonID);

                using (TransactionScope Scope = new TransactionScope())
                {
                    // Create driver record if none exists.
                    if (Driver == null)
                        if (!CreateDriver(ref Driver, CreatedByUserID)) { return false; }

                    // Create the license and mark application complete.
                    if (CreateLicense(Driver.DriverID, Notes, CreatedByUserID))
                    {
                        if (this.SetComplete())
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