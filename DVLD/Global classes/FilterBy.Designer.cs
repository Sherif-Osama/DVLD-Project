namespace DVLD.Global_classes
{
    partial class FilterBy
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Filtertext = new System.Windows.Forms.TextBox();
            this.FilterBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.QuestionBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Filtertext
            // 
            this.Filtertext.Location = new System.Drawing.Point(277, 22);
            this.Filtertext.Name = "Filtertext";
            this.Filtertext.Size = new System.Drawing.Size(173, 22);
            this.Filtertext.TabIndex = 5;
            this.Filtertext.TextChanged += new System.EventHandler(this.Filtertext_TextChanged);
            this.Filtertext.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Filtertext_KeyPress);
            // 
            // FilterBox
            // 
            this.FilterBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FilterBox.FormattingEnabled = true;
            this.FilterBox.Location = new System.Drawing.Point(87, 21);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(170, 24);
            this.FilterBox.TabIndex = 4;
            this.FilterBox.SelectedIndexChanged += new System.EventHandler(this.FilterBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter By";
            // 
            // QuestionBox
            // 
            this.QuestionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QuestionBox.Location = new System.Drawing.Point(277, 22);
            this.QuestionBox.Name = "QuestionBox";
            this.QuestionBox.Size = new System.Drawing.Size(173, 24);
            this.QuestionBox.TabIndex = 0;
            this.QuestionBox.SelectedIndexChanged += new System.EventHandler(this.QuestionBox_SelectedIndexChanged);
            // 
            // FilterBy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.QuestionBox);
            this.Controls.Add(this.Filtertext);
            this.Controls.Add(this.FilterBox);
            this.Controls.Add(this.label1);
            this.Name = "FilterBy";
            this.Size = new System.Drawing.Size(481, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Filtertext;
        private System.Windows.Forms.ComboBox FilterBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox QuestionBox;
    }
}
