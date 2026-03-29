using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;

namespace DVLD.Applications.ApplicationsTypes
{
    public partial class EditApplications : Form
    {
        // Holds the Application Type ID
        private int ApplicationID;

        // Object to store the application type being edited
        ClsApplicationsTypes ApplicationsTypes;

        public EditApplications(int ApplicationID)
        {
            InitializeComponent();
            this.ApplicationID = ApplicationID;
        }

        #region load application info
        private void EditApplications_Load(object sender, EventArgs e)
        {
            ApplicationsTypes = ClsApplicationsTypes.Find(ApplicationID);

            if (ApplicationsTypes != null)
            {
                lblApplicationTypeID.Text = ApplicationsTypes.ID.ToString();
                txtTitle.Text = ApplicationsTypes.Title;
                txtFees.Text = ApplicationsTypes.Fees.ToString();
            }
            else
            { btnSave.Enabled = false; }
        }
        #endregion

        #region save edited application info
        // Triggered when the user clicks the "Save" button
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all required textboxes have valid values
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, this))
            {
                // Update object properties from input fields
                ApplicationsTypes.Title = txtTitle.Text.Trim();
                ApplicationsTypes.Fees = float.TryParse(txtFees.Text, out float Fees) ? Fees : ApplicationsTypes.Fees;

                // Save the updated data
                if (ApplicationsTypes.Save())
                { MessageBox.Show("Saved successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                else
                    MessageBox.Show("An error occurred while saving. Try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        // Allows only digits and control characters (e.g., Backspace) in the Fees textbox
        private void txtFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtFees.MaxLength = 9; // Limit input to 9 characters (e.g., 999999999)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            { e.Handled = true; }
        }

        // Clears error message when the Title text changes
        private void txtTitle_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtTitle, "");

        // Closes the form when the Close button is clicked
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}