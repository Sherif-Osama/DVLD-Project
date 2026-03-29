using BusinessLayer;
using System;
using System.Windows.Forms;

namespace DVLD.License
{
    public partial class ShowPersonLicenseHistory : Form
    {
        int PersonID;

        public ShowPersonLicenseHistory(int PersonID)
        {
            InitializeComponent();
            this.PersonID = PersonID;
        }

        private void ShowPersonLicenseHistory_Load_1(object sender, EventArgs e)
        {
            ClsPerson PersonInfo = ClsPerson.Find(PersonID);
            if (PersonInfo != null)
            {
                ctrlPersonCardWithFilter1.LoadPerson(PersonInfo.PersonID);

                ClsDriver Driver = ClsDriver.FindByPersonID(PersonID);

                if (Driver != null)
                { ctrlDriverLicenses1.LoadAllDriverLicenses(Driver.DriverID); }
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}