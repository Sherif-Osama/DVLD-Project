using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.International_License;
using DVLD.License.Local_Licenses.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.International_License
{
    public partial class InternationalLicense : Form
    {
        private ClsLicenses CurrentLicense;

        private ClsInternationalLicenses NewInternationalLicense;

        public InternationalLicense() => InitializeComponent();

        private async void InternationalLicense_Load(object sender, EventArgs e)
        {
            lblApplicationDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            lblIssueDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            lblFees.Text = (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.NewInternationalLicense))?.Fees.ToString() ?? "Unknown";
            lblExpirationDate.Text = DateTime.Now.AddYears(1).ToString("MM/dd/yyyy");
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName ?? "Unknown";
        }

        private async Task LoadAplicationsData()
        {
            NewInternationalLicense = new ClsInternationalLicenses();
            NewInternationalLicense.ExpirationDate = DateTime.Now.AddYears(1);
            NewInternationalLicense.CreatedByUserID = ClsGlobal.CurrentUser.UserID;
            NewInternationalLicense.ApplicationDate = DateTime.Now;
            NewInternationalLicense.IssueDate = DateTime.Now;
            NewInternationalLicense.DriverID = CurrentLicense.DriverID;
            NewInternationalLicense.ApplicantPersonID = CurrentLicense?.DriverInfo?.PersonID ?? -1;
            NewInternationalLicense.IsActive = true;
            NewInternationalLicense.IssueDate = DateTime.Now;
            NewInternationalLicense.LastStatusDate = DateTime.Now;
            NewInternationalLicense.IssuedUsingLocalLicenseID = CurrentLicense.LicenseID;
            NewInternationalLicense.PaidFees = (await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.NewInternationalLicense))?.Fees ?? -1;
        }

        private async void btnIssueLicense_Click(object sender, EventArgs e)
        {
            if (NewInternationalLicense == null)
            {
                await LoadAplicationsData();
                if (await NewInternationalLicense.SaveAsync())
                {
                    lblLocalLicenseID.Text = CurrentLicense?.LicenseID.ToString();
                    lblInternationalLicenseID.Text = NewInternationalLicense?.InternationalLicenseID.ToString();
                    lblApplicationID.Text = NewInternationalLicense?.ApplicationID.ToString();
                    ctrlDriverLicenseInfoWithFilter1.Enabled = false;
                    btnIssueLicense.Enabled = false;
                    MessageBox.Show("International license application has been successfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save the new international license application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnIssueLicense.Enabled = false;
                }
            }
        }


        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (NewInternationalLicense != null)
            {
                ShowInternationalLicenseInfo InternationalLicenseInfo = new ShowInternationalLicenseInfo(NewInternationalLicense.InternationalLicenseID);
                InternationalLicenseInfo.ShowDialog();
            }
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentLicense != null)
            {
                if (CurrentLicense.DriverInfo != null)
                {
                    ShowPersonLicenseHistory LicenseHistory = new ShowPersonLicenseHistory(CurrentLicense.DriverInfo.PersonID);
                    LicenseHistory.ShowDialog();
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
                    if (CurrentLicense.LicenseClassID == 3)
                    {
                        llShowLicenseHistory.Enabled = true;

                        NewInternationalLicense = await ClsInternationalLicenses.GetActiveInternationalLicenseByDriverIDAsync(CurrentLicense.DriverID);
                        if (NewInternationalLicense != null)
                        {
                            lblLocalLicenseID.Text = CurrentLicense?.LicenseID.ToString();
                            lblInternationalLicenseID.Text = NewInternationalLicense?.InternationalLicenseID.ToString();
                            lblApplicationID.Text = NewInternationalLicense?.ApplicationID.ToString();
                            MessageBox.Show("Person already have an active international license with ID = " + NewInternationalLicense.InternationalLicenseID.ToString(), "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            llShowLicenseInfo.Enabled = true;
                            btnIssueLicense.Enabled = false;
                        }
                        else
                        {
                            btnIssueLicense.Enabled = true;
                        }
                    }
                    else
                    {
                        btnIssueLicense.Enabled = false;
                        MessageBox.Show("The selected driver does not have a Class 3 License.", "Invalid License Class", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("License not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnIssueLicense.Enabled = false;
            }
        }
    }
}