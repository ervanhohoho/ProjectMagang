namespace testProjectBCA
{
    partial class ExportOpti
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
            this.kanwilComboBox = new System.Windows.Forms.ComboBox();
            this.ExportBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // kanwilComboBox
            // 
            this.kanwilComboBox.FormattingEnabled = true;
            this.kanwilComboBox.Location = new System.Drawing.Point(12, 12);
            this.kanwilComboBox.Name = "kanwilComboBox";
            this.kanwilComboBox.Size = new System.Drawing.Size(121, 21);
            this.kanwilComboBox.TabIndex = 0;
            // 
            // ExportBtn
            // 
            this.ExportBtn.Location = new System.Drawing.Point(160, 12);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(75, 23);
            this.ExportBtn.TabIndex = 1;
            this.ExportBtn.Text = "Export";
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // ExportOpti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 431);
            this.Controls.Add(this.ExportBtn);
            this.Controls.Add(this.kanwilComboBox);
            this.Name = "ExportOpti";
            this.Text = "ExportOpti";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox kanwilComboBox;
        private System.Windows.Forms.Button ExportBtn;
    }
}