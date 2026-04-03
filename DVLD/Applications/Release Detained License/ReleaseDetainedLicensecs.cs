using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.Local_Licenses.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Release_Detained_License
{
    public partial class ReleaseDetainedLicensecs : Form
    {
        ClsLicenses CurrentLicense;
        private int LicenseID;

        public ReleaseDetainedLicensecs()
        {
            InitializeComponent();
        }

        public ReleaseDetainedLicensecs(int LicenseID)
        {
            InitializeComponent();
            this.LicenseID = LicenseID;
            LoadLicenseInfo();
        }

        private async void LoadLicenseInfo()
        {
            await ctrlDriverLicenseInfoWithFilter1.LoadLicenseInfoAsync(LicenseID);
        }

        private async void ReleaseDetainedLicensecs_Load(object sender, EventArgs e)
        {
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName;

            CurrentLicense = await ClsLicenses.FindAsync(LicenseID);
            await LoadApplicationDataAsync();
        }

        private async Task LoadApplicationDataAsync()
        {
            if (CurrentLicense != null)
            {
                ClsDetainedLicenses DetainedLicense = await ClsDetainedLicenses.FindByLicenseIDAsync(CurrentLicense.LicenseID);

                if (DetainedLicense != null)
                {
                    llShowLicenseHistory.Enabled = true;
                    llShowLicenseInfo.Enabled = true;
                    btnRelease.Enabled = true;
                    lblDetainID.Text = DetainedLicense.DetainID.ToString();
                    lblLicenseID.Text = DetainedLicense.LicenseID.ToString();
                    lblFineFees.Text = DetainedLicense.FineFees.ToString();
                    lblDetainDate.Text = DetainedLicense.DetainDate.ToString("dd/MMM/yyyy");
                    lblApplicationFees.Text = (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense))?.Fees.ToString() ?? "Unknown";
                    lblTotalFees.Text = (DetainedLicense.FineFees + (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.ReleaseDetainedDrivingLicense))?.Fees ?? 0).ToString();
                }
            }
        }

        private async void btnRelease_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to release this detained  license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (await CurrentLicense.ReleaseAsync(ClsGlobal.CurrentUser.UserID))
                {
                    btnRelease.Enabled = false;
                    lblApplicationID.Text = (await ClsDetainedLicenses.FindByLicenseIDAsync(CurrentLicense.LicenseID))?.ReleaseApplicationID.ToString() ?? "Unknown";
                    MessageBox.Show("Detained License released Successfully ", "Detained License Released", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Faild to to release the Detain License", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentLicense != null)
            {
                ShowLocalLicenseInfo LicenseInfo = new ShowLocalLicenseInfo(CurrentLicense.LicenseID);
                LicenseInfo.ShowDialog();
            }
        }

        private async void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentLicense != null)
            {
                ClsDriver Driver = await ClsDriver.FindAsync(CurrentLicense.DriverID);

                if (Driver != null)
                {
                    ShowPersonLicenseHistory ShowPersonLicense = new ShowPersonLicenseHistory(Driver.PersonID);
                    ShowPersonLicense.ShowDialog();
                }
            }
        }

        private async void ctrlDriverLicenseInfoWithFilter1_LicenseSearchCompleted(object sender, ctrlDriverLicenseInfoWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                CurrentLicense = await ClsLicenses.FindAsync(e.LicenseID);
                if (CurrentLicense != null)
                {
                    if (CurrentLicense.IsDetained)
                    {
                        await LoadApplicationDataAsync();
                    }
                    else
                    {
                        btnRelease.Enabled = false;
                        MessageBox.Show("Selected License i is not detained, choose another one.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                llShowLicenseHistory.Enabled = false;
                llShowLicenseInfo.Enabled = false;
                btnRelease.Enabled = false;
                MessageBox.Show("No license found with the provided information.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
    }
}