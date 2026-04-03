using BusinessLayer;
using DVLD.Global_classes;
using DVLD.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Tests.Controls
{
    public partial class crlScheduleTest : UserControl
    {
        enum EnMode { Addnew = 1, Update = 2 }
        enum EnCreationMode { FirstTimeSchedule = 1, RetakeTestSchedule = 2 };

        int AppointmentID;
        EnMode Mode;
        EnCreationMode CreationMode;
        ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication;
        ClsTestAppointments TestAppointment;
        private ClsTestTypes TestType;
        private ClsTestTypes.EnTestType Type;
        #region Initialization Components

        public crlScheduleTest()
        {
            InitializeComponent();
        }

        public async Task InitializeControl(int LocalDrivingLicenseApplicationID, int AppointmentID, ClsTestTypes.EnTestType Type)
        {
            this.AppointmentID = AppointmentID;
            this.Mode = AppointmentID == -1 ? EnMode.Addnew : EnMode.Update;
            LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalDrivingLicenseApplicationID);
            this.Type = Type;

            TestType = await ClsTestTypes.FindAsync((int)Type);

            if (TestType != null && await LocalDrivingLicenseApplication.DoesAttendTestTypeAsync((int)Type))
                CreationMode = EnCreationMode.RetakeTestSchedule;
            else
                CreationMode = EnCreationMode.FirstTimeSchedule;

            await LoadUIData();
        }
        #endregion

        #region Load UI Image & Title Data
        private void LoadVisionTestUI()
        {
            pbTestTypeImage.Image = Resources.Vision_512;
            lblTitle.Text = "Schedule Vision Test";
            this.Text = "Vision Test";
        }

        private void LoadWrittenTestUI()
        {
            pbTestTypeImage.Image = Resources.Written_Test_512;
            lblTitle.Text = "Schedule Written Test";
            this.Text = "Written Test";
        }

        private void LoadStreetTestUI()
        {
            pbTestTypeImage.Image = Resources.driving_test_512;
            lblTitle.Text = "Schedule Street Test";
            this.Text = "Street Test";
        }

        private async Task LoadUIData()
        {
            switch (Type)
            {
                case ClsTestTypes.EnTestType.VisionTest:
                    LoadVisionTestUI();
                    break;
                case ClsTestTypes.EnTestType.WrittenTest:
                    LoadWrittenTestUI();
                    break;
                case ClsTestTypes.EnTestType.StreetTest:
                    LoadStreetTestUI();
                    break;
            }
            await LoadAppointmentData();
        }
        #endregion

        private async Task LoadAppointmentData()
        {
            switch (Mode)
            {
                case EnMode.Addnew:
                    TestAppointment = new ClsTestAppointments();
                    await LoadApplicationData();
                    break;
                case EnMode.Update:
                    TestAppointment = await ClsTestAppointments.FindAsync(AppointmentID);
                    if (TestAppointment != null)
                    {
                        dtpTestDate.Value = TestAppointment?.AppointmentDate ?? DateTime.Now;
                        await LoadApplicationData();
                    }
                    else { btnSave.Enabled = false; }
                    break;
            }
        }

        private async Task LoadRetakeTestUI()
        {
            if (CreationMode == EnCreationMode.RetakeTestSchedule)
            {
                lblRetakeTestAppID.Text = TestAppointment.RetakeTestApplicationID == -1 ? "N/A" : TestAppointment.RetakeTestApplicationID.ToString();
                decimal Fees = Convert.ToDecimal((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.RetakeTest))?.Fees ?? 0);
                lblTitle.Text = "Schedule Retake Test";
                gbRetakeTestInfo.Enabled = true;
                lblRetakeAppFees.Text = Fees.ToString();
                lblTotalFees.Text = (Fees + Convert.ToDecimal(TestType?.Fees ?? 0)).ToString();
            }
            else
            { gbRetakeTestInfo.Enabled = false; }
        }

        private async Task LoadApplicationData()
        {
            if (LocalDrivingLicenseApplication != null)
            {
                dtpTestDate.MaxDate = DateTime.Now.AddMonths(1);
                dtpTestDate.MinDate = DateTime.Now;
                lblLocalDrivingLicenseAppID.Text = LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                lblDrivingClass.Text = (await ClsLicenseClasses.FindAsync(LocalDrivingLicenseApplication.LicenseClassID))?.ClassName ?? "UnKnown";
                lblFullName.Text = LocalDrivingLicenseApplication.ApplicantPersonInfo?.FullName ?? "UnKnown";
                lblFees.Text = TestType?.Fees.ToString() ?? "UnKnown";
                lblTrial.Text = (await LocalDrivingLicenseApplication.TestTrialCountAsync((int)Type)).ToString();
                await LoadRetakeTestUI();
            }
            else
            {
                MessageBox.Show("Failed to load application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
            }
        }

        #region Save Appointment Data

        private bool SaveValidation()
        {
            errorProvider1.Clear();
            if (dtpTestDate.Value.Date < DateTime.Now.Date || dtpTestDate.Value.Date > DateTime.Now.AddMonths(1).Date)
            {
                errorProvider1.SetError(dtpTestDate, "Appointment date must be within the next month.");
                return false;
            }
            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveValidation())
            {
                TestAppointment.TestTypeID = TestType.TestTypeID;
                TestAppointment.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID;
                TestAppointment.AppointmentDate = dtpTestDate.Value;
                TestAppointment.PaidFees = Convert.ToSingle(TestType?.Fees ?? -1);
                TestAppointment.CreatedByUserID = ClsGlobal.CurrentUser.UserID;

                if (await TestAppointment.SaveAsync())
                {
                    lblRetakeTestAppID.Text = CreationMode == EnCreationMode.RetakeTestSchedule ? TestAppointment.RetakeTestApplicationID.ToString() : lblRetakeTestAppID.Text;
                    MessageBox.Show("Test appointment saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.ParentForm.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save test appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        #endregion

        private void dtpTestDate_ValueChanged(object sender, EventArgs e) => errorProvider1.Clear();
    }
}