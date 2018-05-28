namespace testProjectBCA
{
    partial class ForecastAtmForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bulanPrediksiTreeView = new System.Windows.Forms.TreeView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.metodePenghitunganComboBox = new System.Windows.Forms.ComboBox();
            this.StartDatePicker = new System.Windows.Forms.DateTimePicker();
            this.EndDatePicker = new System.Windows.Forms.DateTimePicker();
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.yangDilihatComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PKT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Start Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(323, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "End Date";
            // 
            // bulanPrediksiTreeView
            // 
            this.bulanPrediksiTreeView.Location = new System.Drawing.Point(15, 70);
            this.bulanPrediksiTreeView.Name = "bulanPrediksiTreeView";
            this.bulanPrediksiTreeView.Size = new System.Drawing.Size(121, 97);
            this.bulanPrediksiTreeView.TabIndex = 2;
            this.bulanPrediksiTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.bulanPrediksiTreeView_AfterCheck);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Bulan Untuk Prediksi";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(501, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Metode Penghitungan";
            // 
            // metodePenghitunganComboBox
            // 
            this.metodePenghitunganComboBox.FormattingEnabled = true;
            this.metodePenghitunganComboBox.Items.AddRange(new object[] {
            "Historis",
            "Historis (Std Deviasi)"});
            this.metodePenghitunganComboBox.Location = new System.Drawing.Point(619, 14);
            this.metodePenghitunganComboBox.Name = "metodePenghitunganComboBox";
            this.metodePenghitunganComboBox.Size = new System.Drawing.Size(121, 21);
            this.metodePenghitunganComboBox.TabIndex = 5;
            // 
            // StartDatePicker
            // 
            this.StartDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.StartDatePicker.Location = new System.Drawing.Point(212, 15);
            this.StartDatePicker.Name = "StartDatePicker";
            this.StartDatePicker.Size = new System.Drawing.Size(102, 20);
            this.StartDatePicker.TabIndex = 6;
            // 
            // EndDatePicker
            // 
            this.EndDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.EndDatePicker.Location = new System.Drawing.Point(381, 15);
            this.EndDatePicker.Name = "EndDatePicker";
            this.EndDatePicker.Size = new System.Drawing.Size(102, 20);
            this.EndDatePicker.TabIndex = 6;
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(46, 14);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(99, 21);
            this.pktComboBox.TabIndex = 7;
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(768, 12);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 8;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 189);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(864, 287);
            this.dataGridView1.TabIndex = 9;
            // 
            // yangDilihatComboBox
            // 
            this.yangDilihatComboBox.FormattingEnabled = true;
            this.yangDilihatComboBox.Items.AddRange(new object[] {
            "Sislok ATM",
            "Sislok CRM",
            "Sislok CDM",
            "Isi ATM",
            "Isi CRM"});
            this.yangDilihatComboBox.Location = new System.Drawing.Point(157, 69);
            this.yangDilihatComboBox.Name = "yangDilihatComboBox";
            this.yangDilihatComboBox.Size = new System.Drawing.Size(121, 21);
            this.yangDilihatComboBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(154, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Yang Dilihat";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Bisque;
            this.label7.Location = new System.Drawing.Point(848, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "SUM";
            // 
            // ForecastAtmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 488);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.yangDilihatComboBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.pktComboBox);
            this.Controls.Add(this.EndDatePicker);
            this.Controls.Add(this.StartDatePicker);
            this.Controls.Add(this.metodePenghitunganComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bulanPrediksiTreeView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ForecastAtmForm";
            this.Text = "ForecastAtmForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView bulanPrediksiTreeView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox metodePenghitunganComboBox;
        private System.Windows.Forms.DateTimePicker StartDatePicker;
        private System.Windows.Forms.DateTimePicker EndDatePicker;
        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox yangDilihatComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}