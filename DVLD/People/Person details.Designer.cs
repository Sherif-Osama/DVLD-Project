namespace DVLD.People
{
    partial class PersonDetails
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
            this.label1 = new System.Windows.Forms.Label();
            this.ClosePersonDetails = new System.Windows.Forms.Button();
            this.userControlInformation1 = new DVLD.People.Controls.ctrlPersonCard();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Brown;
            this.label1.Location = new System.Drawing.Point(254, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 38);
            this.label1.TabIndex = 1;
            this.label1.Text = "Person Details";
            // 
            // ClosePersonDetails
            // 
            this.ClosePersonDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClosePersonDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClosePersonDetails.Image = global::DVLD.Properties.Resources.Close_32;
            this.ClosePersonDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ClosePersonDetails.Location = new System.Drawing.Point(711, 453);
            this.ClosePersonDetails.Name = "ClosePersonDetails";
            this.ClosePersonDetails.Size = new System.Drawing.Size(122, 44);
            this.ClosePersonDetails.TabIndex = 4;
            this.ClosePersonDetails.Text = "Close";
            this.ClosePersonDetails.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ClosePersonDetails.UseVisualStyleBackColor = true;
            this.ClosePersonDetails.Click += new System.EventHandler(this.Close_Click);
            // 
            // userControlInformation1
            // 
            this.userControlInformation1.Location = new System.Drawing.Point(12, 107);
            this.userControlInformation1.Name = "userControlInformation1";
            this.userControlInformation1.Size = new System.Drawing.Size(821, 321);
            this.userControlInformation1.TabIndex = 2;
            // 
            // PersonDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(847, 509);
            this.Controls.Add(this.ClosePersonDetails);
            this.Controls.Add(this.userControlInformation1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PersonDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PersonDetails";
            this.Load += new System.EventHandler(this.PersonDetails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Controls.ctrlPersonCard userControlInformation1;
        private System.Windows.Forms.Button ClosePersonDetails;
    }
}