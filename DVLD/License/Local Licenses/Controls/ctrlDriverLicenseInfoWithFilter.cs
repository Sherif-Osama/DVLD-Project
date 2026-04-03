using BusinessLayer;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.License.Local_Licenses.Controls
{
    // Simple control to search and display a driver's license by ID with a small UI.
    public partial class ctrlDriverLicenseInfoWithFilter : UserControl
    {
        public class FilterResult : EventArgs
        {
            public int LicenseID { get; }
            public bool IsFound { get; }

            public FilterResult(int LicensesID, bool IsFound)
            {
                this.LicenseID = LicensesID;
                this.IsFound = IsFound;
            }
        }

        public event EventHandler<FilterResult> LicenseSearchCompleted;

        protected virtual void OnLicenseSearchCompleted(bool IsFound, int licenseID)
        {
            LicenseSearchCompleted?.Invoke(this, new FilterResult(licenseID, IsFound));
        }

        public ctrlDriverLicenseInfoWithFilter() => InitializeComponent();

        public async Task LoadLicenseInfoAsync(int LicenseID)
        {
            await ctrlDriverLicenseInfo1.LoadLicenseInfoAsync(LicenseID);
            Filtertext.Text = LicenseID.ToString();
            Filtertext.Enabled = false;
        }

        // Try to parse the text box as an integer license ID and load license info.
        private async Task SearchLicenseAsync()
        {
            if (int.TryParse(Filtertext.Text, out int LicenseID))
            {
                // Look up license by ID
                if (await ClsLicenses.FindAsync(LicenseID) is ClsLicenses License)
                {
                    // Load license details into the child control and notify listeners.
                    await ctrlDriverLicenseInfo1.LoadLicenseInfoAsync(License.LicenseID);

                    OnLicenseSearchCompleted(true, License.LicenseID);
                }
                else
                { OnLicenseSearchCompleted(false, -1); }
            }
            else // Invalid input: inform the user.
            { MessageBox.Show("Invalid License ID.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        // Handler for the Find button: validate input then run search.
        private async void btnFind_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Filtertext.Text)) { await SearchLicenseAsync(); }
            // Show error if the field is empty
            else { errorProvider1.SetError(Filtertext, "This field is required!"); }
        }

        // Allow only digits in the filter textbox (max 9 chars).
        private void Filtertext_KeyPress(object sender, KeyPressEventArgs e)
        {
            Filtertext.MaxLength = 9;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

            if (e.KeyChar == (char)13)
                btnFind.PerformClick();
        }

        // Clear validation error when the user modifies the text.
        private void Filtertext_TextChanged(object sender, EventArgs e) => errorProvider1.SetError(Filtertext, "");
    }
}