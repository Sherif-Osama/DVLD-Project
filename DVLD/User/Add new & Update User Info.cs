using BusinessLayer;
using DVLD.Controls;
using DVLD.Global_classes;
using System;
using System.Windows.Forms;
namespace DVLD.User
{
    // Form responsible for adding a new user or updating existing user
    public partial class AddOrUpdateUserForm : Form
    {
        // Delegate and event used to notify parent form to refresh its data
        public delegate void RefreshForm();
        public event RefreshForm RefreshFormData;

        private ClsUser User; // Holds current user object (new or loaded from DB)

        // Determines whether form is used for adding or updating
        private enum EnMode { AddNew = 1, Update = 2 };

        private int UserID { get; set; }   // ID of user when updating
        private int PersonID { get; set; } // Selected person ID
        EnMode Mode; // Current working mode of form

        #region Initialization Components
        // Default constructor → used when adding new user
        public AddOrUpdateUserForm()
        {
            InitializeComponent();              // Initialize UI controls
            Nextbutton.Enabled = false;         // Disable Next until person is selected
            InitializePageLoginInfo(false);     // Disable login tab initially
            tabControl1.SelectedIndex = 0;      // Start with first tab
            Mode = EnMode.AddNew;               // Set form to Add mode
        }

        // Constructor with UserID → used when updating existing user
        public AddOrUpdateUserForm(int UserID) : this()
        {
            this.UserID = UserID;               // Store user ID
            Mode = EnMode.Update;               // Switch to Update mode
        }

        // Enable/Disable login related controls
        void InitializePageLoginInfo(bool Enabled)
        {
            tabPage2.Enabled = Enabled;
            Save.Enabled = Enabled;
        }

        #endregion

        #region Load UI Data
        // Loads existing user data from database when updating
        private void SetUserData()
        {
            User = ClsUser.Find(UserID); // Retrieve user from database

            if (User != null)
            {
                PersonID = User.PersonID;
                ctrlPersonCardWithFilter1.LoadPerson(User.PersonID); // Load person info into control
                txtUserName.Text = User.UserName;
                lblUserID.Text = User.UserID.ToString();
                chkIsActive.Checked = User.IsActive;
            }
            else { Save.Enabled = false; }
        }

        // Executes when form loads
        private void AddUpdataUserInfo_Load(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case EnMode.AddNew:
                    LabAddUpadate.Text = "Add new User";
                    User = new ClsUser();               // Create empty user object
                    ctrlPersonCardWithFilter1.FilterFocus();
                    break;

                case EnMode.Update:
                    LabAddUpadate.Text = "Update User";
                    InitializePageLoginInfo(true);     // Enable login tab
                    txtPassword.Enabled = false;
                    txtPassword.Tag = "Optional";
                    txtConfirmPassword.Enabled = false;
                    txtConfirmPassword.Tag = "Optional";
                    Nextbutton.Enabled = true;
                    SetUserData();                      // Fill controls with data
                    break;
            }
        }
        #endregion

        #region Save/Validation Methods

        // Validates that password and confirm password match
        private bool IsMatchPassword()
        {
            errorProvider1.Clear();

            if (txtConfirmPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                errorProvider1.SetError(txtConfirmPassword, "password confirmation doesn't match password ");
                txtConfirmPassword.Focus();
                return false;
            }
            return true;
        }

        // Ensures username is unique
        private bool UserNameValidation()
        {
            if (Mode == EnMode.AddNew)
            {
                if (ClsUser.UserExists(txtUserName.Text.Trim()))
                {
                    errorProvider1.SetError(txtUserName, "This username is already in use by another user! Enter another username.");
                    return false;
                }
                errorProvider1.SetError(txtUserName, "");
                return true;
            }

            // In update mode → allow same username if it belongs to current user
            if (ClsUser.UserExists(txtUserName.Text.Trim()) &&
                User.UserName != txtUserName.Text.Trim())
            {
                errorProvider1.SetError(txtUserName,
                    "This username is already in use by another user! Enter another username.");
                return false;
            }

            errorProvider1.SetError(txtUserName, "");
            return true;
        }

        // Saves user data after validation
        private void SaveUserInfo()
        {
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, tabPage2)
                && IsMatchPassword()
                && UserNameValidation())
            {
                // Assign UI values to object
                User.UserName = txtUserName.Text.Trim();
                User.Password = txtPassword.Text.Trim();
                User.IsActive = chkIsActive.Checked;
                User.PersonID = PersonID;

                // Save to database
                if (User.Save())
                {
                    MessageBox.Show("Saved successfully", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Mode = EnMode.Update; // Switch to Update mode after first save
                    lblUserID.Text = User.UserID.ToString();
                    LabAddUpadate.Text = "Update User";
                    InitializePageLoginInfo(false);
                    RefreshFormData?.Invoke(); // Notify parent form to refresh list
                }
                else
                {
                    MessageBox.Show("An error occurred while saving. Try again", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Person Control Events
        private void ctrlPersonCardWithFilter1_PersonSearchCompleted(object sender, ctrlPersonCardWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                PersonID = e.PersonID;
                Nextbutton.Enabled = e.IsFound;
            }
        }
        #endregion

        #region Button Events

        // Save button click
        private void Save_Click(object sender, EventArgs e) => SaveUserInfo();

        // Cancel button click → close form
        private void butCancel_Click(object sender, EventArgs e) => this.Close();

        // Next button → validate selected person and move to login tab
        private void Nextbutton_Click(object sender, EventArgs e)
        {
            if (Mode == EnMode.AddNew)
            {
                if (PersonID <= 0)
                {
                    MessageBox.Show("Please select a person first.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ClsUser.IsUser(PersonID))
                {
                    MessageBox.Show("this person is already used in the system.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Nextbutton.Enabled = false;
                    return;
                }
            }

            tabControl1.SelectedIndex = 1; // Move to login tab
            InitializePageLoginInfo(true); // Enable login controls
        }

        #endregion

        #region TextBox Events
        // Clear error when user modifies username
        private void txtUserName_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtUserName, "");

        // Clear error when password changes
        private void txtPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtPassword, "");

        // Clear error when confirm password changes
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(txtConfirmPassword, "");

        #endregion
    }
}