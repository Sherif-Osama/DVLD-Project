using BusinessLayer;
using System;
using System.Windows.Forms;

namespace DVLD.Tests.Test_Appointments
{
    public partial class ScheduleTest : Form
    {
        ClsTestTypes.EnTestType Type;
        int LocalDrivingLicenseApplicationID;
        int AppointmentID;

        #region Initialization Components
        public ScheduleTest(int LocalDrivingLicenseApplicationID, int AppointmentID, ClsTestTypes.EnTestType Type)
        {
            InitializeComponent();
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.Type = Type;
            this.AppointmentID = AppointmentID;
        }

        private async void ScheduleTest_Load_1(object sender, EventArgs e) => await crlScheduleTest1.InitializeControl(LocalDrivingLicenseApplicationID, AppointmentID, Type);

        #endregion
    }
}
