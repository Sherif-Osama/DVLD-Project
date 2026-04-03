using DataAccessLayer;
using DTO;
using System.Threading.Tasks;
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
        public static Task<int> GetPassedTestsCountForApplicationAsync(int LocalDrivingLicenseApplicationID) => ClsTestData.GetPassedTestsCountForApplicationAsync(LocalDrivingLicenseApplicationID);

        public static Task<bool> HasPassedTestTypeAsync(int TestTypeID, int LocalDrivingLicenseApplicationID) => ClsTestData.HasPassedTestTypeAsync(TestTypeID, LocalDrivingLicenseApplicationID);

        #region Find Methods
        private async Task LoadRelatedDataAsync()
        {
            AppointmentsInfo = await ClsTestAppointments.FindAsync(this.TestAppointmentID);
        }

        // Find a test record by its ID. Returns null if not found.
        public static async Task<ClsTest> FindAsync(int TestID)
        {
            TestDTO TestDTO = await ClsTestData.FindAsync(TestID);

            if (TestDTO == null) { return null; }

            ClsTest TestObj = new ClsTest(TestDTO);

            await TestObj.LoadRelatedDataAsync();

            return TestObj;
        }
        #endregion

        #region Add new/Update
        // If this test was created as part of a retake flow, mark the retake application completed.
        private async Task<bool> HandleRetakeApplication()
        {
            if (AppointmentsInfo != null && AppointmentsInfo.RetakeTestApplicationID != -1)
            {
                ClsApplications Applications = await ClsApplications.FindAsync(AppointmentsInfo.RetakeTestApplicationID);

                if (Applications != null)
                {
                    if (!await Applications.SetCompleteAsync())
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
        private async Task<bool> AddNewAsync()
        {
            AppointmentsInfo = await ClsTestAppointments.FindAsync(this.TestAppointmentID);

            if (!AppointmentsInfo.IsLocked)
            {
                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (!await HandleRetakeApplication())
                        return false;

                    this.TestID = await ClsTestData.AddNewAsync(MappingToDTO());
                    if ((this.TestID == -1))
                        return false;

                    if (!await AppointmentsInfo.LockAppointmentAsync())
                        return false;

                    Scope.Complete();
                    return true;
                }
            }
            return false;
        }

        // Update an existing test record.
        private Task<bool> UpdateAsync() => ClsTestData.UpdateAsync(MappingToDTO());

        // Public save entrypoint: calls AddNew or Update depending on Mode.
        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case EnMode.AddNew:
                    if (await AddNewAsync()) { Mode = EnMode.Update; return true; }
                    return false;
                case EnMode.Update:
                    return await UpdateAsync();
            }

            return false;
        }
        #endregion
    }
}