using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;

namespace DVLD.Login
{
    // Simple login form: validates input, authenticates user and opens the main form.
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void CheckRememberMe() => ClsGlobal.RememberUsernameAndPassword(chkRememberMe.Checked, txtPassword.Text.Trim());

        // Handle the login button click
        private void ButLogIn_Click(object sender, EventArgs e)
        {
            // Ensure both username and password fields are not empty
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, splitContainerLogin.Panel2))
            {
                // Try to find the user by username and password (business layer)
                ClsGlobal.CurrentUser = ClsUser.Find(txtUserName.Text.Trim(), txtPassword.Text.Trim());

                // If found and active, open MainForm and hide this login form
                if (ClsGlobal.CurrentUser != null && ClsGlobal.CurrentUser.IsActive)
                {
                    CheckRememberMe();

                    MainForm MainLogin = new MainForm();

                    // When MainForm triggers logout, show this login form again
                    MainLogin.OnLogOut += ShowForm;

                    MainLogin.Show();

                    // Hide login while main form is open
                    this.Hide();
                }
                else // Credentials invalid or user not active
                { MessageBox.Show("Your password or username is wrong"); }
            }
        }

        // Callback to show the login form (used by MainForm on logout)
        private void ShowForm() => this.Show();

        // Close the login form
        private void Close_Login_Form_Click(object sender, EventArgs e) => this.Close();

        // Clear validation error when username text changes
        private void txtUserName_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtUserName, "");

        // Clear validation error when password text changes
        private void txtPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtPassword, "");

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string UserName = string.Empty, Password = string.Empty;

            if (ClsGlobal.GetStoredCredential(ref UserName, ref Password))
            {
                txtUserName.Text = UserName;
                txtPassword.Text = Password;
                chkRememberMe.Checked = true;
            }
            else
                chkRememberMe.Checked = false;
        }
    }
}