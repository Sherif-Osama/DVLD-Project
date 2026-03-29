using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License.Local_Licenses.Controls;
using System;
using System.Windows.Forms;
namespace DVLD.License.Detain_License
{
    public partial class DetainLicense : Form
    {
        public DetainLicense()
        {
            InitializeComponent();
        }

        ClsLicenses DetainedLicense;

        private void DetainLicenseApplication_Load(object sender, EventArgs e)
        {
            lblDetainDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName;
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void btnDetain_Click(object sender, EventArgs e)
        {
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, this.gpDetain))
            {
                if (MessageBox.Show("Are you sure you want to detain this license?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    float Fees = string.IsNullOrWhiteSpace(txtFineFees.Text.Trim()) ? 0 : Convert.ToSingle(txtFineFees.Text.Trim());

                    int DetainID = DetainedLicense.Detain(Fees, ClsGlobal.CurrentUser.UserID);

                    if (DetainID != -1)
                    {
                        lblDetainID.Text = DetainID.ToString();
                        lblLicenseID.Text = DetainedLicense.LicenseID.ToString();
                        btnDetain.Enabled = false;
                        MessageBox.Show("License Detained Successfully with ID =" + DetainID.ToString(), "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    { MessageBox.Show("Faild to Detain License", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void txtFineFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ctrlDriverLicenseInfoWithFilter1_LicenseSearchCompleted(object sender, ctrlDriverLicenseInfoWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                DetainedLicense = ClsLicenses.Find(e.LicenseID);

                if (DetainedLicense != null)
                {
                    llShowLicenseInfo.Enabled = e.IsFound;
                    llShowLicenseHistory.Enabled = e.IsFound;
                    btnDetain.Enabled = e.IsFound;
                }
                else
                {
                    llShowLicenseHistory.Enabled = false;
                    llShowLicenseInfo.Enabled = false;
                    MessageBox.Show("No license found with the provided information.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                llShowLicenseHistory.Enabled = e.IsFound;
                llShowLicenseInfo.Enabled = e.IsFound;
                MessageBox.Show("No license found with the provided information.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}