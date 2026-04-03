using BusinessLayer;
using DVLD.License.International_License;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.License
{
    public partial class ctrlDriverLicenses : UserControl
    {
        ClsDriver Driver;

        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }

        private async Task LoadLocalLicenseInfoAsync()
        {
            dgvLocalLicensesHistory.DataSource = await ClsDriver.GetAllLocalLicensesAsync(Driver.DriverID);
            int RowCount = dgvLocalLicensesHistory.Rows.Count;
            lblLocalLicensesRecords.Text = RowCount.ToString();

            if (RowCount > 0)
            {
                dgvLocalLicensesHistory.Columns[0].HeaderText = "Lic.ID";
                dgvLocalLicensesHistory.Columns[0].Width = 85;

                dgvLocalLicensesHistory.Columns[1].HeaderText = "App.ID";
                dgvLocalLicensesHistory.Columns[1].Width = 85;

                dgvLocalLicensesHistory.Columns[2].HeaderText = "Class Name";
                dgvLocalLicensesHistory.Columns[2].Width = 200;

                dgvLocalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvLocalLicensesHistory.Columns[3].Width = 130;

                dgvLocalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvLocalLicensesHistory.Columns[4].Width = 130;

                dgvLocalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvLocalLicensesHistory.Columns[5].Width = 80;
            }
        }

        private async Task LoadInternationalLicenseInfoAsync()
        {
            dgvInternationalLicensesHistory.DataSource = await ClsDriver.GetAllInternationalLicensesAsync(Driver.DriverID);
            int RowCount = dgvInternationalLicensesHistory.Rows.Count;
            lblInternationalLicensesRecords.Text = RowCount.ToString();

            if (RowCount > 0)
            {
                dgvInternationalLicensesHistory.Columns[0].HeaderText = "Int.License ID";
                dgvInternationalLicensesHistory.Columns[0].Width = 105;

                dgvInternationalLicensesHistory.Columns[1].HeaderText = "Application ID";
                dgvInternationalLicensesHistory.Columns[1].Width = 105;

                dgvInternationalLicensesHistory.Columns[2].HeaderText = "L.License ID";
                dgvInternationalLicensesHistory.Columns[2].Width = 105;

                dgvInternationalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvInternationalLicensesHistory.Columns[3].Width = 150;

                dgvInternationalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvInternationalLicensesHistory.Columns[4].Width = 150;

                dgvInternationalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvInternationalLicensesHistory.Columns[5].Width = 80;
            }
        }

        public async Task LoadAllDriverLicensesAsync(int DriverID)
        {
            Driver = await ClsDriver.FindAsync(DriverID);

            if (Driver != null)
            {
                await LoadLocalLicenseInfoAsync();
                await LoadInternationalLicenseInfoAsync();
            }
            else
            {
                MessageBox.Show("Driver information could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showLicenseInfoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvLocalLicensesHistory.CurrentRow.Cells[0].Value.ToString(), out int LicenseID))
            {
                ShowLocalLicenseInfo LicenseInfo = new ShowLocalLicenseInfo(LicenseID);
                LicenseInfo.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not retrieve the License ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InternationalLicenseHistorytoolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(dgvInternationalLicensesHistory.CurrentRow.Cells[0].Value.ToString(), out int InternationalLicenseID))
            {
                ShowInternationalLicenseInfo InternationalLicenseInfo = new ShowInternationalLicenseInfo(InternationalLicenseID);
                InternationalLicenseInfo.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not retrieve the License ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}