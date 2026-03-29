using DataAccessLayer;
using DTO;
using System.Transactions;

namespace BusinessLayer
{
    // Business object representing a single test result for an appointment.
    // Encapsulates DTO mapping, basic validation and persistence (add/update).
    public class ClsTest
    {
        // Mode indicates whether this instance will be inserted or updated.
        public enum EnMode { AddNew = 1, Update = 2 };

        // Current persistence mode.
        public EnMode Mode { get; private set; }

        // Primary key for the test record.
        public int TestID { get; set; }
        public int TestAppointmentID { get; set; }

        public ClsTestAppointments AppointmentsInfo { get; private set; }

        // The pass/fail result of the test.
        public bool TestResult { get; set; }

        // Optional notes about the test (examiner remarks, etc.).
        public string Notes { get; set; }

        // User who recorded/created this test record.
        public int CreatedByUserID { get; set; }

        #region Constructors
        // Default constructor for a new (unsaved) test record.
        public ClsTest()
        {
            TestID = -1;
            TestAppointmentID = -1;
            TestResult = false;
            Notes = string.Empty;
            CreatedByUserID = -1;
            this.Mode = EnMode.AddNew;
        }

        // Private ctor used when creating a business object from a DTO (loaded from DB).
        private ClsTest(TestDTO TestDTO)
        {
            this.TestID = TestDTO.TestID;
            this.TestAppointmentID = TestDTO.TestAppointmentID;
            AppointmentsInfo = ClsTestAppointments.Find(this.TestAppointmentID);
            this.TestResult = TestDTO.TestResult;
            this.Notes = TestDTO.Notes;
            this.CreatedByUserID = TestDTO.CreatedByUserID;
            this.Mode = EnMode.Update;
        }
        #endregion

        // Map this business object to the DTO expected by the data layer.
        private TestDTO MappingToDTO()
        {
            return new TestDTO
            {
                TestID = this.TestID,
                TestAppointmentID = this.TestAppointmentID,
                TestResult = this.TestResult,
                Notes = this.Notes,
                CreatedByUserID = this.CreatedByUserID
            };
        }

        // Convenience wrappers for common queries in the data layer.
        public static int GetPassedTestsCountForApplication(int LocalDrivingLicenseApplicationID) => ClsTestData.GetPassedTestsCountForApplication(LocalDrivingLicenseApplicationID);

        public static bool HasPassedTestType(int TestTypeID, int LocalDrivingLicenseApplicationID) => ClsTestData.HasPassedTestType(TestTypeID, LocalDrivingLicenseApplicationID);

        // Find a test record by its ID. Returns null if not found.
        public static ClsTest Find(int TestID)
        {
            TestDTO TestDTO = ClsTestData.Find(TestID);

            if (TestDTO == null) { return null; }

            return new ClsTest(TestDTO);
        }

        #region Add new/Update
        // If this test was created as part of a retake flow, mark the retake application completed.
        private bool HandleRetakeApplication()
        {
            if (AppointmentsInfo != null && AppointmentsInfo.RetakeTestApplicationID != -1)
            {
                ClsApplications Applications = ClsApplications.Find(AppointmentsInfo.RetakeTestApplicationID);
                if (Applications != null)
                {
                    if (!Applications.SetComplete())
                        return false;
                }
                else
                    return false;
            }
            return true;
        }

        // Add new test record:
        // - Ensure appointment isn't locked,
        // - Handle retake application completion if needed,
        // - Persist test record and lock appointment inside a transaction.
        private bool AddNew()
        {
            AppointmentsInfo = ClsTestAppointments.Find(this.TestAppointmentID);

            if (!AppointmentsInfo.IsLocked)
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    if (!HandleRetakeApplication())
                        return false;

                    this.TestID = ClsTestData.AddNew(MappingToDTO());
                    if ((this.TestID == -1))
                        return false;

                    if (!AppointmentsInfo.LockAppointment())
                        return false;

                    Scope.Complete();
                    return true;
                }
            }
            return false;
        }

        // Update an existing test record.
        private bool Update() => ClsTestData.Update(MappingToDTO());

        // Public save entrypoint: calls AddNew or Update depending on Mode.
        public bool Save()
        {
            switch (Mode)
            {
                case EnMode.AddNew:
                    if (AddNew()) { Mode = EnMode.Update; return true; }
                    return false;
                case EnMode.Update:
                    return Update();
            }

            return false;
        }
        #endregion
    }
}
