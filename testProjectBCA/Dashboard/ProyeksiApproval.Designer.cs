namespace testProjectBCA.Dashboard
{
    partial class ProyeksiApproval
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
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.kanwilComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tanggalApprovalPicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Location = new System.Drawing.Point(12, 61);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(776, 366);
            this.cartesianChart1.TabIndex = 0;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // kanwilComboBox
            // 
            this.kanwilComboBox.FormattingEnabled = true;
            this.kanwilComboBox.Location = new System.Drawing.Point(284, 15);
            this.kanwilComboBox.Name = "kanwilComboBox";
            this.kanwilComboBox.Size = new System.Drawing.Size(121, 21);
            this.kanwilComboBox.TabIndex = 2;
            this.kanwilComboBox.SelectedValueChanged += new System.EventHandler(this.kanwilComboBox_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(243, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Kanwil";
            // 
            // tanggalApprovalPicker
            // 
            this.tanggalApprovalPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tanggalApprovalPicker.Location = new System.Drawing.Point(117, 15);
            this.tanggalApprovalPicker.Name = "tanggalApprovalPicker";
            this.tanggalApprovalPicker.Size = new System.Drawing.Size(98, 20);
            this.tanggalApprovalPicker.TabIndex = 4;
            this.tanggalApprovalPicker.ValueChanged += new System.EventHandler(this.tanggalApprovalPicker_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tanggal Approval";
            // 
            // ProyeksiApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tanggalApprovalPicker);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.kanwilComboBox);
            this.Controls.Add(this.cartesianChart1);
            this.Name = "ProyeksiApproval";
            this.Text = "ProyeksiApproval";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart cartesianChart1;
        private System.Windows.Forms.ComboBox kanwilComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker tanggalApprovalPicker;
        private System.Windows.Forms.Label label2;
    }
}