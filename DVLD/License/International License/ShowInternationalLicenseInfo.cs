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

        private async void ctrlDriverInternationalLicenseInfo1_Load(object sender, EventArgs e) => await ctrlDriverInternationalLicenseInfo1.LoadInternationalLicenseInfoAsync(LicenseID);

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}