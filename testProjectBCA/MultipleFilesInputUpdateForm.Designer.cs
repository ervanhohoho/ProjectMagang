namespace testProjectBCA
{
    partial class MultipleFilesInputUpdateForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.filesListBelumMasuk = new System.Windows.Forms.ListBox();
            this.dataYangSudahAdaList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SelectFilesButton
            // 
            this.SelectFilesButton.Location = new System.Drawing.Point(12, 12);
            this.SelectFilesButton.Name = "SelectFilesButton";
            this.SelectFilesButton.Size = new System.Drawing.Size(122, 54);
            this.SelectFilesButton.TabIndex = 0;
            this.SelectFilesButton.Text = "Select Files";
            this.SelectFilesButton.UseVisualStyleBackColor = true;
            this.SelectFilesButton.Click += new System.EventHandler(this.SelectFilesButton_Click);
            // 
            // filesList
            // 
            this.filesList.FormattingEnabled = true;
            this.filesList.Location = new System.Drawing.Point(181, 30);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(120, 95);
            this.filesList.TabIndex = 1;
            // 
            // inputBtn
            // 
            this.inputBtn.Location = new System.Drawing.Point(12, 82);
            this.inputBtn.Name = "inputBtn";
            this.inputBtn.Size = new System.Drawing.Size(122, 52);
            this.inputBtn.TabIndex = 2;
            this.inputBtn.Text = "Input";
            this.inputBtn.UseVisualStyleBackColor = true;
            this.inputBtn.Click += new System.EventHandler(this.inputBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Data yang akan masuk";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data yang belum ada";
            // 
            // filesListBelumMasuk
            // 
            this.filesListBelumMasuk.FormattingEnabled = true;
            this.filesListBelumMasuk.Location = new System.Drawing.Point(345, 28);
            this.filesListBelumMasuk.Name = "filesListBelumMasuk";
            this.filesListBelumMasuk.Size = new System.Drawing.Size(120, 95);
            this.filesListBelumMasuk.TabIndex = 5;
            // 
            // dataYangSudahAdaList
            // 
            this.dataYangSudahAdaList.FormattingEnabled = true;
            this.dataYangSudahAdaList.Location = new System.Drawing.Point(513, 28);
            this.dataYangSudahAdaList.Name = "dataYangSudahAdaList";
            this.dataYangSudahAdaList.Size = new System.Drawing.Size(120, 95);
            this.dataYangSudahAdaList.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(513, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Data yang sudah ada";
            // 
            // MultipleFilesInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 146);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataYangSudahAdaList);
            this.Controls.Add(this.filesListBelumMasuk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputBtn);
            this.Controls.Add(this.filesList);
            this.Controls.Add(this.SelectFilesButton);
            this.Name = "MultipleFilesInputForm";
            this.Text = "Input Laporan PKT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectFilesButton;
        private System.Windows.Forms.ListBox filesList;
        private System.Windows.Forms.Button inputBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox filesListBelumMasuk;
        private System.Windows.Forms.ListBox dataYangSudahAdaList;
        private System.Windows.Forms.Label label3;
    }
}