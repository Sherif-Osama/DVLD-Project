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

        private void ApplicationsDetails_Load(object sender, EventArgs e) => ctrlDrivingLicenseApplicationInfo1.SetApplicationData(ApplicationID);

        private void btnClose_Click(object sender, EventArgs e) => this.Close();


    }
}
