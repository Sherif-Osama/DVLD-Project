using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLayer
{
    // Represents a test appointment for a driving test (business layer).
    // Encapsulates data, DTO mapping, validation rules and persistence logic.
    public class ClsTestAppointments
    {
        // Persistence mode: adding a new record or updating an existing one.
        public enum EnMode { AddNew = 1, Update = 2 };
        public EnMode Mode { get; private set; }
        public int TestAppointmentID { get; set; }
        public int TestTypeID { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public float PaidFees { get; set; }

        public int CreatedByUserID { get; set; }

        public bool IsLocked { get; set; }

        // If this appointment is a retake, reference the retake application.
        public int RetakeTestApplicationID { get; set; }

        #region Constructors
        // Default constructor for creating a new appointment (unsaved).
        public ClsTestAppointments()
        {
            TestAppointmentID = -1;
            TestTypeID = -1;
            LocalDrivingLicenseApplicationID = -1;
            AppointmentDate = DateTime.Now;
            PaidFees = 0;
            CreatedByUserID = -1;
            IsLocked = false;
            RetakeTestApplicationID = -1;
            Mode = EnMode.AddNew;
        }

        // Private constructor used when loading from data store (DTO -> business object).
        private ClsTestAppointments(TestAppointmentsDTO TAD)
        {
            TestAppointmentID = TAD.TestAppointmentID;
            TestTypeID = TAD.TestTypeID;
            LocalDrivingLicenseApplicationID = TAD.LocalDrivingLicenseApplicationID;
            AppointmentDate = TAD.AppointmentDate;
            PaidFees = TAD.PaidFees;
            CreatedByUserID = TAD.CreatedByUserID;
            IsLocked = TAD.IsLocked;
            RetakeTestApplicationID = TAD.RetakeTestApplicationID;
            Mode = EnMode.Update;
        }
        #endregion

        // Map this business object to the DTO used by the data layer.
        private TestAppointmentsDTO MappingToDTO()
        {
            return new TestAppointmentsDTO
            {
                TestAppointmentID = this.TestAppointmentID,
                TestTypeID = this.TestTypeID,
                LocalDrivingLicenseApplicationID = this.LocalDrivingLicenseApplicationID,
                AppointmentDate = this.AppointmentDate,
                PaidFees = this.PaidFees,
                CreatedByUserID = this.CreatedByUserID,
                IsLocked = this.IsLocked,
                RetakeTestApplicationID = this.RetakeTestApplicationID
            };
        }

        // Find a specific appointment by its ID (returns null if not found).
        public static async Task<ClsTestAppointments> FindAsync(int AppointmentID)
        {
            TestAppointmentsDTO TAD = await ClsTestAppointmentsData.FindAsync(AppointmentID);
            if (TAD == null) { return null; }
            return new ClsTestAppointments(TAD);
        }

        // Get all appointments for a particular application and test type.
        public static Task<DataTable> GetAllApplicationTestAppointmentsAsync(int ApplicationID, int TestTypeID) => ClsTestAppointmentsData.GetAllApplicationTestAppointmentsAsync(ApplicationID, TestTypeID);
        // Lock this appointment (prevent further updates).
        public Task<bool> LockAppointmentAsync() => ClsTestAppointmentsData.LockAppointmentAsync(this.TestAppointmentID);

        #region Add New/Update/delete/save methods

        private async Task<bool> CreateRetakeApplicationIfNeeded()
        {
            ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(this.LocalDrivingLicenseApplicationID);
            if (LocalDrivingLicenseApplication != null)
            {
                if (await LocalDrivingLicenseApplication.DoesAttendTestTypeAsync((int)this.TestTypeID))
                {
                    ClsApplications Applications = new ClsApplications
                    {
                        ApplicantPersonID = LocalDrivingLicenseApplication.ApplicantPersonID,
                        ApplicationDate = DateTime.Now,
                        ApplicationTypeID = (int)ClsApplications.EnApplicationType.RetakeTest,
                        ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                        LastStatusDate = DateTime.Now,
                        PaidFees = (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.RetakeTest))?.Fees ?? -1,
                        CreatedByUserID = this.CreatedByUserID
                    };

                    if (await Applications.SaveAsync())
                    {
                        this.RetakeTestApplicationID = Applications.ApplicationID;
                        return true;
                    }
                    else { return false; }

                }
                return true;
            }
            return false;
        }

        // Add a new appointment if business rules allow it.
        private async Task<bool> AddNewAsync()
        {
            using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (await CreateRetakeApplicationIfNeeded())
                {
                    this.TestAppointmentID = await ClsTestAppointmentsData.AddNewAsync(MappingToDTO());

                    if (this.TestAppointmentID != -1)
                    {
                        Scope.Complete();
                        return true;
                    }
                }
            }
            return false;
        }

        // Update the appointment if it is not locked.
        private Task<bool> UpdateAsync() => ClsTestAppointmentsData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            if (Mode == EnMode.AddNew)
            {
                bool IsPassTest = await ClsTest.HasPassedTestTypeAsync(this.TestTypeID, this.LocalDrivingLicenseApplicationID);
                ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalDrivingLicenseApplicationID);
                bool HasActiveAppoint = await (LocalDrivingLicenseApplication?.HasActiveAppointmentAsync(this.TestTypeID) ?? Task.FromResult(false));

                return !(IsPassTest || HasActiveAppoint);
            }
            else if (Mode == EnMode.Update)
            { return !this.IsLocked; }

            return false;
        }

        // Save entry point: add or update depending on Mode.
        public async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewAsync()) { Mode = EnMode.Update; return true; }
                        break;
                    case EnMode.Update:
                        return await UpdateAsync();
                }
            }
            return false;
        }
        #endregion
    }
}
