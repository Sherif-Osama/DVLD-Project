using System;
using System.Windows.Forms;

namespace DVLD.User
{
    public partial class UserDetailes : Form
    {
        private int UserID { get; set; }

        public UserDetailes(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
        }

        // Event handler triggered when the embedded control (ctrlLogInInformationcs1) is loaded
        private void ctrlLogInInformationcs1_Load(object sender, EventArgs e)
        {
            if (UserID != 0)
            {
                ctrlLogInInformationcs1.LoadUserInfo(UserID);
            }
        }
    }
}