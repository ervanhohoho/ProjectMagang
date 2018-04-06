namespace testProjectBCA
{
    partial class RekapApproval
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
            this.startDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.endDatePicker = new System.Windows.Forms.DateTimePicker();
            this.loadBtn = new System.Windows.Forms.Button();
            this.realisasiGridView = new System.Windows.Forms.DataGridView();
            this.approvalGridView = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AkurasiForecastGridView = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.realisasiGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.approvalGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AkurasiForecastGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Date";
            // 
            // startDatePicker
            // 
            this.startDatePicker.Location = new System.Drawing.Point(9, 38);
            this.startDatePicker.Name = "startDatePicker";
            this.startDatePicker.Size = new System.Drawing.Size(200, 20);
            this.startDatePicker.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(506, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "PKT";
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(508, 38);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(121, 21);
            this.pktComboBox.TabIndex = 4;
            this.pktComboBox.SelectedIndexChanged += new System.EventHandler(this.pktComboBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.endDatePicker);
            this.groupBox1.Controls.Add(this.loadBtn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pktComboBox);
            this.groupBox1.Controls.Add(this.startDatePicker);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1098, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "End Date";
            // 
            // endDatePicker
            // 
            this.endDatePicker.Location = new System.Drawing.Point(247, 38);
            this.endDatePicker.Name = "endDatePicker";
            this.endDatePicker.Size = new System.Drawing.Size(200, 20);
            this.endDatePicker.TabIndex = 7;
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(676, 39);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(75, 23);
            this.loadBtn.TabIndex = 5;
            this.loadBtn.Text = "Load";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // realisasiGridView
            // 
            this.realisasiGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.realisasiGridView.Location = new System.Drawing.Point(12, 161);
            this.realisasiGridView.Name = "realisasiGridView";
            this.realisasiGridView.Size = new System.Drawing.Size(1098, 106);
            this.realisasiGridView.TabIndex = 6;
            // 
            // approvalGridView
            // 
            this.approvalGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.approvalGridView.Location = new System.Drawing.Point(12, 325);
            this.approvalGridView.Name = "approvalGridView";
            this.approvalGridView.Size = new System.Drawing.Size(1098, 106);
            this.approvalGridView.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Realisasi";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Approval";
            // 
            // AkurasiForecastGridView
            // 
            this.AkurasiForecastGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AkurasiForecastGridView.Location = new System.Drawing.Point(12, 485);
            this.AkurasiForecastGridView.Name = "AkurasiForecastGridView";
            this.AkurasiForecastGridView.Size = new System.Drawing.Size(1098, 109);
            this.AkurasiForecastGridView.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 461);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Akurasi Prediksi";
            // 
            // RekapApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 628);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.AkurasiForecastGridView);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.approvalGridView);
            this.Controls.Add(this.realisasiGridView);
            this.Controls.Add(this.groupBox1);
            this.Name = "RekapApproval";
            this.Text = "RekapApproval";
            this.Load += new System.EventHandler(this.RekapApproval_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.realisasiGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.approvalGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AkurasiForecastGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker startDatePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.DataGridView realisasiGridView;
        private System.Windows.Forms.DataGridView approvalGridView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker endDatePicker;
        private System.Windows.Forms.DataGridView AkurasiForecastGridView;
        private System.Windows.Forms.Label label6;
    }
}