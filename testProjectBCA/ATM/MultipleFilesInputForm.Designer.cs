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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.filesListBelumMasuk = new System.Windows.Forms.ListBox();
            this.dataYangSudahAdaList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectFilesButton
            // 
            this.SelectFilesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectFilesButton.Location = new System.Drawing.Point(3, 3);
            this.SelectFilesButton.Name = "SelectFilesButton";
            this.tableLayoutPanel1.SetRowSpan(this.SelectFilesButton, 2);
            this.SelectFilesButton.Size = new System.Drawing.Size(188, 58);
            this.SelectFilesButton.TabIndex = 0;
            this.SelectFilesButton.Text = "Select Files";
            this.SelectFilesButton.UseVisualStyleBackColor = true;
            this.SelectFilesButton.Click += new System.EventHandler(this.SelectFilesButton_Click);
            // 
            // filesList
            // 
            this.filesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesList.FormattingEnabled = true;
            this.filesList.IntegralHeight = false;
            this.filesList.Location = new System.Drawing.Point(197, 35);
            this.filesList.Name = "filesList";
            this.tableLayoutPanel1.SetRowSpan(this.filesList, 3);
            this.filesList.Size = new System.Drawing.Size(146, 88);
            this.filesList.TabIndex = 1;
            // 
            // inputBtn
            // 
            this.inputBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputBtn.Location = new System.Drawing.Point(3, 67);
            this.inputBtn.Name = "inputBtn";
            this.tableLayoutPanel1.SetRowSpan(this.inputBtn, 2);
            this.inputBtn.Size = new System.Drawing.Size(188, 56);
            this.inputBtn.TabIndex = 2;
            this.inputBtn.Text = "Input";
            this.inputBtn.UseVisualStyleBackColor = true;
            this.inputBtn.Click += new System.EventHandler(this.inputBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(197, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Data yang akan masuk";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(349, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data yang belum ada";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // filesListBelumMasuk
            // 
            this.filesListBelumMasuk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesListBelumMasuk.FormattingEnabled = true;
            this.filesListBelumMasuk.IntegralHeight = false;
            this.filesListBelumMasuk.Location = new System.Drawing.Point(349, 35);
            this.filesListBelumMasuk.Name = "filesListBelumMasuk";
            this.tableLayoutPanel1.SetRowSpan(this.filesListBelumMasuk, 3);
            this.filesListBelumMasuk.Size = new System.Drawing.Size(122, 88);
            this.filesListBelumMasuk.TabIndex = 5;
            // 
            // dataYangSudahAdaList
            // 
            this.dataYangSudahAdaList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataYangSudahAdaList.FormattingEnabled = true;
            this.dataYangSudahAdaList.IntegralHeight = false;
            this.dataYangSudahAdaList.Location = new System.Drawing.Point(477, 35);
            this.dataYangSudahAdaList.Name = "dataYangSudahAdaList";
            this.tableLayoutPanel1.SetRowSpan(this.dataYangSudahAdaList, 3);
            this.dataYangSudahAdaList.Size = new System.Drawing.Size(154, 88);
            this.dataYangSudahAdaList.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(477, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 32);
            this.label3.TabIndex = 7;
            this.label3.Text = "Data yang sudah ada";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.16438F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.83562F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 159F));
            this.tableLayoutPanel1.Controls.Add(this.filesListBelumMasuk, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.inputBtn, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.SelectFilesButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.filesList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataYangSudahAdaList, 3, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 126);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // MultipleFilesInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 150);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MultipleFilesInputForm";
            this.Text = "Input Laporan PKT";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}