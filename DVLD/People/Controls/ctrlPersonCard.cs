using BusinessLayer;
using DVLD.Properties;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.People.Controls
{
    // UserControl for showing a person's details with an option to edit
    public partial class ctrlPersonCard : UserControl
    {
        // Current person ID
        private int PersonID;

        // Holds person info object
        private ClsPerson PersonInfo;

        public ctrlPersonCard()
        {
            InitializeComponent();
        }

        // Load person data by ID
        public async Task LoadPersonAsync(int PersonID)
        {
            PersonInfo = await ClsPerson.FindAsync(PersonID);

            if (PersonInfo == null)
            {
                MessageBox.Show("No Person with PersonID = " + PersonID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetPersonData();
        }
        // Load person data by National number
        public async Task LoadPersonAsync(string NationalNo)
        {
            PersonInfo = await ClsPerson.FindAsync(NationalNo);

            if (PersonInfo == null)
            {
                MessageBox.Show("No Person with NationalNo = " + NationalNo.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetPersonData();
        }

        // Opens the edit form when the edit link is clicked
        private async void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddAndEditPerson EditPerson = new AddAndEditPerson(PersonID);
            // Refresh data after editing
            EditPerson.Databake += async (PersonID) => await LoadPersonAsync(PersonID);
            EditPerson.ShowDialog();
        }

        // Fetch and display person data
        private void SetPersonData()
        {
            if (PersonInfo != null)
            {
                PersonID = PersonInfo.PersonID;
                llEditPersonInfo.Enabled = true;
                lblPersonID.Text = PersonInfo.PersonID.ToString();
                lblFullName.Text = $"{PersonInfo.FirstName} {PersonInfo.SecondName} {PersonInfo.ThirdName} {PersonInfo.LastName}";
                lblAddress.Text = PersonInfo.Address;
                lblCountry.Text = PersonInfo.Country.CountryName;
                lblDateOfBirth.Text = PersonInfo.DateOfBirth.ToShortDateString();
                lblEmail.Text = PersonInfo.Email;
                lblPhone.Text = PersonInfo.Phone;
                lblNationalNo.Text = PersonInfo.NationalNo;
                lblGendor.Text = PersonInfo.Gender == ClsPerson.EnGender.Female ? "Female" : "Male";

                if (!string.IsNullOrEmpty(PersonInfo.ImagePath) && File.Exists(PersonInfo.ImagePath))
                {
                    pbPersonImage.ImageLocation = PersonInfo.ImagePath;
                }
                else
                {
                    if (PersonInfo.Gender == ClsPerson.EnGender.Female)
                    { pbPersonImage.Image = Resources.Female_512; }
                    else
                    { pbPersonImage.Image = Resources.Male_512; }
                }
            }
            else
            {
                Clear();
                MessageBox.Show("No Person with PersonID = " + PersonID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void Clear()
        {
            PersonInfo = null;
            llEditPersonInfo.Enabled = false;
            pbPersonImage.Image = Resources.Male_512;
            lblPersonID.Text = "[????]";
            lblFullName.Text = "[????]";
            lblAddress.Text = "[????]";
            lblCountry.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblEmail.Text = "[????]";
            lblPhone.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblGendor.Text = "[????]";
        }
    }
}