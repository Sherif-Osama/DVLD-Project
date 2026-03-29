using BusinessLayer;
using DVLD.Properties;
using System.Windows.Forms;

namespace DVLD.Tests.Controls
{
    public partial class ctrlSecheduledTest : UserControl
    {
        int ApplicationID;
        ClsTestTypes.EnTestType Type;
        ClsTestAppointments AppointmentsInfo;
        ClsLocalDrivingLicenseApplication Application;
        public ctrlSecheduledTest()
        {
            InitializeComponent();
        }

        public void Initionlaze(int LocalDrivingLicenseAppID, int AppointmentID, ClsTestTypes.EnTestType Type)
        {
            this.ApplicationID = LocalDrivingLicenseAppID;
            this.Type = Type;

            AppointmentsInfo = ClsTestAppointments.Find(AppointmentID);
            Application = ClsLocalDrivingLicenseApplication.Find(LocalDrivingLicenseAppID);
            SetDada();
        }

        #region Load UI Image & Title Data
        private void SetDada()
        {
            LoadUIData();
            if (AppointmentsInfo != null && Application != null)
            {
                lblLocalDrivingLicenseAppID.Text = this.ApplicationID.ToString();
                lblDrivingClass.Text = Application?.LicenseClassInfo?.ClassName ?? "Unknown";
                lblFullName.Text = Application?.ApplicantPersonInfo?.FullName ?? "UnKnown";
                lblTrial.Text = Application?.TestTrialCount((int)Type).ToString() ?? "UnKnown";
                lblDate.Text = Application.ApplicationDate.ToString("dd/MM/yyyy");
                lblFees.Text = ClsTestTypes.Find((int)Type)?.Fees.ToString() ?? "UnKnown";
                lblTestID.Text = "Not taken yet";
            }
        }

        private void LoadVisionTestUI()
        {
            pbTestTypeImage.Image = Resources.Vision_512;
            lblTitle.Text = "Schedule Vision Test";
        }

        private void LoadWrittenTestUI()
        {
            pbTestTypeImage.Image = Resources.Written_Test_512;
            lblTitle.Text = "Schedule Written Test";
        }

        private void LoadStreetTestUI()
        {
            pbTestTypeImage.Image = Resources.driving_test_512;
            lblTitle.Text = "Schedule Street Test";
        }

        private void LoadUIData()
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
        }
        #endregion
    }
}