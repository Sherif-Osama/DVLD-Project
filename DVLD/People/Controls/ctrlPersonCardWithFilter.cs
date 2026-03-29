using BusinessLayer;
using System;
using System.Windows.Forms;

namespace DVLD.Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public class FilterResult : EventArgs
        {
            public int PersonID { get; }
            public bool IsFound { get; }
            public FilterResult(int PersonID, bool IsFound)
            {
                this.PersonID = PersonID;
                this.IsFound = IsFound;
            }
        }

        public event EventHandler<FilterResult> PersonSearchCompleted;

        protected virtual void OnPersonSearchCompleted(int PersonID, bool IsFound)
        {
            PersonSearchCompleted?.Invoke(this, new FilterResult(PersonID, IsFound));
        }

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();

            // Initialize the filter options
            FilterBox.Items.Clear();
            FilterBox.Items.Add("Person ID");
            FilterBox.Items.Add("National No");
            FilterBox.SelectedIndex = 0; // Default selection
        }

        public void LoadPerson(int PersonID)
        {
            ctrlPersonCard1.LoadPerson(PersonID);
            Filtertext.Text = PersonID.ToString();
            groupBox1.Enabled = false;
        }

        public void FilterFocus() => Filtertext.Focus();

        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtertext.Clear();
            Filtertext.KeyPress -= Filtertext_KeyPress;

            // Allow only digits if searching by Person ID
            if (FilterBox.SelectedItem.ToString() == "Person ID")
                Filtertext.KeyPress += Filtertext_KeyPress;
            else
                Filtertext.KeyPress -= Filtertext_KeyPress;
        }

        // Restrict input to digits only when searching by Person ID
        private void Filtertext_KeyPress(object sender, KeyPressEventArgs e)
        {
            Filtertext.MaxLength = 9; // Set max length for Person ID
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            { e.Handled = true; }


            if (e.KeyChar == (char)13)
                Searchbutton.PerformClick();
        }

        // Shows a message when the searched person is not found
        private void ShowNotFoundMessage() => MessageBox.Show("Person not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void HandlePersonNotFound()
        {
            ctrlPersonCard1.Clear();
            OnPersonSearchCompleted(-1, false);
            ShowNotFoundMessage();
        }

        // Main method to perform the search logic
        private void SearchPerson()
        {
            switch (FilterBox.SelectedItem.ToString())
            {
                // Search by Person ID
                case "Person ID":
                    if (int.TryParse(Filtertext.Text, out int PersonID))
                    {
                        if (ClsPerson.PersonExists(PersonID))
                        {
                            // Load the found person into the card
                            ctrlPersonCard1.LoadPerson(PersonID);
                            OnPersonSearchCompleted(PersonID, true);
                        }
                        else
                        // Clear the card and show a "not found" message
                        { HandlePersonNotFound(); }
                    }
                    else
                    { HandlePersonNotFound(); }
                    break;
                // Search by National No
                case "National No":

                    if (!string.IsNullOrWhiteSpace(Filtertext.Text))
                    {
                        // Find the person by national number
                        ClsPerson personInfo = ClsPerson.Find(Filtertext.Text);

                        if (personInfo != null)
                        {
                            // Load the found person into the card
                            ctrlPersonCard1.LoadPerson(personInfo.PersonID);
                            OnPersonSearchCompleted(personInfo.PersonID, true);
                        }
                        else
                        { HandlePersonNotFound(); }
                    }
                    else
                    { HandlePersonNotFound(); }
                    break;
            }
        }

        // Called when the Search button is clicked
        private void Searchbutton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Filtertext.Text))
            { SearchPerson(); }
            else
            { errorProvider1.SetError(Filtertext, "This field is required!"); }
        }

        private void Filtertext_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(Filtertext, "");

        private void Addbutton_Click(object sender, EventArgs e)
        {
            AddAndEditPerson AddNewPerson = new AddAndEditPerson();
            AddNewPerson.ShowDialog();
        }
    }
}
