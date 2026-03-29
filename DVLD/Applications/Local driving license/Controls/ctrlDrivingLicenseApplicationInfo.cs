using BusinessLayer;
using DVLD.License;
using System.Windows.Forms;

namespace DVLD.Applications.Local_driving_license
{
    // UserControl that displays local driving license application details.
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        // Business object holding the local driving license application data.
        private ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationData;

        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }
        #region set Data in controls
        // Load application data by ID and populate UI controls.
        public void SetApplicationData(int ApplicationID)
        {
            // Retrieve business object from the data layer.
            LocalDrivingLicenseApplicationData = ClsLocalDrivingLicenseApplication.Find(ApplicationID);

            if (LocalDrivingLicenseApplicationData != null)
            {
                llShowLicenceInfo.Enabled = LocalDrivingLicenseApplicationData.IsLicenseIssued();
                lblLocalDrivingLicenseApplicationID.Text = LocalDrivingLicenseApplicationData.LocalDrivingLicenseApplicationID.ToString();
                lblAppliedFor.Text = LocalDrivingLicenseApplicationData.LicenseClassInfo?.ClassName ?? "UnKnown";
                lblPassedTests.Text = LocalDrivingLicenseApplicationData.GetPassedTestsCount().ToString();
                ctrlApplicationBasicInfo1.SetApplicationBasicInfo(LocalDrivingLicenseApplicationData.LocalDrivingLicenseApplicationID);
            }

        }
        #endregion

        private void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClsLicenses License = ClsLicenses.FindByApplicationID(LocalDrivingLicenseApplicationData.ApplicationID);

            if (License != null)
            {
                ShowLocalLicenseInfo LicenseInfo = new ShowLocalLicenseInfo(License.LicenseID);
                LicenseInfo.ShowDialog();
            }
        }
    }
}