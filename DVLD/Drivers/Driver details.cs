using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.People;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DVLD.Drivers
{
    public partial class DriversDetails : Form
    {
        public DriversDetails()
        {
            InitializeComponent();
        }

        private async Task RefreshDataGridViewAsync()
        {
            dgvDrivers.DataSource = await ClsDriver.GetAllDriversAsync();

            int RowCount = dgvDrivers.Rows.Count;

            if (RowCount > 0)
            {
                dgvDrivers.Columns[0].HeaderText = "Driver ID";
                dgvDrivers.Columns[0].Width = 90;

                dgvDrivers.Columns[1].HeaderText = "Person ID";
                dgvDrivers.Columns[1].Width = 90;

                dgvDrivers.Columns[2].HeaderText = "National No.";
                dgvDrivers.Columns[2].Width = 110;

                dgvDrivers.Columns[3].HeaderText = "Full Name";
                dgvDrivers.Columns[3].Width = 290;

                dgvDrivers.Columns[4].HeaderText = "Date";
                dgvDrivers.Columns[4].Width = 150;

                dgvDrivers.Columns[5].HeaderText = "Active Licenses";
                dgvDrivers.Columns[5].Width = 110;
            }

            lblRecordsCount.Text = RowCount.ToString();
        }

        private void LoadFilterData()
        {
            if (dgvDrivers != null)
            {
                filterBy1.LoadColumn(dgvDrivers);
            }
        }

        private async void DriversDetails_Load(object sender, System.EventArgs e)
        {

            await RefreshDataGridViewAsync();
            LoadFilterData();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvDrivers.CurrentRow.Cells[1].Value.ToString(), out int personId))
            {
                PersonDetails Person = new PersonDetails(personId);
                Person.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unable to retrieve the Person ID for the selected driver.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvDrivers.CurrentRow.Cells[1].Value.ToString(), out int personID))
            {
                ShowPersonLicenseHistory LicenseHistoryForm = new ShowPersonLicenseHistory(personID);
                LicenseHistoryForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unable to retrieve the Person ID for the selected driver.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e) => this.Close();

        private void filterBy1_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvDrivers.DataSource = e.FilteredView;
            lblRecordsCount.Text = e.FoundRows.ToString();
        }
    }
}