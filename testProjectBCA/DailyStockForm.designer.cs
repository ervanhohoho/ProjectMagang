namespace testProjectBCA
{
    partial class DailyStockForm
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
            this.comboPkt = new System.Windows.Forms.ComboBox();
            this.comboTahun = new System.Windows.Forms.ComboBox();
            this.comboBulan = new System.Windows.Forms.ComboBox();
            this.comboBulan2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboOption = new System.Windows.Forms.ComboBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.comboJenisTampilan = new System.Windows.Forms.ComboBox();
            this.buttonShow = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 38);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(957, 340);
            this.dataGridView1.TabIndex = 0;
            // 
            // comboPkt
            // 
            this.comboPkt.FormattingEnabled = true;
            this.comboPkt.Location = new System.Drawing.Point(12, 12);
            this.comboPkt.Name = "comboPkt";
            this.comboPkt.Size = new System.Drawing.Size(121, 21);
            this.comboPkt.TabIndex = 1;
            this.comboPkt.Text = "pkt";
            // 
            // comboTahun
            // 
            this.comboTahun.FormattingEnabled = true;
            this.comboTahun.Location = new System.Drawing.Point(309, 12);
            this.comboTahun.Name = "comboTahun";
            this.comboTahun.Size = new System.Drawing.Size(121, 21);
            this.comboTahun.TabIndex = 2;
            this.comboTahun.Text = "tahun";
            this.comboTahun.SelectionChangeCommitted += new System.EventHandler(this.comboTahun_SelectionChangeCommitted);
            // 
            // comboBulan
            // 
            this.comboBulan.FormattingEnabled = true;
            this.comboBulan.Location = new System.Drawing.Point(436, 12);
            this.comboBulan.Name = "comboBulan";
            this.comboBulan.Size = new System.Drawing.Size(121, 21);
            this.comboBulan.TabIndex = 3;
            this.comboBulan.Text = "bulan";
            // 
            // comboBulan2
            // 
            this.comboBulan2.FormattingEnabled = true;
            this.comboBulan2.Location = new System.Drawing.Point(592, 12);
            this.comboBulan2.Name = "comboBulan2";
            this.comboBulan2.Size = new System.Drawing.Size(121, 21);
            this.comboBulan2.TabIndex = 4;
            this.comboBulan2.Text = "bulan";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(563, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "s/d";
            // 
            // comboOption
            // 
            this.comboOption.FormattingEnabled = true;
            this.comboOption.Items.AddRange(new object[] {
            "Nominal",
            "Pieces"});
            this.comboOption.Location = new System.Drawing.Point(848, 12);
            this.comboOption.Name = "comboOption";
            this.comboOption.Size = new System.Drawing.Size(121, 21);
            this.comboOption.TabIndex = 6;
            this.comboOption.Text = "nom / pcs";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Coll. Cabang Process",
            "Coll. Cabang Pass Trough",
            "Coll. Retail",
            "Coll. Curex",
            "Coll. ATM Reguler",
            "Coll. ATM Adhoc",
            "Coll. BI",
            "Coll. Interbank",
            "Coll. Antar CPC"});
            this.checkedListBox1.Location = new System.Drawing.Point(139, 384);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(160, 139);
            this.checkedListBox1.TabIndex = 7;
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Items.AddRange(new object[] {
            "Del. Cabang Reguler",
            "Del. Cabang Adhoc",
            "Del. Retail",
            "Del. Curex",
            "Del. ATM Reguler",
            "Del. ATM Adhoc",
            "Del. BI",
            "Del. Interbank",
            "Del. Antar CPC"});
            this.checkedListBox2.Location = new System.Drawing.Point(305, 384);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(164, 139);
            this.checkedListBox2.TabIndex = 8;
            // 
            // comboJenisTampilan
            // 
            this.comboJenisTampilan.FormattingEnabled = true;
            this.comboJenisTampilan.Items.AddRange(new object[] {
            "Total Inflow",
            "Total Outflow",
            "Choose items.."});
            this.comboJenisTampilan.Location = new System.Drawing.Point(12, 384);
            this.comboJenisTampilan.Name = "comboJenisTampilan";
            this.comboJenisTampilan.Size = new System.Drawing.Size(121, 21);
            this.comboJenisTampilan.TabIndex = 9;
            this.comboJenisTampilan.SelectionChangeCommitted += new System.EventHandler(this.comboJenisTampilan_SelectionChangeCommitted);
            // 
            // buttonShow
            // 
            this.buttonShow.Location = new System.Drawing.Point(12, 411);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(75, 23);
            this.buttonShow.TabIndex = 10;
            this.buttonShow.Text = "Show";
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(12, 440);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(75, 23);
            this.buttonExport.TabIndex = 11;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // DailyStockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 591);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonShow);
            this.Controls.Add(this.comboJenisTampilan);
            this.Controls.Add(this.checkedListBox2);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.comboOption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBulan2);
            this.Controls.Add(this.comboBulan);
            this.Controls.Add(this.comboTahun);
            this.Controls.Add(this.comboPkt);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DailyStockForm";
            this.Text = "dailyStock";
            this.Load += new System.EventHandler(this.DailyStockForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboPkt;
        private System.Windows.Forms.ComboBox comboTahun;
        private System.Windows.Forms.ComboBox comboBulan;
        private System.Windows.Forms.ComboBox comboBulan2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboOption;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.ComboBox comboJenisTampilan;
        private System.Windows.Forms.Button buttonShow;
        private System.Windows.Forms.Button buttonExport;
    }
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>

        #endregion
    
}