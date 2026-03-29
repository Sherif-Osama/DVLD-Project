using BusinessLayer;
using System.Windows.Forms;

namespace DVLD.License.International_License
{
    public partial class ctrlDriverInternationalLicenseInfo : UserControl
    {
        public ctrlDriverInternationalLicenseInfo()
        {
            InitializeComponent();
        }

        public void LoadInternationalLicenseInfo(int LicenseID)
        {
            ClsInternationalLicenses InternationalLicense = ClsInternationalLicenses.Find(LicenseID);
            if (InternationalLicense != null)
            {
                ClsPerson personInfo = ClsPerson.Find(InternationalLicense.ApplicantPersonID);
                if (personInfo != null)
                {
                    lblFullName.Text = personInfo.FullName;
                    lblNationalNo.Text = personInfo.NationalNo;
                    lblGender.Text = personInfo.Gender == 0 ? "Male" : "Female";
                    lblDateOfBirth.Text = personInfo.DateOfBirth.ToString("MM/dd/yyyy");
                }


                lblInternationalLicenseID.Text = InternationalLicense.InternationalLicenseID.ToString();
                lblLocalLicenseID.Text = InternationalLicense.IssuedUsingLocalLicenseID.ToString();
                lblApplicationID.Text = InternationalLicense.ApplicationID.ToString();
                lblDriverID.Text = InternationalLicense.DriverID.ToString();
                lblIssueDate.Text = InternationalLicense.IssueDate.ToString("MM/dd/yyyy");
                lblExpirationDate.Text = InternationalLicense.ExpirationDate.ToString("MM/dd/yyyy");
                lblIsActive.Text = InternationalLicense.IsActive ? "Yes" : "No";
            }
        }
    }
}