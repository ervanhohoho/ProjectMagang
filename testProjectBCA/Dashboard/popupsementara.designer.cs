namespace testProjectBCA
{
    partial class popupsementara
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
            this.cartesianChart3 = new LiveCharts.WinForms.CartesianChart();
            this.comboBulan3 = new System.Windows.Forms.ComboBox();
            this.comboTahun3 = new System.Windows.Forms.ComboBox();
            this.comboArea = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cartesianChart3
            // 
            this.cartesianChart3.Location = new System.Drawing.Point(12, 12);
            this.cartesianChart3.Name = "cartesianChart3";
            this.cartesianChart3.Size = new System.Drawing.Size(1011, 421);
            this.cartesianChart3.TabIndex = 0;
            this.cartesianChart3.Text = "cartesianChart3";
            // 
            // comboBulan3
            // 
            this.comboBulan3.FormattingEnabled = true;
            this.comboBulan3.Location = new System.Drawing.Point(177, 439);
            this.comboBulan3.Name = "comboBulan3";
            this.comboBulan3.Size = new System.Drawing.Size(121, 21);
            this.comboBulan3.TabIndex = 18;
            this.comboBulan3.Text = "Bulan";
            this.comboBulan3.SelectionChangeCommitted += new System.EventHandler(this.comboBulan3_SelectionChangeCommitted);
            // 
            // comboTahun3
            // 
            this.comboTahun3.FormattingEnabled = true;
            this.comboTahun3.Location = new System.Drawing.Point(33, 439);
            this.comboTahun3.Name = "comboTahun3";
            this.comboTahun3.Size = new System.Drawing.Size(121, 21);
            this.comboTahun3.TabIndex = 17;
            this.comboTahun3.Text = "Tahun";
            this.comboTahun3.SelectionChangeCommitted += new System.EventHandler(this.comboTahun3_SelectionChangeCommitted);
            // 
            // comboArea
            // 
            this.comboArea.FormattingEnabled = true;
            this.comboArea.Location = new System.Drawing.Point(902, 439);
            this.comboArea.Name = "comboArea";
            this.comboArea.Size = new System.Drawing.Size(121, 21);
            this.comboArea.TabIndex = 20;
            this.comboArea.SelectionChangeCommitted += new System.EventHandler(this.comboArea_SelectionChangeCommitted);
            // 
            // popupsementara
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1052, 488);
            this.Controls.Add(this.comboArea);
            this.Controls.Add(this.comboBulan3);
            this.Controls.Add(this.comboTahun3);
            this.Controls.Add(this.cartesianChart3);
            this.Name = "popupsementara";
            this.Text = "popupsementara";
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart cartesianChart3;
        private System.Windows.Forms.ComboBox comboBulan3;
        private System.Windows.Forms.ComboBox comboTahun3;
        private System.Windows.Forms.ComboBox comboArea;
    }
}