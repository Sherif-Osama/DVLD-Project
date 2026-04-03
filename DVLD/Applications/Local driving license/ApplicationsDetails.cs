using System;
using System.Windows.Forms;

namespace DVLD.Applications.Local_driving_license
{
    public partial class ApplicationsDetails : Form
    {
        int ApplicationID;
        public ApplicationsDetails(int ApplicationID)
        {
            InitializeComponent();
            this.ApplicationID = ApplicationID;
        }

        private async void ApplicationsDetails_Load(object sender, EventArgs e) => await ctrlDrivingLicenseApplicationInfo1.LoadApplicationDataAsync(ApplicationID);

        private void btnClose_Click(object sender, EventArgs e) => this.Close();


    }
}
