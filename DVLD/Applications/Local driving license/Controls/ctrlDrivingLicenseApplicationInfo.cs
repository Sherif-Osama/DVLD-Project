using BusinessLayer;
using DVLD.License;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Local_driving_license
{
    // UserControl that displays local driving license application details.
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        // Business object holding the local driving license application data.
        private ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationData;
        public ctrlDrivingLicenseApplicationInfo() => InitializeComponent();

        #region set Data in controls
        // Load application data by ID and populate UI controls.
        public async Task LoadApplicationDataAsync(int ApplicationID)
        {
            // Retrieve business object from the data layer.
            LocalDrivingLicenseApplicationData = await ClsLocalDrivingLicenseApplication.FindAsync(ApplicationID);

            if (LocalDrivingLicenseApplicationData != null)
            {
                llShowLicenceInfo.Enabled = await LocalDrivingLicenseApplicationData.IsLicenseIssuedAsync();
                lblLocalDrivingLicenseApplicationID.Text = LocalDrivingLicenseApplicationData.LocalDrivingLicenseApplicationID.ToString();
                lblAppliedFor.Text = LocalDrivingLicenseApplicationData.LicenseClassInfo?.ClassName ?? "UnKnown";
                lblPassedTests.Text = (await LocalDrivingLicenseApplicationData.GetPassedTestsCountAsync()).ToString();
                await ctrlApplicationBasicInfo1.SetApplicationBasicInfoAsync(LocalDrivingLicenseApplicationData.LocalDrivingLicenseApplicationID);
            }
        }
        #endregion

        private async void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClsLicenses License = await ClsLicenses.FindByApplicationIDAsync(LocalDrivingLicenseApplicationData.ApplicationID);

            if (License != null)
            {
                ShowLocalLicenseInfo LicenseInfo = new ShowLocalLicenseInfo(License.LicenseID);
                LicenseInfo.ShowDialog();
            }
        }
    }
}