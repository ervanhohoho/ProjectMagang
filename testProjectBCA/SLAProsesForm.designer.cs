namespace testProjectBCA
{
    partial class SLAProsesForm
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
            this.comboBulan1 = new System.Windows.Forms.ComboBox();
            this.comboTahun1 = new System.Windows.Forms.ComboBox();
            this.comboBulan2 = new System.Windows.Forms.ComboBox();
            this.comboTahun2 = new System.Windows.Forms.ComboBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.comboNamaPkt = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dataGridView1.Location = new System.Drawing.Point(12, 79);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(1110, 532);
            this.dataGridView1.TabIndex = 0;
            // 
            // comboBulan1
            // 
            this.comboBulan1.FormattingEnabled = true;
            this.comboBulan1.Location = new System.Drawing.Point(12, 44);
            this.comboBulan1.Name = "comboBulan1";
            this.comboBulan1.Size = new System.Drawing.Size(121, 21);
            this.comboBulan1.TabIndex = 1;
            // 
            // comboTahun1
            // 
            this.comboTahun1.FormattingEnabled = true;
            this.comboTahun1.Location = new System.Drawing.Point(139, 44);
            this.comboTahun1.Name = "comboTahun1";
            this.comboTahun1.Size = new System.Drawing.Size(121, 21);
            this.comboTahun1.TabIndex = 2;
            this.comboTahun1.SelectionChangeCommitted += new System.EventHandler(this.comboTahun1_SelectionChangeCommitted);
            // 
            // comboBulan2
            // 
            this.comboBulan2.FormattingEnabled = true;
            this.comboBulan2.Location = new System.Drawing.Point(325, 44);
            this.comboBulan2.Name = "comboBulan2";
            this.comboBulan2.Size = new System.Drawing.Size(121, 21);
            this.comboBulan2.TabIndex = 3;
            // 
            // comboTahun2
            // 
            this.comboTahun2.FormattingEnabled = true;
            this.comboTahun2.Location = new System.Drawing.Point(452, 44);
            this.comboTahun2.Name = "comboTahun2";
            this.comboTahun2.Size = new System.Drawing.Size(121, 21);
            this.comboTahun2.TabIndex = 4;
            this.comboTahun2.SelectionChangeCommitted += new System.EventHandler(this.comboTahun2_SelectionChangeCommitted);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(638, 44);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // comboNamaPkt
            // 
            this.comboNamaPkt.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboNamaPkt.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboNamaPkt.FormattingEnabled = true;
            this.comboNamaPkt.Location = new System.Drawing.Point(12, 12);
            this.comboNamaPkt.Name = "comboNamaPkt";
            this.comboNamaPkt.Size = new System.Drawing.Size(248, 21);
            this.comboNamaPkt.TabIndex = 6;
            this.comboNamaPkt.SelectionChangeCommitted += new System.EventHandler(this.comboNamaPkt_SelectionChangeCommitted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(733, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightGreen;
            this.label1.Location = new System.Drawing.Point(1005, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "SUM";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.label2.Location = new System.Drawing.Point(1079, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "AVG";
            // 
            // SLAProsesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 623);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboNamaPkt);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.comboTahun2);
            this.Controls.Add(this.comboBulan2);
            this.Controls.Add(this.comboTahun1);
            this.Controls.Add(this.comboBulan1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "SLAProsesForm";
            this.Text = "SLAProsesForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBulan1;
        private System.Windows.Forms.ComboBox comboTahun1;
        private System.Windows.Forms.ComboBox comboBulan2;
        private System.Windows.Forms.ComboBox comboTahun2;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ComboBox comboNamaPkt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}