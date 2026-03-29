using BusinessLayer;
using System.Windows.Forms;

namespace DVLD.License.Local_Licenses.Controls
{
    public partial class ctrlDriverLicenseInfo : UserControl
    {
        public ctrlDriverLicenseInfo()
        {
            InitializeComponent();
        }

        public void LoadLicenseInfo(int LicenseID)
        {
            ClsLicenses License = ClsLicenses.Find(LicenseID);

            if (License != null)
            {
                lblLicenseID.Text = License.LicenseID.ToString();
                lblClass.Text = License.LicenseClassesInfo?.ClassName ?? "Unknown";
                lblIssueDate.Text = License.IssueDate.ToString("dd/MM/yyyy");
                lblIssueReason.Text = License.IssueReasonText;
                lblNotes.Text = string.IsNullOrEmpty(License.Notes) ? "No Notes" : License.Notes;
                lblIsActive.Text = License.IsActive ? "Yes" : "No";
                lblDriverID.Text = License.DriverID.ToString();
                lblExpirationDate.Text = License.ExpirationDate.ToString("dd/MM/yyyy");
                lblIsDetained.Text = License.IsDetained ? "Yes" : "No";

                #region Person Information
                lblFullName.Text = License.DriverInfo?.PersonInfo?.FullName ?? "Unknown";
                lblNationalNo.Text = License.DriverInfo?.PersonInfo?.NationalNo ?? "Unknown";
                lblDateOfBirth.Text = License.DriverInfo?.PersonInfo?.DateOfBirth.ToString("dd/MM/yyyy") ?? "Unknown";
                lblGender.Text = License.DriverInfo?.PersonInfo?.Gender == ClsPerson.EnGender.Female ? "Female" : "Male";

                if (string.IsNullOrEmpty(License.DriverInfo?.PersonInfo?.ImagePath))
                {
                    pbPersonImage.Image = License.DriverInfo?.PersonInfo?.Gender == ClsPerson.EnGender.Female ? Properties.Resources.Female_512 : Properties.Resources.Male_512;
                }
                else
                { pbPersonImage.ImageLocation = License.DriverInfo?.PersonInfo?.ImagePath; }
                #endregion
            }
            else
                MessageBox.Show("License not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}