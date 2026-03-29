using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;

namespace DVLD.User
{
    public partial class Change_Password : Form
    {
        private int UserID;
        ClsUser UserInfo;

        public Change_Password(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
        }

        // Loads user information when the form is opened
        private void Change_Password_Load(object sender, EventArgs e)
        {
            UserInfo = ClsUser.Find(UserID);

            if (UserInfo != null)
            { ctrlLogInInformationcs1.LoadUserInfo(UserInfo.UserID); }
            else
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Could not Find User with id = " + UserID,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                this.Close();
            }
        }

        #region Save/validation Password
        // Checks whether the new password and confirmation password match
        private bool IsMatchPasswords()
        {
            if (txtConfirmPassword.Text.Trim() == txtNewPassword.Text.Trim())
            { return true; }
            else
            { errorProvider1.SetError(txtConfirmPassword, "The password confirmation must match the new password."); }

            return false;
        }

        private bool Validations() => ClsValidation.ValidateEmptyTextBoxes(errorProvider1, this) && IsMatchPasswords() && UserInfo != null;

        // Triggered when the user clicks the "Save" button
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate input fields and password confirmation before proceeding
            if (!Validations())
            { return; }

            if (UserInfo.Password != HashHelper.ComputeHashing(txtCurrentPassword.Text.Trim()))
            { { errorProvider1.SetError(txtCurrentPassword, "Incorrect password"); return; } }

            // Save changes to the database
            if (UserInfo.ChangePassword(txtNewPassword.Text.Trim()))
            {
                MessageBox.Show("Saved successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
            }
            else
            { MessageBox.Show("An error occurred while saving. Try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        #endregion

        #region TextBox Events
        // Clears validation error messages when the text changes
        private void txtCurrentPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtCurrentPassword, "");
        private void txtNewPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtNewPassword, "");
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtConfirmPassword, "");
        #endregion

        // Closes the form when the "Close" button is clicked
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}