using DVLD.Applications.International_License;
using DVLD.Applications.Local_driving_license;
using DVLD.Applications.Release_Detained_License;
using DVLD.Applications.Renew_Local_License;
using DVLD.Applications.Replace_Lost_Or_Damaged_License;
using DVLD.Drivers;
using DVLD.Global_classes;
using DVLD.License.Detain_License;
using DVLD.Tests;
using DVLD.User;
using System;
using System.Windows.Forms;

namespace DVLD
{
    // Main application window. Hosts menu actions and opens modal dialogs for each feature.
    public partial class MainForm : Form
    {
        // Delegate + event used to notify subscribers when the user logs out.
        public delegate void LogoutHandler();
        public event LogoutHandler OnLogOut;

        public MainForm()
        {
            InitializeComponent();
        }

        // Open people management modal dialog.
        private void ManegePeople_Click(object sender, EventArgs e)
        {
            ManagePeopleForm people = new ManagePeopleForm();
            people.ShowDialog();
        }

        // Perform sign-out: clear current user, notify subscribers, and close main window.
        private void SignOut_Click(object sender, EventArgs e)
        {
            ClsGlobal.CurrentUser = null;
            OnLogOut?.Invoke();
            this.Close();
        }

        // Open user management modal dialog.
        private void BManageUser_Click(object sender, EventArgs e)
        {
            Users ManageUserForm = new Users();
            ManageUserForm.ShowDialog();
        }

        // Open change-password dialog for the currently logged-in user.
        private void Change_Password_Click(object sender, EventArgs e)
        {
            Change_Password ChangeCurrentUserPassword = new Change_Password(ClsGlobal.CurrentUser.UserID);
            ChangeCurrentUserPassword.ShowDialog();
        }

        // Show details for the currently logged-in user.
        private void CurrentUserInfo_Click(object sender, EventArgs e)
        {
            UserDetailes CurrentUserInfo = new UserDetailes(ClsGlobal.CurrentUser.UserID);
            CurrentUserInfo.ShowDialog();
        }

        // Open application types management dialog.
        private void applicationsTypesMenuItem1_Click_1(object sender, EventArgs e)
        {
            ManageApplicationsTypes applicationsTypes = new ManageApplicationsTypes();
            applicationsTypes.ShowDialog();
        }

        // Open list of test types.
        private void ManageTestTypes_Click(object sender, EventArgs e)
        {
            ListTestTypes listTest = new ListTestTypes();
            listTest.ShowDialog();
        }

        // Open "add/update local driving license" dialog. -1 indicates "create new".
        private void AddUpdateLocalDrivingLicesnseMenuItem6_Click(object sender, EventArgs e)
        {
            AddUpdateLocalDrivingLicenseApplication AddNEw = new AddUpdateLocalDrivingLicenseApplication();
            AddNEw.ShowDialog();
        }

        // Open list of local driving license applications.
        private void LocaldrivinglicenseListMenuItem9_Click(object sender, EventArgs e)
        {
            ListLocalDrivingLicesnseApplications Applicationslist = new ListLocalDrivingLicesnseApplications();
            Applicationslist.ShowDialog();
        }

        // Open international license application dialog.
        private void InternationalLicense_Click(object sender, EventArgs e)
        {
            InternationalLicense internationalLicense = new InternationalLicense();
            internationalLicense.ShowDialog();
        }

        // Open driver details dialog.
        private void ManageDriver_Click(object sender, EventArgs e)
        {
            DriversDetails DriverDetails = new DriversDetails();
            DriverDetails.ShowDialog();
        }

        // Open list of international license applications.
        private void ManageInternationalLicenseApplication_Click(object sender, EventArgs e)
        {
            ListInternationalLicesnseApplications InternationalLicesnseApplications = new ListInternationalLicesnseApplications();
            InternationalLicesnseApplications.ShowDialog();
        }

        // Open renew local license dialog.
        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenewLocalLicense RenewLocalLicense = new RenewLocalLicense();
            RenewLocalLicense.ShowDialog();
        }

        // Open replacement (lost/damaged) license dialog.
        private void ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceLostOrDamagedLicense ReplaceLostOrDamagedLicense = new ReplaceLostOrDamagedLicense();
            ReplaceLostOrDamagedLicense.ShowDialog();
        }

        // Open list of detained licenses dialog.
        private void ManageDetainedLicensestoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListDetainedLicenses ListDetainedLicenses = new ListDetainedLicenses();
            ListDetainedLicenses.ShowDialog();
        }

        // Open detain license dialog to detain a license and apply fines.
        private void detainLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DetainLicense DetainLicense = new DetainLicense();
            DetainLicense.ShowDialog();
        }

        // Open release detained license dialog.
        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReleaseDetainedLicensecs ReleaseLicense = new ReleaseDetainedLicensecs();
            ReleaseLicense.ShowDialog();
        }

        // Duplicate menu action that also opens release detained license dialog.
        private void releaseDetainedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReleaseDetainedLicensecs ReleaseLicense = new ReleaseDetainedLicensecs();
            ReleaseLicense.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClsGlobal.CurrentUser = null;
            OnLogOut?.Invoke();
        }
    }
}