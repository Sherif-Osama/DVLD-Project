using System;
using System.Windows.Forms;

namespace DVLD.License.International_License
{
    public partial class ShowInternationalLicenseInfo : Form
    {
        int LicenseID;

        public ShowInternationalLicenseInfo(int LicenseID)
        {
            InitializeComponent();
            this.LicenseID = LicenseID;
        }

        private void ctrlDriverInternationalLicenseInfo1_Load(object sender, EventArgs e) => ctrlDriverInternationalLicenseInfo1.LoadInternationalLicenseInfo(LicenseID);

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}