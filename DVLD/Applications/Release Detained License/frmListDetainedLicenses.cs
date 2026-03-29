using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.Detain_License;
using DVLD.People;
using System.Windows.Forms;
namespace DVLD.Applications.Release_Detained_License
{
    public partial class ListDetainedLicenses : Form
    {
        public ListDetainedLicenses()
        {
            InitializeComponent();
        }

        private void LoadDGVInfo()
        {
            dgvDetainedLicenses.DataSource = ClsDetainedLicenses.GetAllDetainedLicenses();

            int RowNum = dgvDetainedLicenses.Rows.Count;

            lblTotalRecords.Text = RowNum.ToString();

            if (RowNum > 0)
            {
                dgvDetainedLicenses.Columns[0].HeaderText = "D.ID";
                dgvDetainedLicenses.Columns[0].Width = 90;

                dgvDetainedLicenses.Columns[1].HeaderText = "L.ID";
                dgvDetainedLicenses.Columns[1].Width = 90;

                dgvDetainedLicenses.Columns[2].HeaderText = "D.Date";
                dgvDetainedLicenses.Columns[2].Width = 130;

                dgvDetainedLicenses.Columns[3].HeaderText = "Is Released";
                dgvDetainedLicenses.Columns[3].Width = 100;

                dgvDetainedLicenses.Columns[4].HeaderText = "Fine Fees";
                dgvDetainedLicenses.Columns[4].Width = 100;

                dgvDetainedLicenses.Columns[5].HeaderText = "Release Date";
                dgvDetainedLicenses.Columns[5].Width = 130;

                dgvDetainedLicenses.Columns[6].HeaderText = "N.No.";
                dgvDetainedLicenses.Columns[6].Width = 90;

                dgvDetainedLicenses.Columns[7].HeaderText = "Full Name";
                dgvDetainedLicenses.Columns[7].Width = 200;

                dgvDetainedLicenses.Columns[8].HeaderText = "Rlease App.ID";
                dgvDetainedLicenses.Columns[8].Width = 90;
            }
        }

        private void LoadFilter() => filterBy1.LoadColumn(dgvDetainedLicenses);

        private void ListDetainedLicenses_Load(object sender, System.EventArgs e)
        {
            LoadDGVInfo();
            LoadFilter();
        }

        private void filterBy1_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvDetainedLicenses.DataSource = e.FilteredView;
            lblTotalRecords.Text = e.FoundRows.ToString();
        }

        private void btnReleaseDetainedLicense_Click(object sender, System.EventArgs e)
        {
            ReleaseDetainedLicensecs ReleaseDetainedLicensecs = new ReleaseDetainedLicensecs();
            ReleaseDetainedLicensecs.ShowDialog();
            LoadDGVInfo();
            LoadFilter();
        }

        private void btnDetainLicense_Click(object sender, System.EventArgs e)
        {
            DetainLicense DetainLicense = new DetainLicense();
            DetainLicense.ShowDialog();
            LoadDGVInfo();
            LoadFilter();
        }

        private void btnClose_Click(object sender, System.EventArgs e) => this.Close();

        private void PesonDetailsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (ClsPerson.Find(dgvDetainedLicenses?.CurrentRow?.Cells[6].Value.ToString()) is ClsPerson Person)
            {
                PersonDetails PersonDetails = new PersonDetails(Person.PersonID);
                PersonDetails.ShowDialog();
            }
            else
            {
                MessageBox.Show("This person does not Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
            {
                ShowLocalLicenseInfo ShowLocalLicenseInfo = new ShowLocalLicenseInfo(LicenseID);
                ShowLocalLicenseInfo.ShowDialog();
            }
            else
            {
                MessageBox.Show("This License does not Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (ClsPerson.Find(dgvDetainedLicenses?.CurrentRow?.Cells[6].Value.ToString()) is ClsPerson Person)
            {
                ShowPersonLicenseHistory PersonLicenseHistory = new ShowPersonLicenseHistory(Person.PersonID);
                PersonLicenseHistory.ShowDialog();
            }
            else
            {
                MessageBox.Show("This person does not Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
            {
                ReleaseDetainedLicensecs ReleaseDetainedLicensecs = new ReleaseDetainedLicensecs(LicenseID);
                ReleaseDetainedLicensecs.ShowDialog();
                LoadDGVInfo();
                LoadFilter();
            }
            else
            {
                MessageBox.Show("This License does not Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cmsApplications_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            releaseDetainedLicenseToolStripMenuItem.Enabled = !(bool)dgvDetainedLicenses?.CurrentRow?.Cells[3]?.Value;
        }
    }
}
