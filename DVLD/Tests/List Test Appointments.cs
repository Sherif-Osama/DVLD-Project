using BusinessLayer;
using DVLD.Properties;
using DVLD.Tests.Test_Appointments;
using System;
using System.Windows.Forms;

namespace DVLD.Tests.Test_Types
{
    // Form that displays and manages test appointments for a specific application and test type.
    public partial class ListTestAppointments : Form
    {
        // The application this form is showing appointments for.
        private int ApplicationID;

        // The test type being displayed (Vision/Written/Street).
        private ClsTestTypes.EnTestType TestType;

        // Business object for the local driving license application.
        private ClsLocalDrivingLicenseApplication LocalDrivingLicenseApplication;

        private ClsTestAppointments Appointment;

        #region Initialization Components
        // Constructor: initialize UI and load the application business object.
        public ListTestAppointments(int ApplicationID, ClsTestTypes.EnTestType Type)
        {
            InitializeComponent();
            this.ApplicationID = ApplicationID;
            this.TestType = Type;
            LocalDrivingLicenseApplication = ClsLocalDrivingLicenseApplication.Find(ApplicationID);
        }

        // Form load: setup UI for the selected test type and populate data.
        private void ListTestAppointments_Load(object sender, EventArgs e)
        {
            ctrlDrivingLicenseApplicationInfo1.SetApplicationData(ApplicationID);
            LoadTestTypeUI();
            RefreshDataGridView();
        }

        // Refresh the grid with all appointments for this application and type; format columns if rows exist.
        private void RefreshDataGridView()
        {
            dataGridView1.DataSource = ClsTestAppointments.GetAllApplicationTestAppointments(ApplicationID, (int)TestType);
            int RowsCount = dataGridView1?.Rows?.Count ?? 0;

            if (RowsCount > 0)
            {
                dataGridView1.Columns[0].HeaderText = "ID";
                dataGridView1.Columns[0].Width = 200;
                dataGridView1.Columns[1].HeaderText = "Appointment Date";
                dataGridView1.Columns[1].Width = 250;
                dataGridView1.Columns[2].HeaderText = "Paid Fees";
                dataGridView1.Columns[2].Width = 150;
                dataGridView1.Columns[3].HeaderText = "Is Locked";
                dataGridView1.Columns[3].Width = 150;
                lblRecordsCount.Text = RowsCount.ToString();
            }
        }
        #endregion

        #region Load Test Type UI
        // Set UI for vision test.
        private void LoadVisionTestUI()
        {
            pbTestTypeImage.Image = Resources.Vision_512;
            lblTitle.Text = "Vision Test Appointments";
            this.Text = "Vision Test Appointments";
        }

        // Set UI for written test.
        private void LoadWrittenTestUI()
        {
            pbTestTypeImage.Image = Resources.Written_Test_512;
            lblTitle.Text = "Written Test Appointments";
            this.Text = "Written Test Appointments";
        }

        // Set UI for street (driving) test.
        private void LoadStreetTestUI()
        {
            pbTestTypeImage.Image = Resources.driving_test_512;
            lblTitle.Text = "Street Test Appointments";
            this.Text = "Street Test Appointments";
        }

        // Choose the appropriate UI based on the test type.
        private void LoadTestTypeUI()
        {
            switch (TestType)
            {
                case ClsTestTypes.EnTestType.VisionTest:
                    LoadVisionTestUI();
                    break;
                case ClsTestTypes.EnTestType.WrittenTest:
                    LoadWrittenTestUI();
                    break;
                case ClsTestTypes.EnTestType.StreetTest:
                    LoadStreetTestUI();
                    break;
            }
        }
        #endregion

        private bool Validation()
        {
            // Prevent adding if there is already an active appointment for this test.
            if (LocalDrivingLicenseApplication.HasActiveAppointment((int)TestType))
            {
                MessageBox.Show("Person Already have an active appointment for this test, You cannot add new appointment", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Prevent adding if the person has already passed this test.
            if (LocalDrivingLicenseApplication.HasPassedTestType((int)TestType))
            {
                MessageBox.Show("Person has already passed this test, You cannot add new appointment", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #region Buttons Events
        // Add appointment button click: validate and open scheduler dialog.
        private void AddbutAppointments_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                // Open schedule dialog for creating a new appointment and refresh the grid after.
                ScheduleTest Schedule = new ScheduleTest(ApplicationID, -1, TestType);
                Schedule.ShowDialog();
                RefreshDataGridView();
            }
        }
        #endregion

        // Edit menu item: open scheduler for the selected appointment after validation.
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dataGridView1?.CurrentRow?.Cells[0]?.Value?.ToString(), out int AppointmentID))
            {
                Appointment = ClsTestAppointments.Find(AppointmentID);
                // Disallow editing if the person already passed this test.
                if (Appointment != null && !Appointment.IsLocked)
                {
                    ScheduleTest Schedule = new ScheduleTest(ApplicationID, Appointment.TestAppointmentID, TestType);
                    Schedule.ShowDialog();
                    RefreshDataGridView();
                }
                else
                {
                    MessageBox.Show("This appointment is locked because the test has already been taken. You cannot edit it.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Inform user when selection is invalid.
                MessageBox.Show("Please select a valid appointment to edit.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Try to get the selected appointment ID from the first cell of the current row.
            if (int.TryParse(dataGridView1?.CurrentRow?.Cells[0]?.Value?.ToString(), out int AppointmentID))
            {
                // Load appointment details.
                Appointment = ClsTestAppointments.Find(AppointmentID);

                // Do not allow taking the test if the person already passed this test type.
                if (Appointment != null)
                {
                    // If appointment exists and is not locked, open the TakeTest dialog.
                    if (!Appointment.IsLocked)
                    {
                        TakeTest TakeTast = new TakeTest(ApplicationID, Appointment.TestAppointmentID, TestType);
                        TakeTast.ShowDialog();
                        RefreshDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("This person has already taken the test.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // Inform user when no valid appointment is selected.
                    MessageBox.Show("Please select a valid appointment.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Inform user when no valid appointment is selected.
                MessageBox.Show("Please select a valid appointment.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Close the form.
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}