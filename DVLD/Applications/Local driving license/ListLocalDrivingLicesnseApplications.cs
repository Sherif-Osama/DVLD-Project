using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.Local_Licenses;
using DVLD.Tests.Test_Types;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Local_driving_license
{
    // Form that lists local driving license applications and lets user manage them
    // (add/edit, view details, schedule tests, issue license, cancel/delete).
    public partial class ListLocalDrivingLicesnseApplications : Form
    {
        public ListLocalDrivingLicesnseApplications() => InitializeComponent();

        #region Load & Refresh
        // Form load: populate grid and initialize filter control.
        private async void ListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            await RefreshDataGridViewAsync();
            LoadFilterData();
        }

        // Refresh the DataGridView data source and set column headers / widths.
        private async Task RefreshDataGridViewAsync()
        {
            dgvLocalDrivingLicenseApplications.DataSource = await ClsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplicationAsync();

            dgvLocalDrivingLicenseApplications.Columns[0].HeaderText = "ID";
            dgvLocalDrivingLicenseApplications.Columns[0].Width = 50;

            dgvLocalDrivingLicenseApplications.Columns[1].HeaderText = "Class Name";
            dgvLocalDrivingLicenseApplications.Columns[1].Width = 180;

            dgvLocalDrivingLicenseApplications.Columns[2].HeaderText = "National No";
            dgvLocalDrivingLicenseApplications.Columns[2].Width = 80;

            dgvLocalDrivingLicenseApplications.Columns[3].HeaderText = "Full Name";
            dgvLocalDrivingLicenseApplications.Columns[3].Width = 230;

            dgvLocalDrivingLicenseApplications.Columns[4].HeaderText = "Application Date";
            dgvLocalDrivingLicenseApplications.Columns[4].Width = 120;

            dgvLocalDrivingLicenseApplications.Columns[5].HeaderText = "Passed Tests";
            dgvLocalDrivingLicenseApplications.Columns[5].Width = 50;

            dgvLocalDrivingLicenseApplications.Columns[6].HeaderText = "Status";
            dgvLocalDrivingLicenseApplications.Columns[6].Width = 77;
        }
        #endregion
        #region Filter
        // Load filter control with grid columns.
        private void LoadFilterData() => filterBy1.LoadColumn(dgvLocalDrivingLicenseApplications);

        private void filterBy1_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvLocalDrivingLicenseApplications.DataSource = e.FilteredView;
            lblRecordsCount.Text = e.FoundRows.ToString();
        }
        #endregion

        private async void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            AddUpdateLocalDrivingLicenseApplication AddLocalDriving = new AddUpdateLocalDrivingLicenseApplication();
            AddLocalDriving.RefreshFormData += async () => await RefreshDataGridViewAsync();
            AddLocalDriving.ShowDialog();
        }

        #region Context Menu Opening
        // When context menu opens, enable/disable items based on selected application state
        // (license issued, application status, passed tests, etc.).
        private async void cmsApplications_Opening(object sender, CancelEventArgs e)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int LocalDrivingLicenseID))
            {
                ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalDrivingLicenseID);
                bool LicenseExists = await LocalDrivingLicenseApplication.IsLicenseIssuedAsync();

                showLicenseToolStripMenuItem.Enabled = LicenseExists;
                ScheduleTestsMenue.Enabled = !LicenseExists;
                CancelApplicationToolStripMenuItem.Enabled = (LocalDrivingLicenseApplication.ApplicationStatus == ClsApplications.EnApplicationStatus.New);
                editToolStripMenuItem.Enabled = !LicenseExists && (LocalDrivingLicenseApplication.ApplicationStatus == ClsApplications.EnApplicationStatus.New);
                ScheduleTestsMenue.Enabled = !LicenseExists;
                DeleteApplicationToolStripMenuItem.Enabled = (LocalDrivingLicenseApplication.ApplicationStatus == ClsApplications.EnApplicationStatus.New);

                // Enable issuing license only when all required tests are passed and license not yet issued.
                issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = (await LocalDrivingLicenseApplication.GetPassedTestsCountAsync() == 3) && !LicenseExists;

                bool PassedVisionTest = await LocalDrivingLicenseApplication.HasPassedTestTypeAsync((int)ClsTestTypes.EnTestType.VisionTest);
                bool PassedWrittenTest = await LocalDrivingLicenseApplication.HasPassedTestTypeAsync((int)ClsTestTypes.EnTestType.WrittenTest);
                bool PassedStreetTest = await LocalDrivingLicenseApplication.HasPassedTestTypeAsync((int)ClsTestTypes.EnTestType.StreetTest);

                // Enable scheduling only when some required tests are not passed and application is new.
                ScheduleTestsMenue.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest) && (LocalDrivingLicenseApplication.ApplicationStatus == ClsApplications.EnApplicationStatus.New);

                if (ScheduleTestsMenue.Enabled)
                {
                    // Enable specific test menu items based on previously passed tests.
                    scheduleVisionTestToolStripMenuItem.Enabled = !PassedVisionTest;
                    scheduleWrittenTestToolStripMenuItem.Enabled = PassedVisionTest && !PassedWrittenTest;
                    scheduleStreetTestToolStripMenuItem.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;
                }
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        // Open application details dialog for selected application.
        private async void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0].Value.ToString(), out int AppID))
            {
                ApplicationsDetails detailsForm = new ApplicationsDetails(AppID);
                detailsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Edit selected application: open edit dialog and refresh list.
        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int ID))
            {
                AddUpdateLocalDrivingLicenseApplication Edit = new AddUpdateLocalDrivingLicenseApplication(ID);
                Edit.RefreshFormData += async () => await RefreshDataGridViewAsync();
                Edit.ShowDialog();
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int LocalLicenseID))
                {
                    ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalLicenseID);
                    if (LocalDrivingLicenseApplication != null)
                    {
                        if (await LocalDrivingLicenseApplication.DeleteAsync())
                        {
                            MessageBox.Show("Application deleted Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshDataGridViewAsync();
                            LoadFilterData();
                        }
                        else
                            MessageBox.Show("Could not deleted applicatoin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
                MessageBox.Show("Something went wrong. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private async void CancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel this application?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int LocalLicenseID))
                {
                    ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalLicenseID);
                    if (LocalDrivingLicenseApplication != null)
                    {
                        if (await LocalDrivingLicenseApplication.CancelAsync())
                        {
                            MessageBox.Show("Application Cancelled Successfully.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshDataGridViewAsync();
                            LoadFilterData();
                        }
                        else
                            MessageBox.Show("Could not cancel applicatoin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
                MessageBox.Show("Something went wrong. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region scheduleTest
        // Helper to open the test appointments list for the selected application and test type.
        private async Task ScheduleTest(ClsTestTypes.EnTestType Type)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value.ToString(), out int AplicationID))
            {
                ListTestAppointments listTestAppointments = new ListTestAppointments(AplicationID, Type);
                listTestAppointments.ShowDialog();
                LoadFilterData();
                await RefreshDataGridViewAsync();
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e) => await ScheduleTest(ClsTestTypes.EnTestType.VisionTest);

        // Menu handlers to schedule specific test types and refresh after returning.
        private async void scheduleWrittenTestToolStripMenuItem_Click_1(object sender, EventArgs e) => await ScheduleTest(ClsTestTypes.EnTestType.WrittenTest);

        private async void scheduleStreetTestToolStripMenuItem_Click_1(object sender, EventArgs e) => await ScheduleTest(ClsTestTypes.EnTestType.StreetTest);
        #endregion

        private async void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int LocalLicenseID))
            {
                IssueDriverLicenseFirstTime Issue = new IssueDriverLicenseFirstTime(LocalLicenseID);
                // refresh after issuing license
                Issue.RefreshFormData += async () => await RefreshDataGridViewAsync();
                Issue.ShowDialog();
            }
        }

        private async void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[0]?.Value?.ToString(), out int LocalDrivingLicenseApplicationID))
            {
                ClsLocalDrivingLicenseApplication LocalLicenseApplication = await ClsLocalDrivingLicenseApplication.FindAsync(LocalDrivingLicenseApplicationID);

                if (LocalLicenseApplication != null)
                {
                    ClsLicenses License = await ClsLicenses.FindByApplicationIDAsync(LocalLicenseApplication.ApplicationID);

                    if (License != null)
                    {
                        ShowLocalLicenseInfo ShowLicenseInfo = new ShowLocalLicenseInfo(License.LicenseID);
                        ShowLicenseInfo.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("License not found. Please try again.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications?.CurrentRow?.Cells[2]?.Value?.ToString() is string NationalNo)
            {
                ClsPerson personInfo = await ClsPerson.FindAsync(NationalNo);

                if (personInfo != null)
                {
                    ShowPersonLicenseHistory ShowHistory = new ShowPersonLicenseHistory(personInfo.PersonID);
                    ShowHistory.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Something went wrong. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}