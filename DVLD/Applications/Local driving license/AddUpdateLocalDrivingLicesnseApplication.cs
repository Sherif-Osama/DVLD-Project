using BusinessLayer;
using DVLD.Controls;
using DVLD.Global_classes;
using GlobalClasses;
using System;
using System.Data;
using System.Windows.Forms;
using static BusinessLayer.ClsApplications;

namespace DVLD.Applications.Local_driving_license
{
    public partial class AddUpdateLocalDrivingLicenseApplication : Form
    {
        public delegate void RefreshForm();
        public event RefreshForm RefreshFormData;
        // Application type → always New Driving License (ID = 1)
        ClsApplicationsTypes ApplicationsTypes;
        // Business object for the Local Driving License Application
        ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication;
        // Main ApplicationID used in update mode
        private int LocalDrivingLicenseAppLicationID;
        // Form mode → Add or Update
        private enum EnMode { AddNew = 1, Update = 2 };
        EnMode Mode;

        #region InitializeComponent
        public AddUpdateLocalDrivingLicenseApplication()
        {
            InitializeComponent();

            // Load application types
            ApplicationsTypes = ClsApplicationsTypes.Find((int)ClsApplications.EnApplicationType.NewDrivingLicense);
            // Disable application info tab until needed
            InitializetabApplicationInfoTab(false);
            Mode = EnMode.AddNew;
        }

        public AddUpdateLocalDrivingLicenseApplication(int ApplicationsID) : this()
        {
            // Set form mode depending on passed ID
            LocalDrivingLicenseAppLicationID = ApplicationsID;
            Mode = EnMode.Update;
        }

        #region Form Load Operations
        private void AddUpdateLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
            InitializeCBLicenseClass();

            // Load form depending on mode
            switch (Mode)
            {
                case EnMode.AddNew:
                    AddNewUIFrom();
                    break;

                case EnMode.Update:
                    UpdateUIFromLoadedData();
                    break;
            }
        }
        #region Add New Mode Load Operations
        private void AddNewUIFrom()
        {
            // Create a new business object
            LocalDrivingLicenseApplication = new ClsLocalDrivingLicenseApplication();

            // Select person card filter by default
            ctrlPersonCardWithFilter1.FilterFocus();

            // Reset form UI for new entry
            LabAddUpadate.Text = "New local driving license application";
            lblCreatedByUser.Text = ClsGlobal.CurrentUser.UserName;
            lblApplicationDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            labApplicationStatus.Text = LocalDrivingLicenseApplication.StatusText;
            lblFees.Text = LocalDrivingLicenseApplication.ApplicationTypeInfo?.Fees.ToString() ?? "Unknown";
            // Load license classes
        }
        #endregion
        #region Update Mode Load Operations
        private void UpdateUIFromLoadedData()
        {
            InitializetabApplicationInfoTab(true);
            LoadApplicationData();
        }

        private void LoadApplicationData()
        {
            LabAddUpadate.Text = "Edit local driving license application";
            try
            {
                // Load application by ID
                LocalDrivingLicenseApplication = ClsLocalDrivingLicenseApplication.Find(LocalDrivingLicenseAppLicationID);

                if (LocalDrivingLicenseApplication != null)
                {
                    ctrlPersonCardWithFilter1.LoadPerson(LocalDrivingLicenseApplication.ApplicantPersonID);
                    lblLocalDrivingLicebseApplicationID.Text = LocalDrivingLicenseApplication.ApplicationID.ToString();
                    lblCreatedByUser.Text = LocalDrivingLicenseApplication.CreatedByUserInfo?.UserName ?? "Unknown";
                    lblApplicationDate.Text = LocalDrivingLicenseApplication.ApplicationDate.ToString("dd/MM/yyyy");
                    lblFees.Text = LocalDrivingLicenseApplication.PaidFees.ToString();
                    labApplicationStatus.Text = LocalDrivingLicenseApplication.StatusText;
                    // Select correct license class in combo
                    cbLicenseClass.SelectedIndex = cbLicenseClass.FindString(LocalDrivingLicenseApplication.LicenseClassInfo?.ClassName ?? "Unknown");
                }
            }
            catch (Exception Ex)
            {
                // Log the error and close the form
                ClsLogger.Log(Ex);
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        #endregion
        #endregion

        private void InitializetabApplicationInfoTab(bool IsEnabled)
        {
            tabApplicationInfo.Enabled = IsEnabled;
            Save.Enabled = IsEnabled;
            Nextbutton.Enabled = IsEnabled;
        }

        private void InitializeCBLicenseClass()
        {
            // Load all license classes into combo box
            DataTable dt = ClsLicenseClasses.GetAllLicenseClassName();

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cbLicenseClass.Items.Add(dr["ClassName"]);
                }

                // Default selection
                if (cbLicenseClass.Items.Count > 0)
                {
                    cbLicenseClass.SelectedIndex = 2;
                }
            }
        }
        #endregion
        #region Saving Operations
        private void CBErrorMessageoMessage()
        {
            errorProvider1.SetError(cbLicenseClass, "This person already has an application of the same type.");
            tabControl1.SelectedIndex = 2;
            cbLicenseClass.Focus();
        }

        private bool ApplicationValidation(int LicenseClassID)
        {
            // Prevent creating a license if person already has one with the same class
            if (ClsLicenses.IsLicenseExist(LocalDrivingLicenseApplication.ApplicantPersonID, LicenseClassID))
            {
                errorProvider1.SetError(cbLicenseClass, "This person already has a license of the same type.");
                tabControl1.SelectedIndex = 2;
                cbLicenseClass.Focus();
                return false;
            }

            bool IsActive = ClsLocalDrivingLicenseApplication.HasActiveApplication(LocalDrivingLicenseApplication.ApplicantPersonID, LicenseClassID);

            if (Mode == EnMode.Update)
            {
                if (IsActive && LicenseClassID != LocalDrivingLicenseApplication.LicenseClassID)
                {
                    CBErrorMessageoMessage();
                    return false;
                }
            }

            if (Mode == EnMode.AddNew)
            {
                if (IsActive)
                { CBErrorMessageoMessage(); return false; }
            }
            return true;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            // Get selected license class ID
            string LicenseClassName = cbLicenseClass?.Text ?? string.Empty;

            int LicenseClassID = ClsLicenseClasses.Find(LicenseClassName)?.LicenseClassID ?? -1;

            if (ApplicationValidation(LicenseClassID))
            {
                if (Mode == EnMode.AddNew)
                {
                    // Fill business object with needed data
                    LocalDrivingLicenseApplication.PaidFees = ApplicationsTypes.Fees;
                    LocalDrivingLicenseApplication.LicenseClassID = LicenseClassID;
                    LocalDrivingLicenseApplication.CreatedByUserID = ClsGlobal.CurrentUser.UserID;
                    LocalDrivingLicenseApplication.ApplicationDate = DateTime.Now;
                    LocalDrivingLicenseApplication.ApplicationStatus = EnApplicationStatus.New;
                    LocalDrivingLicenseApplication.LastStatusDate = DateTime.Now;
                }

                if (Mode == EnMode.Update)
                {
                    LocalDrivingLicenseApplication.LicenseClassID = LicenseClassID;
                    LocalDrivingLicenseApplication.LastStatusDate = DateTime.Now;
                }

                // Save data to database
                if (LocalDrivingLicenseApplication.Save())
                {
                    lblLocalDrivingLicebseApplicationID.Text = LocalDrivingLicenseApplication.ApplicationID.ToString();
                    MessageBox.Show("Saved successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Switch to update mode after saving
                    Mode = EnMode.Update;
                    RefreshFormData?.Invoke();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("An error occurred while saving. Try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
        #region Control Events
        private void ctrlPersonCardWithFilter1_PersonSearchCompleted(object sender, ctrlPersonCardWithFilter.FilterResult e)
        {
            if (e.IsFound)
            {
                LocalDrivingLicenseApplication.ApplicantPersonID = e.PersonID;
                Nextbutton.Enabled = true;
            }
        }
        #endregion
        #region Button Click Events
        private void Nextbutton_Click(object sender, EventArgs e)
        {
            // For new applications → person must be selected
            if (Mode == EnMode.AddNew)
            {
                if (LocalDrivingLicenseApplication.ApplicantPersonID == -1)
                {
                    MessageBox.Show("Please select a person first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Switch to next tab
            tabControl1.SelectedIndex = 1;
            InitializetabApplicationInfoTab(true);
        }
        private void butCancel_Click(object sender, EventArgs e) => this.Close();
        #endregion
        private void cbLicenseClass_SelectedIndexChanged(object sender, EventArgs e) => errorProvider1.SetError(cbLicenseClass, "");

    }
}