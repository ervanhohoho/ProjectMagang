namespace testProjectBCA
{
    partial class proyeksiLikuiditasForm
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
            this.rasio100TextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rasio50TextBox = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bulanPrediksiTreeView = new System.Windows.Forms.TreeView();
            this.tanggalMaxPrediksiPicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.metodePrediksiComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ForecastButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.fit100Num = new System.Windows.Forms.NumericUpDown();
            this.fit50Num = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.adhoc100Num = new System.Windows.Forms.NumericUpDown();
            this.adhoc50Num = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fit100Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fit50Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.adhoc100Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.adhoc50Num)).BeginInit();
            this.SuspendLayout();
            // 
            // rasio100TextBox
            // 
            this.rasio100TextBox.Location = new System.Drawing.Point(264, 75);
            this.rasio100TextBox.Name = "rasio100TextBox";
            this.rasio100TextBox.Size = new System.Drawing.Size(100, 20);
            this.rasio100TextBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(143, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target Rasio ATM 100";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Target Rasio ATM 50";
            // 
            // rasio50TextBox
            // 
            this.rasio50TextBox.Location = new System.Drawing.Point(264, 112);
            this.rasio50TextBox.Name = "rasio50TextBox";
            this.rasio50TextBox.Size = new System.Drawing.Size(100, 20);
            this.rasio50TextBox.TabIndex = 5;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 185);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1039, 410);
            this.dataGridView1.TabIndex = 4;
            // 
            // bulanPrediksiTreeView
            // 
            this.bulanPrediksiTreeView.Location = new System.Drawing.Point(16, 82);
            this.bulanPrediksiTreeView.Name = "bulanPrediksiTreeView";
            this.bulanPrediksiTreeView.Size = new System.Drawing.Size(121, 97);
            this.bulanPrediksiTreeView.TabIndex = 3;
            this.bulanPrediksiTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.bulanPrediksiTreeView_AfterCheck);
            // 
            // tanggalMaxPrediksiPicker
            // 
            this.tanggalMaxPrediksiPicker.Location = new System.Drawing.Point(355, 15);
            this.tanggalMaxPrediksiPicker.Name = "tanggalMaxPrediksiPicker";
            this.tanggalMaxPrediksiPicker.Size = new System.Drawing.Size(200, 20);
            this.tanggalMaxPrediksiPicker.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(240, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Tanggal Max Prediksi";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Bulan untuk prediksi";
            // 
            // metodePrediksiComboBox
            // 
            this.metodePrediksiComboBox.FormattingEnabled = true;
            this.metodePrediksiComboBox.Items.AddRange(new object[] {
            "Historis",
            "Std Deviasi"});
            this.metodePrediksiComboBox.Location = new System.Drawing.Point(113, 14);
            this.metodePrediksiComboBox.Name = "metodePrediksiComboBox";
            this.metodePrediksiComboBox.Size = new System.Drawing.Size(121, 21);
            this.metodePrediksiComboBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Metode Prediksi";
            // 
            // ForecastButton
            // 
            this.ForecastButton.Location = new System.Drawing.Point(889, 63);
            this.ForecastButton.Name = "ForecastButton";
            this.ForecastButton.Size = new System.Drawing.Size(78, 78);
            this.ForecastButton.TabIndex = 6;
            this.ForecastButton.Text = "Forecast";
            this.ForecastButton.UseVisualStyleBackColor = true;
            this.ForecastButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(973, 63);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(78, 78);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(396, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "FIT 100 (%)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(396, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "FIT 50 (%)";
            // 
            // fit100Num
            // 
            this.fit100Num.Location = new System.Drawing.Point(480, 76);
            this.fit100Num.Name = "fit100Num";
            this.fit100Num.Size = new System.Drawing.Size(120, 20);
            this.fit100Num.TabIndex = 13;
            this.fit100Num.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // fit50Num
            // 
            this.fit50Num.Location = new System.Drawing.Point(480, 111);
            this.fit50Num.Name = "fit50Num";
            this.fit50Num.Size = new System.Drawing.Size(120, 20);
            this.fit50Num.TabIndex = 13;
            this.fit50Num.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(652, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Adhoc 50";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(652, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Adhoc 100";
            // 
            // adhoc100Num
            // 
            this.adhoc100Num.Location = new System.Drawing.Point(738, 76);
            this.adhoc100Num.Maximum = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.adhoc100Num.Name = "adhoc100Num";
            this.adhoc100Num.Size = new System.Drawing.Size(120, 20);
            this.adhoc100Num.TabIndex = 15;
            this.adhoc100Num.ThousandsSeparator = true;
            // 
            // adhoc50Num
            // 
            this.adhoc50Num.Location = new System.Drawing.Point(738, 111);
            this.adhoc50Num.Maximum = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            0});
            this.adhoc50Num.Name = "adhoc50Num";
            this.adhoc50Num.Size = new System.Drawing.Size(120, 20);
            this.adhoc50Num.TabIndex = 15;
            this.adhoc50Num.ThousandsSeparator = true;
            // 
            // proyeksiLikuiditasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 607);
            this.Controls.Add(this.adhoc50Num);
            this.Controls.Add(this.adhoc100Num);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.fit50Num);
            this.Controls.Add(this.fit100Num);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.ForecastButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.metodePrediksiComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tanggalMaxPrediksiPicker);
            this.Controls.Add(this.bulanPrediksiTreeView);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.rasio50TextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rasio100TextBox);
            this.Name = "proyeksiLikuiditasForm";
            this.Text = "proyeksiLikuiditasForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fit100Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fit50Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.adhoc100Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.adhoc50Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox rasio100TextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox rasio50TextBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView bulanPrediksiTreeView;
        private System.Windows.Forms.DateTimePicker tanggalMaxPrediksiPicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox metodePrediksiComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ForecastButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown fit100Num;
        private System.Windows.Forms.NumericUpDown fit50Num;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown adhoc100Num;
        private System.Windows.Forms.NumericUpDown adhoc50Num;
    }
}