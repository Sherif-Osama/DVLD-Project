using BusinessLayer;
using DVLD.Applications.ApplicationsTypes;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ManageApplicationsTypes : Form
    {
        // Constructor initializes the form
        public ManageApplicationsTypes()
        {
            InitializeComponent();
        }

        // Loads all application types data into the DataGridView
        private void LoadDataGridView()
        {
            dgvApplicationTypes.DataSource = ClsApplicationsTypes.GetAllApplicationsTypes();

            int NumberOfRows = dgvApplicationTypes.Rows.Count;
            lblRecordsCount.Text = NumberOfRows.ToString(); // Displays total records count

            // Adjusts DataGridView columns only if there are records
            if (NumberOfRows > 0)
            {
                dgvApplicationTypes.Columns[0].HeaderText = "ID";
                dgvApplicationTypes.Columns[0].Width = 120;

                dgvApplicationTypes.Columns[1].HeaderText = "Title";
                dgvApplicationTypes.Columns[1].Width = 250;

                dgvApplicationTypes.Columns[2].HeaderText = "Fees";
                dgvApplicationTypes.Columns[2].Width = 103;
            }
        }

        // Triggered when the form loads — populates the DataGridView
        private void ManageApplicationsTypes_Load(object sender, EventArgs e) => LoadDataGridView();

        // Closes the current form when the "Close" button is clicked
        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        // Triggered when user clicks "Edit" option from context menu
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Gets the selected Application Type ID from the DataGridView
            if (int.TryParse(dgvApplicationTypes?.CurrentRow?.Cells[0]?.Value?.ToString(), out int ID))
            {
                // Opens the edit form for the selected Application Type
                EditApplications Edit = new EditApplications(ID);
                Edit.ShowDialog();

                // Refreshes the DataGridView after editing
                LoadDataGridView();
            }
        }
    }
}
