using DataAccessLayer;
using DTO;
using System;
using System.Data;
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
        public static ClsTestAppointments Find(int AppointmentID)
        {
            TestAppointmentsDTO TAD = ClsTestAppointmentsData.Find(AppointmentID);
            if (TAD == null) { return null; }
            return new ClsTestAppointments(TAD);
        }

        // Get all appointments for a particular application and test type.
        public static DataTable GetAllApplicationTestAppointments(int ApplicationID, int TestTypeID) => ClsTestAppointmentsData.GetAllApplicationTestAppointments(ApplicationID, TestTypeID);
        // Lock this appointment (prevent further updates).
        public bool LockAppointment() => ClsTestAppointmentsData.LockAppointment(this.TestAppointmentID);

        #region Add New/Update/delete/save methods

        private bool CreateRetakeApplicationIfNeeded()
        {
            ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = ClsLocalDrivingLicenseApplication.Find(this.LocalDrivingLicenseApplicationID);
            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.DoesAttendTestType((int)this.TestTypeID))
                {
                    ClsApplications Applications = new ClsApplications
                    {
                        ApplicantPersonID = LocalDrivingLicenseApplication.ApplicantPersonID,
                        ApplicationDate = DateTime.Now,
                        ApplicationTypeID = (int)ClsApplications.EnApplicationType.RetakeTest,
                        ApplicationStatus = ClsApplications.EnApplicationStatus.New,
                        LastStatusDate = DateTime.Now,
                        PaidFees = ClsApplicationsTypes.Find((int)ClsApplications.EnApplicationType.RetakeTest)?.Fees ?? -1,
                        CreatedByUserID = this.CreatedByUserID
                    };

                    if (Applications.Save())
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
        private bool AddNew()
        {
            using (TransactionScope Scope = new TransactionScope())
            {
                if (CreateRetakeApplicationIfNeeded())
                {
                    this.TestAppointmentID = ClsTestAppointmentsData.AddNew(MappingToDTO());

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
        private bool Update() => ClsTestAppointmentsData.Update(MappingToDTO());

        private bool BusinessRules()
        {
            if (Mode == EnMode.AddNew)
            {
                bool IsPassTest = ClsTest.HasPassedTestType(this.TestTypeID, this.LocalDrivingLicenseApplicationID);
                ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = ClsLocalDrivingLicenseApplication.Find(LocalDrivingLicenseApplicationID);
                bool HasActiveAppoint = LocalDrivingLicenseApplication?.HasActiveAppointment(this.TestTypeID) ?? false;

                return !(IsPassTest || HasActiveAppoint);
            }
            else if (Mode == EnMode.Update)
            { return !this.IsLocked; }

            return false;
        }

        // Save entry point: add or update depending on Mode.
        public bool Save()
        {
            if (BusinessRules())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (AddNew()) { Mode = EnMode.Update; return true; }
                        break;
                    case EnMode.Update:
                        return Update();
                }
            }
            return false;
        }
        #endregion
    }
}
