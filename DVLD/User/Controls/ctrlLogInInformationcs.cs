using BusinessLayer;
using System.Windows.Forms;

namespace DVLD.Controls
{
    public partial class ctrlLogInInformationcs : UserControl
    {
        public int UserID { get; set; }

        public ctrlLogInInformationcs()
        {
            InitializeComponent();
        }

        public void LoadUserInfo(int UserID)
        {
            this.UserID = UserID;
            SetData();
        }

        // Fetches user data from the Business Layer and displays it in the control
        private void SetData()
        {
            ClsUser User = ClsUser.Find(UserID);

            if (User != null)
            {
                // Display user information in corresponding labels
                lblUserID.Text = User.UserID.ToString();
                lblUserName.Text = User.UserName;
                lblIsActive.Text = User.IsActive == true ? "Yes" : "No";

                // Load related person data into the embedded person card control
                ctrlPersonCard2.LoadPerson(User.PersonID);
            }
        }
    }
}
