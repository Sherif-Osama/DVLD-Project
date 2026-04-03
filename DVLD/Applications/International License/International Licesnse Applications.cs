using BusinessLayer;
using DVLD.Global_classes;
using DVLD.License;
using DVLD.License.International_License;
using DVLD.People;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.International_License
{
    public partial class ListInternationalLicesnseApplications : Form
    {
        public ListInternationalLicesnseApplications()
        {
            InitializeComponent();
        }

        private async Task RefreshDataGridViewAsync()
        {
            dgvInternationalLicenses.DataSource = await ClsInternationalLicenses.GetAllInternationalLicensesAsync();

            int totalRecords = dgvInternationalLicenses.Rows.Count;

            if (totalRecords > 0)
            {
                {
                    dgvInternationalLicenses.Columns[0].HeaderText = "Int.License ID";
                    dgvInternationalLicenses.Columns[0].Width = 100;

                    dgvInternationalLicenses.Columns[1].HeaderText = "Application ID";
                    dgvInternationalLicenses.Columns[1].Width = 100;

                    dgvInternationalLicenses.Columns[2].HeaderText = "Driver ID";
                    dgvInternationalLicenses.Columns[2].Width = 100;

                    dgvInternationalLicenses.Columns[3].HeaderText = "L.License ID";
                    dgvInternationalLicenses.Columns[3].Width = 100;

                    dgvInternationalLicenses.Columns[4].HeaderText = "Issue Date";
                    dgvInternationalLicenses.Columns[4].Width = 150;

                    dgvInternationalLicenses.Columns[5].HeaderText = "Expiration Date";
                    dgvInternationalLicenses.Columns[5].Width = 150;

                    dgvInternationalLicenses.Columns[6].HeaderText = "Is Active";
                    dgvInternationalLicenses.Columns[6].Width = 100;
                }

                lblInternationalLicensesRecords.Text = totalRecords.ToString();
            }
        }

        private void LoadFilterData()
        {
            if (dgvInternationalLicenses != null)
            {
                filterBy1.LoadColumn(dgvInternationalLicenses);
            }
        }

        private async void ListInternationalLicesnseApplications_Load(object sender, System.EventArgs e)
        {
            await RefreshDataGridViewAsync();
            LoadFilterData();
        }


        private void filterBy1_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvInternationalLicenses.DataSource = e.FilteredView;
            lblInternationalLicensesRecords.Text = e.FoundRows.ToString();
        }

        private async void PersonDetailsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvInternationalLicenses.CurrentRow.Cells[2].Value.ToString(), out int DriverID))
            {
                ClsDriver Driver = await ClsDriver.FindAsync(DriverID);
                if (Driver != null)
                {
                    PersonDetails Person = new PersonDetails(Driver.PersonID);
                    Person.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Driver not found");
                }
            }
            else
            {
                MessageBox.Show("Invalid Driver ID");
            }
        }

        private async void showDetailsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvInternationalLicenses.CurrentRow.Cells[0].Value.ToString(), out int LicenseID))
            {
                ClsInternationalLicenses License = await ClsInternationalLicenses.FindAsync(LicenseID);
                if (License != null)
                {
                    ShowInternationalLicenseInfo InternationalLicenseInfo = new ShowInternationalLicenseInfo(License.InternationalLicenseID);
                    InternationalLicenseInfo.ShowDialog();
                }
                else
                {
                    MessageBox.Show("License not found");
                }
            }
            else
            {
                MessageBox.Show("Invalid License ID");
            }
        }

        private async void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvInternationalLicenses.CurrentRow.Cells[2].Value.ToString(), out int DriverID))
            {
                ClsDriver Driver = await ClsDriver.FindAsync(DriverID);
                if (Driver != null)
                {
                    ShowPersonLicenseHistory PersonLicenseHistory = new ShowPersonLicenseHistory(Driver.PersonID);
                    PersonLicenseHistory.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Driver not found");
                }
            }
            else
            {
                MessageBox.Show("Invalid Driver ID");
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e) => this.Close();

        private void btnNewApplication_Click(object sender, System.EventArgs e)
        {
            InternationalLicense NewInternationalLicenseApplication = new InternationalLicense();
            NewInternationalLicenseApplication.ShowDialog();
        }

    }
}