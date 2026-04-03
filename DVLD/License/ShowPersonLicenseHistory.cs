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

        private async void ShowPersonLicenseHistory_Load_1(object sender, EventArgs e)
        {
            ClsPerson PersonInfo = await ClsPerson.FindAsync(PersonID);
            if (PersonInfo != null)
            {
                await ctrlPersonCardWithFilter1.LoadPersonAsync(PersonInfo.PersonID);

                ClsDriver Driver = await ClsDriver.FindByPersonIDAsync(PersonID);

                if (Driver != null)
                { await ctrlDriverLicenses1.LoadAllDriverLicensesAsync(Driver.DriverID); }
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}