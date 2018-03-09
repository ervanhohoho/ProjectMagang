namespace testProjectBCA
{
    partial class MultipleFilesInputForm
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
            this.SelectFilesButton = new System.Windows.Forms.Button();
            this.filesList = new System.Windows.Forms.ListBox();
            this.inputBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SelectFilesButton
            // 
            this.SelectFilesButton.Location = new System.Drawing.Point(12, 12);
            this.SelectFilesButton.Name = "SelectFilesButton";
            this.SelectFilesButton.Size = new System.Drawing.Size(75, 23);
            this.SelectFilesButton.TabIndex = 0;
            this.SelectFilesButton.Text = "Select Files";
            this.SelectFilesButton.UseVisualStyleBackColor = true;
            this.SelectFilesButton.Click += new System.EventHandler(this.SelectFilesButton_Click);
            // 
            // filesList
            // 
            this.filesList.FormattingEnabled = true;
            this.filesList.Location = new System.Drawing.Point(181, 12);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(120, 95);
            this.filesList.TabIndex = 1;
            // 
            // inputBtn
            // 
            this.inputBtn.Location = new System.Drawing.Point(12, 41);
            this.inputBtn.Name = "inputBtn";
            this.inputBtn.Size = new System.Drawing.Size(75, 23);
            this.inputBtn.TabIndex = 2;
            this.inputBtn.Text = "Input";
            this.inputBtn.UseVisualStyleBackColor = true;
            this.inputBtn.Click += new System.EventHandler(this.inputBtn_Click);
            // 
            // MultipleFilesInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 143);
            this.Controls.Add(this.inputBtn);
            this.Controls.Add(this.filesList);
            this.Controls.Add(this.SelectFilesButton);
            this.Name = "MultipleFilesInputForm";
            this.Text = "Input Laporan PKT";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SelectFilesButton;
        private System.Windows.Forms.ListBox filesList;
        private System.Windows.Forms.Button inputBtn;
    }
}