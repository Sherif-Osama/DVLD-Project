namespace DVLD.User
{
    partial class UserDetailes
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
            this.ctrlLogInInformationcs1 = new DVLD.Controls.ctrlLogInInformationcs();
            this.SuspendLayout();
            // 
            // ctrlLogInInformationcs1
            // 
            this.ctrlLogInInformationcs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlLogInInformationcs1.UserID = 0;
            this.ctrlLogInInformationcs1.Location = new System.Drawing.Point(0, 0);
            this.ctrlLogInInformationcs1.Name = "ctrlLogInInformationcs1";
            this.ctrlLogInInformationcs1.Size = new System.Drawing.Size(842, 447);
            this.ctrlLogInInformationcs1.TabIndex = 0;
            this.ctrlLogInInformationcs1.Load += new System.EventHandler(this.ctrlLogInInformationcs1_Load);
            // 
            // UserDetailes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 447);
            this.Controls.Add(this.ctrlLogInInformationcs1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserDetailes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UserDetailes";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ctrlLogInInformationcs ctrlLogInInformationcs1;
    }
}