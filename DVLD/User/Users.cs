using BusinessLayer;
using DVLD.Global_classes;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.User
{
    public partial class Users : Form
    {
        #region Initialization Components
        // Constructor initializes the form
        public Users() => InitializeComponent();

        // Refreshes the DataGridView by reloading all users and filters
        private async Task RefreshDataGridView()
        {
            dgvUsers.DataSource = await ClsUser.GetAllUsersDataAsync();
            LoadFilterData();
        }
        #endregion

        #region Load UI and Data
        // Triggered when the form is loaded
        // Loads users and initializes the filtering options
        private async void Users_Load(object sender, EventArgs e)
        {
            await LoadUsersData();
            LoadFilterData();
        }

        // Loads all users into the DataGridView and sets column headers and widths
        private async Task LoadUsersData()
        {
            dgvUsers.DataSource = await ClsUser.GetAllUsersDataAsync();
            int NumberOfRows = dgvUsers.Rows.Count;
            RecordsNumber.Text = NumberOfRows.ToString(); // Display total count

            if (NumberOfRows > 0)
            {
                dgvUsers.Columns[0].HeaderText = "User ID";
                dgvUsers.Columns[0].Width = 90;

                dgvUsers.Columns[1].HeaderText = "Person ID";
                dgvUsers.Columns[1].Width = 90;

                dgvUsers.Columns[2].HeaderText = "Full Name";
                dgvUsers.Columns[2].Width = 225;

                dgvUsers.Columns[3].HeaderText = "User Name";
                dgvUsers.Columns[3].Width = 100;

                dgvUsers.Columns[4].HeaderText = "Is Active";
                dgvUsers.Columns[4].Width = 94;
            }
        }
        #endregion

        #region load Filter
        // Loads filter columns dynamically based on the DataGridView
        private void LoadFilterData() => filterUsersBy.LoadColumn(dgvUsers);

        // Updates the grid view when filter results change
        private void filterUsersBy_FilterResultsChanged(object sender, FilterBy.FilterResultsEventArgs e)
        {
            dgvUsers.DataSource = e.FilteredView;
            RecordsNumber.Text = e.FoundRows.ToString();
        }
        #endregion

        #region Buttons Actions
        // Opens the Add User form
        private async void BtnAddNewUser_Click(object sender, EventArgs e)
        {
            AddOrUpdateUserForm AddNewUser = new AddOrUpdateUserForm();
            AddNewUser.RefreshFormData += async () => await RefreshDataGridView(); // Refresh data after adding
            AddNewUser.ShowDialog();
        }

        // Closes the Users management form
        private void CloseUsersForm_Click(object sender, EventArgs e) => this.Close();

        // Opens the User Details form for the selected user
        private void showUserDetailesMenuItem_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvUsers.CurrentRow.Cells[0].Value.ToString(), out int ID))
            {
                UserDetailes UserDetailes = new UserDetailes(ID);
                UserDetailes.ShowDialog();
            }
        }
        #endregion

        #region ToolStripMenu Actions
        // Opens the Edit User form for the selected user
        private void AddOrUpdateUser_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvUsers.CurrentRow.Cells[0].Value.ToString(), out int ID))
            {
                AddOrUpdateUserForm EditUser = new AddOrUpdateUserForm(ID);
                EditUser.RefreshFormData += async () => await RefreshDataGridView(); // Refresh data after editing
                EditUser.ShowDialog();
            }
        }

        // Deletes the selected user from the database after confirmation
        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string UserIDMessage = dgvUsers.CurrentRow.Cells[0].Value.ToString();

            if (MessageBox.Show($"Do you want delete Person with ID {UserIDMessage}",
                "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (int.TryParse(dgvUsers.CurrentRow?.Cells[0]?.Value?.ToString(), out int UserID))
                {
                    if (await ClsUser.DeleteAsync(UserID))
                    {
                        MessageBox.Show("Deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await RefreshDataGridView();
                    }
                    else
                    { MessageBox.Show("Failed", "The operation failed.", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                else
                {
                    MessageBox.Show("No user selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Opens the Change Password form for the selected user
        private void ChangePassword_Click(object sender, EventArgs e)
        {
            if (int.TryParse(dgvUsers.CurrentRow.Cells[0].Value.ToString(), out int UserID))
            {
                Change_Password ChangePassword = new Change_Password(UserID);
                ChangePassword.ShowDialog();
            }
        }
        #endregion


    }
}
