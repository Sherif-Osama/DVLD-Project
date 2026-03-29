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

        public IssueDriverLicenseFirstTime(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();
            LocalLicenseApplication = ClsLocalDrivingLicenseApplication.Find(LocalDrivingLicenseApplicationID);
        }

        private void btnIssueLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to issue license", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (LocalLicenseApplication != null)
                {
                    string Notes = richTextBox1.Text.Trim();

                    if (LocalLicenseApplication.IssueLicense(Notes, ClsGlobal.CurrentUser.UserID))
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

        private void IssueDriverLicenseFirstTime_Load(object sender, EventArgs e) => ctrlDrivingLicenseApplicationInfo1.SetApplicationData(LocalLicenseApplication.LocalDrivingLicenseApplicationID);

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel this process?\n\n", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
            else
                return;
        }
    }
}