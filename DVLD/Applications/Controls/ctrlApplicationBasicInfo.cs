using BusinessLayer;
using DVLD.People;
using System.Windows.Forms;

namespace DVLD.Applications.Controls
{
    // UserControl that shows basic information for a driving license application.
    public partial class ctrlApplicationBasicInfo : UserControl
    {
        // Business object containing the current application's data.
        ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationInfo;

        public ctrlApplicationBasicInfo()
        {
            InitializeComponent();
        }

        // Load application data by ID and populate UI labels with simple formatted values.
        public void SetApplicationBasicInfo(int ApplicationID)
        {
            LocalDrivingLicenseApplicationInfo = ClsLocalDrivingLicenseApplication.Find(ApplicationID);
            if (LocalDrivingLicenseApplicationInfo != null)
            {

                lblApplicationID.Text = LocalDrivingLicenseApplicationInfo.ApplicationID.ToString();
                lblStatus.Text = LocalDrivingLicenseApplicationInfo.StatusText;
                lblFees.Text = LocalDrivingLicenseApplicationInfo.PaidFees.ToString();
                // Show application type title or "Unknown" when not found.
                lblType.Text = LocalDrivingLicenseApplicationInfo.ApplicationTypeInfo?.Title ?? "Unknown";
                // Show applicant full name or "Unknown" when not found.
                lblApplicant.Text = LocalDrivingLicenseApplicationInfo.ApplicantPersonInfo?.FullName ?? "Unknown";
                lblStatusDate.Text = LocalDrivingLicenseApplicationInfo.LastStatusDate.ToString("dd/MM/yyyy");
                lblDate.Text = LocalDrivingLicenseApplicationInfo.ApplicationDate.ToString("dd/MM/yyyy");
                // Show the user who created the application or "Unknown" when not found.
                lblCreatedByUser.Text = LocalDrivingLicenseApplicationInfo.CreatedByUserInfo?.UserName ?? "Unknown";
            }
        }

        // Open a dialog showing detailed person information for the applicant.
        private void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PersonDetails PersonDetails = new PersonDetails(LocalDrivingLicenseApplicationInfo.ApplicantPersonID);
            PersonDetails.ShowDialog();
        }
    }
}