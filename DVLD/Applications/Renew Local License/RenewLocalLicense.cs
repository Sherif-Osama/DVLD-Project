using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.Local_Licenses.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Renew_Local_License
{
    public partial class RenewLocalLicense : Form
    {
        public RenewLocalLicense()
        {
            InitializeComponent();
        }

        ClsLicenses CurrentLicense;
        ClsLicenses NewLicenses;

        private async void RenewLocalLicense_Load(object sender, EventArgs e)
        {
            lblApplicationDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            lblIssueDate.Text = lblApplicationDate.Text;
            lblExpirationDate.Text = "???";
            lblApplicationFees.Text = (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.RenewDrivingLicense))?.Fees.ToString() ?? "Unknown";
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName;
        }

        private async void btnRenewLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Renew the license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (CurrentLicense != null)
                {
                    string Note = string.IsNullOrEmpty(txtNotes.Text.Trim()) ? string.Empty : txtNotes.Text.Trim();
                    NewLicenses = await CurrentLicense.RenewLicenseAsync(Note, ClsGlobal.CurrentUser.UserID);

                    if (NewLicenses != null)
                    {
                        llShowLicenseInfo.Enabled = true;
                        btnRenewLicense.Enabled = false;
                        MessageBox.Show("License renewed successfully! New Expiration Date: "
                            + NewLicenses.ExpirationDate.ToShortDateString() + " New License ID = " + NewLicenses.LicenseID.ToString()
                            , "Renewal Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadApplicationInfoAsync();
                    }
                    else
                    {
                        llShowLicenseInfo.Enabled = false;
                        MessageBox.Show("Failed to renew the license. Please try again later.", "Renewal Failure",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    btnRenewLicense.Enabled = false;
                    MessageBox.Show("License information is not available. Please search for a valid license before attempting to renew.", "Renewal Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task LoadApplicationInfoAsync()
        {
            if (NewLicenses != null)
            {
                lblRenewedLicenseID.Text = NewLicenses.LicenseID.ToString();
                lblOldLicenseID.Text = CurrentLicense.LicenseID.ToString();
                lblApplicationID.Text = NewLicenses.ApplicationID.ToString();
                lblExpirationDate.Text = NewLicenses.ExpirationDate.ToString("MM/dd/yyyy");
                lblLicenseFees.Text = NewLicenses.LicenseClassesInfo?.ClassFees.ToString() ?? "Unknown";
                lblTotalFees.Text = ((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.RenewDrivingLicense))?.Fees + (NewLicenses.LicenseClassesInfo?.ClassFees ?? 0)).ToString();
            }
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentLicense != null)
            {
                if (CurrentLicense.DriverInfo != null)
                {
                    ShowPersonLicenseHistory showLicenseHistory = new ShowPersonLicenseHistory(CurrentLicense.DriverInfo.PersonID);
                    showLicenseHistory.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Driver information is not available for this license. Cannot display license history.", "License History Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                llShowLicenseHistory.Enabled = false;
                MessageBox.Show("License information is not available. Please search for a valid license to view its history.", "License History Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (NewLicenses != null)
            {
                ShowLocalLicenseInfo NewLicenseInfo = new ShowLocalLicenseInfo(NewLicenses.LicenseID);
                NewLicenseInfo.ShowDialog();
            }
            else
            {
                llShowLicenseInfo.Enabled = false;
                MessageBox.Show("New license information is not available. Please renew the license to view its details.", "License Information Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void ctrlDriverLicenseInfoWithFilter1_LicenseSearchCompleted(object sender, ctrlDriverLicenseInfoWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                CurrentLicense = await ClsLicenses.FindAsync(e.LicenseID);
                if (CurrentLicense != null)
                {
                    llShowLicenseHistory.Enabled = true;

                    if (CurrentLicense.ExpirationDate < DateTime.Now)
                    {
                        btnRenewLicense.Enabled = true;
                    }
                    else
                    {
                        btnRenewLicense.Enabled = false;
                        MessageBox.Show("The license is not eligible for renewal. It must be expired.", "Renewal Eligibility", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    btnRenewLicense.Enabled = false;
                    MessageBox.Show("No license found with the provided information.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void btnClose_Click(object sender, EventArgs e) => this.Close();

    }
}
