using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.Local_Licenses.Controls;
using System;
using System.Windows.Forms;
using static BusinessLayer.ClsLicenses;

namespace DVLD.Applications.Replace_Lost_Or_Damaged_License
{
    // Form to handle replacing a lost or damaged driving license.
    public partial class ReplaceLostOrDamagedLicense : Form
    {
        public ReplaceLostOrDamagedLicense()
        {
            InitializeComponent();
        }

        // Currently selected existing license (to be replaced)
        ClsLicenses CurrentLicense;

        // Newly issued replacement license
        ClsLicenses NewLicense;

        // Initialize UI values on form load (date and current user)
        private void ReplaceLostOrDamagedLicense_Load(object sender, EventArgs e)
        {
            lblApplicationDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName;
        }

        // Show license history for the driver associated with the current license.
        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurrentLicense != null)
            {
                if (CurrentLicense.DriverInfo != null)
                {
                    // Open a dialog showing this person's license history
                    ShowPersonLicenseHistory showLicenseHistory = new ShowPersonLicenseHistory(CurrentLicense.DriverInfo.PersonID);
                    showLicenseHistory.ShowDialog();
                }
                else
                {
                    llShowLicenseHistory.Enabled = false;
                    MessageBox.Show("Driver Does Not Exist", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                llShowLicenseHistory.Enabled = false;
                MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Determine the replacement issue reason based on selected radio button.
        private EnIssueReason GetIssueReason()
        {
            // Return DamagedReplacement when damaged option is checked, otherwise LostReplacement
            if (rbDamagedLicense.Checked)
                return EnIssueReason.DamagedReplacement;
            else
                return EnIssueReason.LostReplacement;
        }

        // Attempt to issue the replacement license when the user clicks the button.
        private async void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Issue a Replacement for the license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (CurrentLicense != null)
                {
                    NewLicense = await CurrentLicense.ReplaceAsync(GetIssueReason(), ClsGlobal.CurrentUser.UserID);

                    if (NewLicense != null)
                    {
                        // Notify success and populate UI with result IDs
                        MessageBox.Show("Licensed Replaced Successfully with ID = " + NewLicense.LicenseID.ToString(), "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        lblApplicationID.Text = NewLicense.ApplicationID.ToString();
                        lblOldLicenseID.Text = CurrentLicense.LicenseID.ToString();
                        lblRreplacedLicenseID.Text = NewLicense.LicenseID.ToString();

                        // Lock parts of the UI to prevent further edits and enable view of new license
                        ctrlDriverLicenseInfoWithFilter1.Enabled = false;
                        btnIssueReplacement.Enabled = false;
                        gbReplacementFor.Enabled = false;
                        llShowLicenseInfo.Enabled = true;
                    }
                    else
                    {
                        // Replacement failed - inform user
                        llShowLicenseInfo.Enabled = false;
                        MessageBox.Show("Faild to Issue a replacemnet for this  License", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // No current license selected - disable actions and inform user
                    btnIssueReplacement.Enabled = false;
                    llShowLicenseHistory.Enabled = false;
                    MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Open a dialog showing the newly issued license details.
        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (NewLicense != null)
            {
                ShowLocalLicenseInfo ShowLocalLicense = new ShowLocalLicenseInfo(NewLicense.LicenseID);
                ShowLocalLicense.ShowDialog();
            }
            else
            {
                llShowLicenseInfo.Enabled = false;
                MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Update displayed application fee when "Damaged" option is selected.
        private async void rbDamagedLicense_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDamagedLicense.Checked)
            {
                // Lookup the application type and show its fees (falls back to "Unknown")
                lblApplicationFees.Text = Convert.ToString((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.ReplaceDamagedDrivingLicense))?.Fees.ToString()) ?? "Unknown";
            }
        }

        // Update displayed application fee when "Lost" option is selected.
        private async void rbLostLicense_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLostLicense.Checked)
            {
                // Lookup the application type and show its fees (falls back to "Unknown")
                lblApplicationFees.Text = Convert.ToString((await ClsApplicationsTypes.FindAsync((int)ClsApplications.EnApplicationType.ReplaceLostDrivingLicense))?.Fees.ToString()) ?? "Unknown";
            }
        }

        private async void ctrlDriverLicenseInfoWithFilter1_LicenseSearchCompleted(object sender, ctrlDriverLicenseInfoWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                CurrentLicense = await ClsLicenses.FindAsync(e.LicenseID);
                if (CurrentLicense != null)
                {
                    if (!CurrentLicense.IsActive)
                    {
                        MessageBox.Show("License is not active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    llShowLicenseHistory.Enabled = true;
                    btnIssueReplacement.Enabled = true;
                }
                else
                {
                    llShowLicenseHistory.Enabled = false;
                    btnIssueReplacement.Enabled = false;
                    MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                llShowLicenseHistory.Enabled = false;
                btnIssueReplacement.Enabled = false;
                MessageBox.Show("License Does Not Exist", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Close the form
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}