namespace testProjectBCA.CabangMenu
{
    partial class DetailTransaksiVault
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboPkt = new System.Windows.Forms.ComboBox();
            this.comboVal = new System.Windows.Forms.ComboBox();
            this.comboSetBon = new System.Windows.Forms.ComboBox();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.exportBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 100);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(575, 484);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboPkt);
            this.groupBox1.Controls.Add(this.comboVal);
            this.groupBox1.Controls.Add(this.comboSetBon);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(425, 81);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // comboPkt
            // 
            this.comboPkt.FormattingEnabled = true;
            this.comboPkt.Location = new System.Drawing.Point(219, 46);
            this.comboPkt.Name = "comboPkt";
            this.comboPkt.Size = new System.Drawing.Size(200, 21);
            this.comboPkt.TabIndex = 6;
            this.comboPkt.SelectionChangeCommitted += new System.EventHandler(this.comboPkt_SelectionChangeCommitted);
            // 
            // comboVal
            // 
            this.comboVal.FormattingEnabled = true;
            this.comboVal.Items.AddRange(new object[] {
            "Sudah Validasi",
            "Belum Validasi"});
            this.comboVal.Location = new System.Drawing.Point(6, 46);
            this.comboVal.Name = "comboVal";
            this.comboVal.Size = new System.Drawing.Size(207, 21);
            this.comboVal.TabIndex = 4;
            this.comboVal.SelectionChangeCommitted += new System.EventHandler(this.comboVal_SelectionChangeCommitted);
            // 
            // comboSetBon
            // 
            this.comboSetBon.FormattingEnabled = true;
            this.comboSetBon.Items.AddRange(new object[] {
            "Out",
            "In"});
            this.comboSetBon.Location = new System.Drawing.Point(6, 19);
            this.comboSetBon.Name = "comboSetBon";
            this.comboSetBon.Size = new System.Drawing.Size(207, 21);
            this.comboSetBon.TabIndex = 3;
            this.comboSetBon.SelectionChangeCommitted += new System.EventHandler(this.comboSetBon_SelectionChangeCommitted);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(219, 20);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 1;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // exportBtn
            // 
            this.exportBtn.Location = new System.Drawing.Point(443, 32);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(144, 61);
            this.exportBtn.TabIndex = 9;
            this.exportBtn.Text = "Export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // DetailTransaksiVault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 596);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Name = "DetailTransaksiVault";
            this.Text = "DetailTransaksiVault";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboPkt;
        private System.Windows.Forms.ComboBox comboVal;
        private System.Windows.Forms.ComboBox comboSetBon;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button exportBtn;
    }
}