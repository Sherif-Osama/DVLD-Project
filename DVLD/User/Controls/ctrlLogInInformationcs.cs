using BusinessLayer;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Controls
{
    public partial class ctrlLogInInformationcs : UserControl
    {
        public int UserID { get; set; }

        public ctrlLogInInformationcs() => InitializeComponent();

        public async Task LoadUserInfo(int UserID)
        {
            this.UserID = UserID;
            await SetData();
        }

        // Fetches user data from the Business Layer and displays it in the control
        private async Task SetData()
        {
            ClsUser User = await ClsUser.FindAsync(UserID);

            if (User != null)
            {
                // Display user information in corresponding labels
                lblUserID.Text = User.UserID.ToString();
                lblUserName.Text = User.UserName;
                lblIsActive.Text = User.IsActive == true ? "Yes" : "No";

                // Load related person data into the embedded person card control
                await ctrlPersonCard2.LoadPersonAsync(User.PersonID);
            }
        }
    }
}
