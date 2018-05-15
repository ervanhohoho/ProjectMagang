namespace testProjectBCA
{
    partial class DashboardCOJForm
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
            this.pieChartStokSaldoCoj = new LiveCharts.WinForms.PieChart();
            this.comboTahun = new System.Windows.Forms.ComboBox();
            this.comboBulan = new System.Windows.Forms.ComboBox();
            this.comboTanggal = new System.Windows.Forms.ComboBox();
            this.pieChartUbVsUk = new LiveCharts.WinForms.PieChart();
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.SuspendLayout();
            // 
            // pieChartStokSaldoCoj
            // 
            this.pieChartStokSaldoCoj.Location = new System.Drawing.Point(12, 93);
            this.pieChartStokSaldoCoj.Name = "pieChartStokSaldoCoj";
            this.pieChartStokSaldoCoj.Size = new System.Drawing.Size(544, 379);
            this.pieChartStokSaldoCoj.TabIndex = 0;
            this.pieChartStokSaldoCoj.Text = "pieChart1";
            // 
            // comboTahun
            // 
            this.comboTahun.FormattingEnabled = true;
            this.comboTahun.Location = new System.Drawing.Point(12, 12);
            this.comboTahun.Name = "comboTahun";
            this.comboTahun.Size = new System.Drawing.Size(121, 21);
            this.comboTahun.TabIndex = 1;
            this.comboTahun.SelectionChangeCommitted += new System.EventHandler(this.comboTahun_SelectionChangeCommitted);
            // 
            // comboBulan
            // 
            this.comboBulan.FormattingEnabled = true;
            this.comboBulan.Location = new System.Drawing.Point(12, 39);
            this.comboBulan.Name = "comboBulan";
            this.comboBulan.Size = new System.Drawing.Size(121, 21);
            this.comboBulan.TabIndex = 2;
            this.comboBulan.SelectionChangeCommitted += new System.EventHandler(this.comboBulan_SelectionChangeCommitted);
            // 
            // comboTanggal
            // 
            this.comboTanggal.FormattingEnabled = true;
            this.comboTanggal.Location = new System.Drawing.Point(12, 66);
            this.comboTanggal.Name = "comboTanggal";
            this.comboTanggal.Size = new System.Drawing.Size(121, 21);
            this.comboTanggal.TabIndex = 3;
            this.comboTanggal.SelectionChangeCommitted += new System.EventHandler(this.comboTanggal_SelectionChangeCommitted);
            // 
            // pieChartUbVsUk
            // 
            this.pieChartUbVsUk.Location = new System.Drawing.Point(12, 478);
            this.pieChartUbVsUk.Name = "pieChartUbVsUk";
            this.pieChartUbVsUk.Size = new System.Drawing.Size(544, 379);
            this.pieChartUbVsUk.TabIndex = 4;
            this.pieChartUbVsUk.Text = "pieChart1";
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Location = new System.Drawing.Point(562, 93);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(544, 379);
            this.cartesianChart1.TabIndex = 5;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // DashboardCOJForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1105, 781);
            this.Controls.Add(this.cartesianChart1);
            this.Controls.Add(this.pieChartUbVsUk);
            this.Controls.Add(this.comboTanggal);
            this.Controls.Add(this.comboBulan);
            this.Controls.Add(this.comboTahun);
            this.Controls.Add(this.pieChartStokSaldoCoj);
            this.Name = "DashboardCOJForm";
            this.Text = "DashboardCOJForm";
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.PieChart pieChartStokSaldoCoj;
        private System.Windows.Forms.ComboBox comboTahun;
        private System.Windows.Forms.ComboBox comboBulan;
        private System.Windows.Forms.ComboBox comboTanggal;
        private LiveCharts.WinForms.PieChart pieChartUbVsUk;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
    }
}