using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;

namespace DVLD.Tests.Test_Types
{
    public partial class EditTestType : Form
    {
        // Property to store the selected Test Type ID
        private int TestTypeID;

        // Object representing the current Test Type being edited
        ClsTestTypes TestTypes;

        // Constructor that receives the Test Type ID
        public EditTestType(ClsTestTypes.EnTestType TestTypeID)
        {
            InitializeComponent();
            this.TestTypeID = (int)TestTypeID;
        }

        // Event handler triggered when the form is loaded
        private async void EditTestType_Load(object sender, EventArgs e)
        {
            // Load test type data using Business Layer
            TestTypes = await ClsTestTypes.FindAsync(TestTypeID);

            // Display data in form fields if found
            if (TestTypes != null)
            {
                lblTestTypeID.Text = TestTypes.TestTypeID.ToString();
                txtTitle.Text = TestTypes.Title;
                txtDescription.Text = TestTypes.Description;
                txtFees.Text = TestTypes.Fees.ToString();
            }
            else
            {
                MessageBox.Show("Could not find Test Type with id = " + TestTypeID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // Triggered when the Save button is clicked
        private async void btnSave_Click(object sender, EventArgs e)
        {
            // Validate required input fields before saving
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, this))
            {
                // Update object properties from user input
                TestTypes.Title = txtTitle.Text.Trim();
                TestTypes.Description = txtDescription.Text.Trim();
                TestTypes.Fees = float.TryParse(txtFees.Text.ToString(), out float Fees) ? Fees : TestTypes.Fees;

                // Attempt to save updated data
                if (await TestTypes.SaveAsync())
                {
                    MessageBox.Show("Saved successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("An error occurred while saving. Try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear validation error when Fees text changes
        private void txtFees_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtFees, "");

        // Clear validation error when Description text changes
        private void txtDescription_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtDescription, "");

        // Clear validation error when Title text changes
        private void txtTitle_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtTitle, "");

        // Close the form when the Close button is clicked
        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        // Allow only digits and control characters (like Backspace) in the Fees textbox
        private void txtFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            { e.Handled = true; }
        }
    }
}
