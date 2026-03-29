namespace DVLD
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.LocalLicense = new System.Windows.Forms.ToolStripMenuItem();
            this.InternationalLicense = new System.Windows.Forms.ToolStripMenuItem();
            this.renewDrivingLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.releaseDetainedDrivingLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageLocalLicenseApplications = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageInternationalLicenseApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.DetainLicensesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageDetainedLicensestoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.detainLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseDetainedLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageApplicationsType = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageTestTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.ManegePeople = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageDriver = new System.Windows.Forms.ToolStripMenuItem();
            this.BManageUser = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.CurrentUserInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangePassword = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SignOut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.ManegePeople,
            this.ManageDriver,
            this.BManageUser,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1808, 72);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.toolStripSeparator2,
            this.toolStripMenuItem8,
            this.toolStripSeparator3,
            this.DetainLicensesToolStripMenuItem1,
            this.ManageApplicationsType,
            this.ManageTestTypes});
            this.toolStripMenuItem3.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem3.Image = global::DVLD.Properties.Resources.Applications_64;
            this.toolStripMenuItem3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(188, 68);
            this.toolStripMenuItem3.Text = "Applications";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.renewDrivingLicenseToolStripMenuItem,
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem,
            this.toolStripSeparator4,
            this.releaseDetainedDrivingLicenseToolStripMenuItem,
            this.toolStripSeparator5});
            this.toolStripMenuItem4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem4.Image = global::DVLD.Properties.Resources.Driver_License_48;
            this.toolStripMenuItem4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(324, 70);
            this.toolStripMenuItem4.Text = "Driving licenses services";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LocalLicense,
            this.InternationalLicense});
            this.toolStripMenuItem5.Image = global::DVLD.Properties.Resources.New_Driving_License_32;
            this.toolStripMenuItem5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(399, 38);
            this.toolStripMenuItem5.Text = "New driving license";
            // 
            // LocalLicense
            // 
            this.LocalLicense.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocalLicense.Image = global::DVLD.Properties.Resources.Local_32;
            this.LocalLicense.Name = "LocalLicense";
            this.LocalLicense.Size = new System.Drawing.Size(265, 26);
            this.LocalLicense.Text = "Local license";
            this.LocalLicense.Click += new System.EventHandler(this.AddUpdateLocalDrivingLicesnseMenuItem6_Click);
            // 
            // InternationalLicense
            // 
            this.InternationalLicense.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InternationalLicense.Image = global::DVLD.Properties.Resources.International_32;
            this.InternationalLicense.Name = "InternationalLicense";
            this.InternationalLicense.Size = new System.Drawing.Size(265, 26);
            this.InternationalLicense.Text = "International driving license";
            this.InternationalLicense.Click += new System.EventHandler(this.InternationalLicense_Click);
            // 
            // renewDrivingLicenseToolStripMenuItem
            // 
            this.renewDrivingLicenseToolStripMenuItem.Image = global::DVLD.Properties.Resources.Renew_Driving_License_32;
            this.renewDrivingLicenseToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.renewDrivingLicenseToolStripMenuItem.Name = "renewDrivingLicenseToolStripMenuItem";
            this.renewDrivingLicenseToolStripMenuItem.Size = new System.Drawing.Size(399, 38);
            this.renewDrivingLicenseToolStripMenuItem.Text = "&Renew Driving License";
            this.renewDrivingLicenseToolStripMenuItem.Click += new System.EventHandler(this.renewDrivingLicenseToolStripMenuItem_Click);
            // 
            // ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem
            // 
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.Image = global::DVLD.Properties.Resources.Damaged_Driving_License_32;
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.Name = "ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem";
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.Size = new System.Drawing.Size(399, 38);
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.Text = "Replacement for Lost or &Damaged License";
            this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem.Click += new System.EventHandler(this.ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(396, 6);
            // 
            // releaseDetainedDrivingLicenseToolStripMenuItem
            // 
            this.releaseDetainedDrivingLicenseToolStripMenuItem.Image = global::DVLD.Properties.Resources.Detained_Driving_License_32;
            this.releaseDetainedDrivingLicenseToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.releaseDetainedDrivingLicenseToolStripMenuItem.Name = "releaseDetainedDrivingLicenseToolStripMenuItem";
            this.releaseDetainedDrivingLicenseToolStripMenuItem.Size = new System.Drawing.Size(399, 38);
            this.releaseDetainedDrivingLicenseToolStripMenuItem.Text = "Release Detained Driving License";
            this.releaseDetainedDrivingLicenseToolStripMenuItem.Click += new System.EventHandler(this.releaseDetainedDrivingLicenseToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(396, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(321, 6);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ManageLocalLicenseApplications,
            this.ManageInternationalLicenseApplication});
            this.toolStripMenuItem8.Image = global::DVLD.Properties.Resources.Manage_Applications_64;
            this.toolStripMenuItem8.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(324, 70);
            this.toolStripMenuItem8.Text = "Manage applications";
            // 
            // ManageLocalLicenseApplications
            // 
            this.ManageLocalLicenseApplications.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageLocalLicenseApplications.Image = global::DVLD.Properties.Resources.LocalDriving_License;
            this.ManageLocalLicenseApplications.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageLocalLicenseApplications.Name = "ManageLocalLicenseApplications";
            this.ManageLocalLicenseApplications.Size = new System.Drawing.Size(335, 38);
            this.ManageLocalLicenseApplications.Text = "Local driving license applications";
            this.ManageLocalLicenseApplications.Click += new System.EventHandler(this.LocaldrivinglicenseListMenuItem9_Click);
            // 
            // ManageInternationalLicenseApplication
            // 
            this.ManageInternationalLicenseApplication.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageInternationalLicenseApplication.Image = global::DVLD.Properties.Resources.International_32;
            this.ManageInternationalLicenseApplication.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageInternationalLicenseApplication.Name = "ManageInternationalLicenseApplication";
            this.ManageInternationalLicenseApplication.Size = new System.Drawing.Size(335, 38);
            this.ManageInternationalLicenseApplication.Text = "International license applications";
            this.ManageInternationalLicenseApplication.Click += new System.EventHandler(this.ManageInternationalLicenseApplication_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(321, 6);
            // 
            // DetainLicensesToolStripMenuItem1
            // 
            this.DetainLicensesToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ManageDetainedLicensestoolStripMenuItem1,
            this.detainLicenseToolStripMenuItem,
            this.releaseDetainedLicenseToolStripMenuItem});
            this.DetainLicensesToolStripMenuItem1.Image = global::DVLD.Properties.Resources.Detain_64;
            this.DetainLicensesToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.DetainLicensesToolStripMenuItem1.Name = "DetainLicensesToolStripMenuItem1";
            this.DetainLicensesToolStripMenuItem1.Size = new System.Drawing.Size(324, 70);
            this.DetainLicensesToolStripMenuItem1.Text = "Detain Licenses";
            // 
            // ManageDetainedLicensestoolStripMenuItem1
            // 
            this.ManageDetainedLicensestoolStripMenuItem1.Image = global::DVLD.Properties.Resources.Detain_32;
            this.ManageDetainedLicensestoolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageDetainedLicensestoolStripMenuItem1.Name = "ManageDetainedLicensestoolStripMenuItem1";
            this.ManageDetainedLicensestoolStripMenuItem1.Size = new System.Drawing.Size(318, 38);
            this.ManageDetainedLicensestoolStripMenuItem1.Text = "Manage Detained Licenses";
            this.ManageDetainedLicensestoolStripMenuItem1.Click += new System.EventHandler(this.ManageDetainedLicensestoolStripMenuItem1_Click);
            // 
            // detainLicenseToolStripMenuItem
            // 
            this.detainLicenseToolStripMenuItem.Image = global::DVLD.Properties.Resources.Detain_32;
            this.detainLicenseToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.detainLicenseToolStripMenuItem.Name = "detainLicenseToolStripMenuItem";
            this.detainLicenseToolStripMenuItem.Size = new System.Drawing.Size(318, 38);
            this.detainLicenseToolStripMenuItem.Text = "Detain License";
            this.detainLicenseToolStripMenuItem.Click += new System.EventHandler(this.detainLicenseToolStripMenuItem_Click);
            // 
            // releaseDetainedLicenseToolStripMenuItem
            // 
            this.releaseDetainedLicenseToolStripMenuItem.Image = global::DVLD.Properties.Resources.Release_Detained_License_32;
            this.releaseDetainedLicenseToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.releaseDetainedLicenseToolStripMenuItem.Name = "releaseDetainedLicenseToolStripMenuItem";
            this.releaseDetainedLicenseToolStripMenuItem.Size = new System.Drawing.Size(318, 38);
            this.releaseDetainedLicenseToolStripMenuItem.Text = "Release Detained License";
            this.releaseDetainedLicenseToolStripMenuItem.Click += new System.EventHandler(this.releaseDetainedLicenseToolStripMenuItem_Click);
            // 
            // ManageApplicationsType
            // 
            this.ManageApplicationsType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageApplicationsType.Image = global::DVLD.Properties.Resources.Application_Types_64;
            this.ManageApplicationsType.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageApplicationsType.Name = "ManageApplicationsType";
            this.ManageApplicationsType.Size = new System.Drawing.Size(324, 70);
            this.ManageApplicationsType.Text = "Manage applications types";
            this.ManageApplicationsType.Click += new System.EventHandler(this.applicationsTypesMenuItem1_Click_1);
            // 
            // ManageTestTypes
            // 
            this.ManageTestTypes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageTestTypes.Image = global::DVLD.Properties.Resources.Test_Type_641;
            this.ManageTestTypes.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageTestTypes.Name = "ManageTestTypes";
            this.ManageTestTypes.Size = new System.Drawing.Size(324, 70);
            this.ManageTestTypes.Text = "Manage test types";
            this.ManageTestTypes.Click += new System.EventHandler(this.ManageTestTypes_Click);
            // 
            // ManegePeople
            // 
            this.ManegePeople.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManegePeople.Image = global::DVLD.Properties.Resources.People_64;
            this.ManegePeople.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManegePeople.Name = "ManegePeople";
            this.ManegePeople.Size = new System.Drawing.Size(141, 68);
            this.ManegePeople.Text = "People";
            this.ManegePeople.Click += new System.EventHandler(this.ManegePeople_Click);
            // 
            // ManageDriver
            // 
            this.ManageDriver.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManageDriver.Image = global::DVLD.Properties.Resources.Drivers_64;
            this.ManageDriver.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ManageDriver.Name = "ManageDriver";
            this.ManageDriver.Size = new System.Drawing.Size(145, 68);
            this.ManageDriver.Text = "Drivers";
            this.ManageDriver.Click += new System.EventHandler(this.ManageDriver_Click);
            // 
            // BManageUser
            // 
            this.BManageUser.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BManageUser.Image = global::DVLD.Properties.Resources.Users_2_64;
            this.BManageUser.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BManageUser.Name = "BManageUser";
            this.BManageUser.Size = new System.Drawing.Size(130, 68);
            this.BManageUser.Text = "Users";
            this.BManageUser.Click += new System.EventHandler(this.BManageUser_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CurrentUserInfo,
            this.ChangePassword,
            this.toolStripSeparator1,
            this.SignOut});
            this.toolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem2.Image = global::DVLD.Properties.Resources.account_settings_64;
            this.toolStripMenuItem2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(221, 68);
            this.toolStripMenuItem2.Text = "Account settings";
            // 
            // CurrentUserInfo
            // 
            this.CurrentUserInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentUserInfo.Image = global::DVLD.Properties.Resources.PersonDetails_321;
            this.CurrentUserInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.CurrentUserInfo.Name = "CurrentUserInfo";
            this.CurrentUserInfo.Size = new System.Drawing.Size(280, 38);
            this.CurrentUserInfo.Text = "Current user information";
            this.CurrentUserInfo.Click += new System.EventHandler(this.CurrentUserInfo_Click);
            // 
            // ChangePassword
            // 
            this.ChangePassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangePassword.Image = global::DVLD.Properties.Resources.Password_32;
            this.ChangePassword.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ChangePassword.Name = "ChangePassword";
            this.ChangePassword.Size = new System.Drawing.Size(280, 38);
            this.ChangePassword.Text = "Change password";
            this.ChangePassword.Click += new System.EventHandler(this.Change_Password_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(277, 6);
            // 
            // SignOut
            // 
            this.SignOut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignOut.Image = global::DVLD.Properties.Resources.sign_out_32__2;
            this.SignOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.SignOut.Name = "SignOut";
            this.SignOut.Size = new System.Drawing.Size(280, 38);
            this.SignOut.Text = "Sign out";
            this.SignOut.Click += new System.EventHandler(this.SignOut_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(1808, 771);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ManegePeople;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem SignOut;
        private System.Windows.Forms.ToolStripMenuItem ChangePassword;
        private System.Windows.Forms.ToolStripMenuItem BManageUser;
        private System.Windows.Forms.ToolStripMenuItem CurrentUserInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem ManageApplicationsType;
        private System.Windows.Forms.ToolStripMenuItem ManageTestTypes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem LocalLicense;
        private System.Windows.Forms.ToolStripMenuItem InternationalLicense;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ManageLocalLicenseApplications;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ManageInternationalLicenseApplication;
        private System.Windows.Forms.ToolStripMenuItem ManageDriver;
        private System.Windows.Forms.ToolStripMenuItem renewDrivingLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DetainLicensesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ManageDetainedLicensestoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem detainLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releaseDetainedLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem releaseDetainedDrivingLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}