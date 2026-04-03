using System;
using System.Windows.Forms;

namespace DVLD.People
{

    public partial class PersonDetails : Form
    {
        private int PersonID;

        public PersonDetails(int PersonID)
        {
            InitializeComponent();
            this.PersonID = PersonID;
        }

        // Load person information into the embedded user control
        private async void PersonDetails_Load(object sender, EventArgs e) => await userControlInformation1.LoadPersonAsync(PersonID);

        private void Close_Click(object sender, EventArgs e) => this.Close();
    }
}
