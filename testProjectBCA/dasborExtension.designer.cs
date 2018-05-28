namespace testProjectBCA
{
    partial class dasborExtension
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
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.comboTahun = new System.Windows.Forms.ComboBox();
            this.comboKanwil = new System.Windows.Forms.ComboBox();
            this.comboBulan = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 65);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(249, 387);
            this.dataGridView1.TabIndex = 0;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(267, 65);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(249, 387);
            this.dataGridView2.TabIndex = 1;
            // 
            // comboTahun
            // 
            this.comboTahun.FormattingEnabled = true;
            this.comboTahun.Location = new System.Drawing.Point(13, 38);
            this.comboTahun.Name = "comboTahun";
            this.comboTahun.Size = new System.Drawing.Size(121, 21);
            this.comboTahun.TabIndex = 2;
            this.comboTahun.SelectionChangeCommitted += new System.EventHandler(this.comboTahun_SelectionChangeCommitted);
            // 
            // comboKanwil
            // 
            this.comboKanwil.FormattingEnabled = true;
            this.comboKanwil.Location = new System.Drawing.Point(13, 13);
            this.comboKanwil.Name = "comboKanwil";
            this.comboKanwil.Size = new System.Drawing.Size(121, 21);
            this.comboKanwil.TabIndex = 3;
            this.comboKanwil.SelectionChangeCommitted += new System.EventHandler(this.comboKanwil_SelectionChangeCommitted);
            // 
            // comboBulan
            // 
            this.comboBulan.FormattingEnabled = true;
            this.comboBulan.Location = new System.Drawing.Point(140, 38);
            this.comboBulan.Name = "comboBulan";
            this.comboBulan.Size = new System.Drawing.Size(121, 21);
            this.comboBulan.TabIndex = 4;
            this.comboBulan.SelectionChangeCommitted += new System.EventHandler(this.comboBulan_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(376, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Sislok ATM dan Sislok CRM";
            // 
            // dasborExtension
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 463);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBulan);
            this.Controls.Add(this.comboKanwil);
            this.Controls.Add(this.comboTahun);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Name = "dasborExtension";
            this.Text = "Extension ";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.ComboBox comboTahun;
        private System.Windows.Forms.ComboBox comboKanwil;
        private System.Windows.Forms.ComboBox comboBulan;
        private System.Windows.Forms.Label label1;
    }
}