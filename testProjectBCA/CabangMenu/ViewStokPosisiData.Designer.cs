namespace testProjectBCA.CabangMenu
{
    partial class ViewStokPosisiData
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
            this.tanggalAwalPicker = new System.Windows.Forms.DateTimePicker();
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.denomComboBox = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.loadButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.jenisUangComboBox = new System.Windows.Forms.ComboBox();
            this.exportBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tanggalAkhirPicker = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Awal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pkt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(699, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Denom";
            // 
            // tanggalAwalPicker
            // 
            this.tanggalAwalPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tanggalAwalPicker.Location = new System.Drawing.Point(64, 9);
            this.tanggalAwalPicker.Name = "tanggalAwalPicker";
            this.tanggalAwalPicker.Size = new System.Drawing.Size(86, 20);
            this.tanggalAwalPicker.TabIndex = 2;
            this.tanggalAwalPicker.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(324, 8);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(178, 21);
            this.pktComboBox.TabIndex = 3;
            // 
            // denomComboBox
            // 
            this.denomComboBox.FormattingEnabled = true;
            this.denomComboBox.Location = new System.Drawing.Point(746, 8);
            this.denomComboBox.Name = "denomComboBox";
            this.denomComboBox.Size = new System.Drawing.Size(121, 21);
            this.denomComboBox.TabIndex = 4;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1016, 398);
            this.dataGridView1.TabIndex = 5;
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(873, 6);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 6;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(508, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Jenis Uang";
            // 
            // jenisUangComboBox
            // 
            this.jenisUangComboBox.FormattingEnabled = true;
            this.jenisUangComboBox.Location = new System.Drawing.Point(574, 8);
            this.jenisUangComboBox.Name = "jenisUangComboBox";
            this.jenisUangComboBox.Size = new System.Drawing.Size(121, 21);
            this.jenisUangComboBox.TabIndex = 4;
            this.jenisUangComboBox.SelectedValueChanged += new System.EventHandler(this.jenisUangComboBox_SelectedValueChanged);
            // 
            // exportBtn
            // 
            this.exportBtn.Location = new System.Drawing.Point(954, 6);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBtn.TabIndex = 7;
            this.exportBtn.Text = "Export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(155, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Akhir";
            // 
            // tanggalAkhirPicker
            // 
            this.tanggalAkhirPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tanggalAkhirPicker.Location = new System.Drawing.Point(192, 9);
            this.tanggalAkhirPicker.Name = "tanggalAkhirPicker";
            this.tanggalAkhirPicker.Size = new System.Drawing.Size(86, 20);
            this.tanggalAkhirPicker.TabIndex = 2;
            this.tanggalAkhirPicker.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // ViewStokPosisiData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 450);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.jenisUangComboBox);
            this.Controls.Add(this.denomComboBox);
            this.Controls.Add(this.pktComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tanggalAkhirPicker);
            this.Controls.Add(this.tanggalAwalPicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Name = "ViewStokPosisiData";
            this.Text = "ViewStokPosisiData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker tanggalAwalPicker;
        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.ComboBox denomComboBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox jenisUangComboBox;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker tanggalAkhirPicker;
    }
}