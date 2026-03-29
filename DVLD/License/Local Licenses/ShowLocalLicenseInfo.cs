using System.Windows.Forms;

namespace DVLD.License
{
    public partial class ShowLocalLicenseInfo : Form
    {
        int LicenseID;
        public ShowLocalLicenseInfo(int licenseID)
        {
            InitializeComponent();
            this.LicenseID = licenseID;
        }

        private void ShowLicenseInfo_Load(object sender, System.EventArgs e) => ctrlDriverLicenseInfo1.LoadLicenseInfo(this.LicenseID);

        private void btnClose_Click(object sender, System.EventArgs e) => this.Close();
    }
}
