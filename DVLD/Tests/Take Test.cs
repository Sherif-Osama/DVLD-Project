using BusinessLayer;
using DVLD.Global_classes;
using System.Windows.Forms;

namespace DVLD.Tests
{
    public partial class TakeTest : Form
    {
        ClsTestAppointments Appointment;
        ClsTest Test;

        public TakeTest(int ApplicationID, int AppointmentID, ClsTestTypes.EnTestType Type)
        {
            InitializeComponent();
            ctrlSecheduledTest1.Initionlaze(ApplicationID, AppointmentID, Type);
            Appointment = ClsTestAppointments.Find(AppointmentID);
        }

        private void SetTestInfo()
        {
            Test = new ClsTest();
            if (Appointment != null)
            {
                Test.CreatedByUserID = ClsGlobal.CurrentUser.UserID;
                Test.Notes = txtNotes.Text.Trim();
                Test.TestAppointmentID = Appointment?.TestAppointmentID ?? -1;
                Test.TestResult = rbPass.Checked ? true : false;
            }
            else
            {
                MessageBox.Show("Error: Appointment Information is not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save? After that you cannot change the Pass/Fail results after you save?.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            SetTestInfo();

            if (Test.Save())
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClose_Click(object sender, System.EventArgs e) => this.Close();
    }
}