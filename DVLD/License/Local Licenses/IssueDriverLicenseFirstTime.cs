using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;

namespace DVLD.License.Local_Licenses
{
    public partial class IssueDriverLicenseFirstTime : Form
    {
        public delegate void RefreshForm();
        public event RefreshForm RefreshFormData;

        private ClsLocalDrivingLicenseApplication LocalLicenseApplication;
        int LocalDrivingLicenseApplicationID;
        public IssueDriverLicenseFirstTime(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
        }

        private async void IssueDriverLicenseFirstTime_Load(object sender, EventArgs e)
        {
            LocalLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalDrivingLicenseApplicationID);
            await ctrlDrivingLicenseApplicationInfo1.LoadApplicationDataAsync(LocalLicenseApplication.LocalDrivingLicenseApplicationID);
        }

        private async void btnIssueLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to issue license", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (LocalLicenseApplication != null)
                {
                    string Notes = richTextBox1.Text.Trim();

                    if (await LocalLicenseApplication.IssueLicenseAsync(Notes, ClsGlobal.CurrentUser.UserID))
                    {
                        MessageBox.Show("License issued successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshFormData?.Invoke();

                    }
                    else
                        MessageBox.Show("An error occurred while issuing the license, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("An error occurred, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel this process?\n\n", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
            else
                return;
        }
    }
}