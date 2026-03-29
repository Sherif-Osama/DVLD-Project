using BusinessLayer;
using DVLD.Tests.Test_Types;
using System;
using System.Windows.Forms;

namespace DVLD.Tests
{
    public partial class ListTestTypes : Form
    {
        // Constructor initializes the form
        public ListTestTypes()
        {
            InitializeComponent();
        }

        // Loads all test types from the database into the DataGridView
        private void LoadDataGridView()
        {
            dgvTestTypes.DataSource = ClsTestTypes.GetAllTestType();

            int NumberOfRows = dgvTestTypes.Rows.Count;
            lblRecordsCount.Text = NumberOfRows.ToString(); // Show total number of records

            // Customize column headers and widths if there are records
            if (NumberOfRows > 0)
            {
                dgvTestTypes.Columns[0].HeaderText = "ID";
                dgvTestTypes.Columns[0].Width = 100;

                dgvTestTypes.Columns[1].HeaderText = "Title";
                dgvTestTypes.Columns[1].Width = 150;

                dgvTestTypes.Columns[2].HeaderText = "TestTypeDescription";
                dgvTestTypes.Columns[2].Width = 290;

                dgvTestTypes.Columns[3].HeaderText = "Fees";
                dgvTestTypes.Columns[3].Width = 123;
            }
        }

        // Triggered when the form is loaded — populates the DataGridView
        private void ListTestTypes_Load(object sender, EventArgs e) => LoadDataGridView();

        // Closes the form when the Close button is clicked
        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        // Triggered when the user selects "Edit" from the context menu
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected Test Type ID from the DataGridView
            if (int.TryParse(dgvTestTypes.CurrentRow.Cells[0].Value.ToString(), out int TestTypeID))
            {
                // Open the Edit form for the selected Test Type
                EditTestType Edit = new EditTestType((ClsTestTypes.EnTestType)TestTypeID);
                Edit.ShowDialog();

                // Refresh the DataGridView after editing
                LoadDataGridView();
            }
        }
    }
}
