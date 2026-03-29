using System;
using System.Windows.Forms;

namespace DVLD.People
{

    public partial class PersonDetails : Form
    {

        public PersonDetails(int PersonID)
        {
            InitializeComponent();

            // Load person information into the embedded user control
            userControlInformation1.LoadPerson(PersonID);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
