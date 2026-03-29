using BusinessLayer;
using DVLD.Global_classes;
using DVLD.People;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ManagePeopleForm : Form
    {
        public ManagePeopleForm()
        {
            InitializeComponent();
        }

        // Load event: initializes data grid and filter controls
        private void ManagePeopleForm_Load_1(object sender, EventArgs e)
        {
            RefreshDataGridView();
            LoadFilterData();
        }

        // Load all people data and update DataGridView headers and record count
        private void RefreshDataGridView()
        {
            dgvPeople.DataSource = ClsPerson.GetAllPeople();
            int NumberOfRecords = dgvPeople.Rows.Count;
            RecordsNumber.Text = NumberOfRecords.ToString();

            if (NumberOfRecords > 0)
            {
                dgvPeople.Columns[0].HeaderText = "Person ID";
                dgvPeople.Columns[1].HeaderText = "National No";
                dgvPeople.Columns[2].HeaderText = "First Name";
                dgvPeople.Columns[3].HeaderText = "Second Name";
                dgvPeople.Columns[4].HeaderText = "Third Name";
                dgvPeople.Columns[5].HeaderText = "Last Name";
                dgvPeople.Columns[6].HeaderText = "Date Of Birth";
                dgvPeople.Columns[7].HeaderText = "Gender";
                dgvPeople.Columns[8].HeaderText = "Phone";
                dgvPeople.Columns[9].HeaderText = "Email";
                dgvPeople.Columns[10].HeaderText = "Nationality";
            }
        }

        // Load filter configuration for the DataGridView
        private void LoadFilterData() => filterBy1.LoadColumn(dgvPeople);

        // Open Add/Edit Person form in "Add New" mode
        private void AddNewPerson_Click(object sender, EventArgs e)
        {
            AddAndEditPerson Form = new AddAndEditPerson();
            Form.RefreshFormDataEvent += RefreshDataGridView;
            Form.ShowDialog();
        }

        // Open Add/Edit Person form in "Edit" mode for selected person
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvPeople.CurrentRow.Cells[0].Value.ToString(), out int ID))
            {
                AddAndEditPerson Form = new AddAndEditPerson(ID);
                Form.RefreshFormDataEvent += RefreshDataGridView;
                Form.ShowDialog();
            }
        }

        // Delete selected person with confirmation and refresh grid
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string PersonIDMessage = dgvPeople.CurrentRow.Cells[0].Value.ToString();
            if (MessageBox.Show($"Do you want delete Person with ID {PersonIDMessage}", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (ClsPerson.Delete((int)dgvPeople.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("Deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshDataGridView();
                }
                else
                {
                    MessageBox.Show("Failed", "The operation failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        // Show details of the selected person
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvPeople.CurrentRow.Cells[0].Value.ToString(), out int ID))
            {
                PersonDetails PersonDetaInFo = new PersonDetails(ID);
                PersonDetaInFo.ShowDialog();
                RefreshDataGridView();
            }
        }

        // Close the ManagePeople form
        private void Close_Click(object sender, EventArgs e) => this.Close();

        private void filterBy1_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvPeople.DataSource = e.FilteredView;
            RecordsNumber.Text = e.FoundRows.ToString();
        }
    }
}