namespace testProjectBCA
{
    partial class PopupInformationBoard
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
            this.rasioChart = new LiveCharts.WinForms.CartesianChart();
            this.bulanCombo = new System.Windows.Forms.ComboBox();
            this.tahunCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.averageCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rasioChart
            // 
            this.rasioChart.Location = new System.Drawing.Point(12, 86);
            this.rasioChart.Name = "rasioChart";
            this.rasioChart.Size = new System.Drawing.Size(921, 281);
            this.rasioChart.TabIndex = 3;
            this.rasioChart.Text = "Rasio";
            // 
            // bulanCombo
            // 
            this.bulanCombo.FormattingEnabled = true;
            this.bulanCombo.Location = new System.Drawing.Point(81, 21);
            this.bulanCombo.Name = "bulanCombo";
            this.bulanCombo.Size = new System.Drawing.Size(121, 21);
            this.bulanCombo.TabIndex = 4;
            this.bulanCombo.SelectionChangeCommitted += new System.EventHandler(this.bulanCombo_SelectionChangeCommitted);
            this.bulanCombo.SelectedValueChanged += new System.EventHandler(this.bulanCombo_SelectedValueChanged);
            // 
            // tahunCombo
            // 
            this.tahunCombo.FormattingEnabled = true;
            this.tahunCombo.Location = new System.Drawing.Point(311, 21);
            this.tahunCombo.Name = "tahunCombo";
            this.tahunCombo.Size = new System.Drawing.Size(121, 21);
            this.tahunCombo.TabIndex = 5;
            this.tahunCombo.SelectionChangeCommitted += new System.EventHandler(this.tahunCombo_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Bulan";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Tahun";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Rasio";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(475, 19);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 9;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(556, 19);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 10;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // averageCheckBox
            // 
            this.averageCheckBox.AutoSize = true;
            this.averageCheckBox.Location = new System.Drawing.Point(661, 23);
            this.averageCheckBox.Name = "averageCheckBox";
            this.averageCheckBox.Size = new System.Drawing.Size(89, 17);
            this.averageCheckBox.TabIndex = 11;
            this.averageCheckBox.Text = "Average Line";
            this.averageCheckBox.UseVisualStyleBackColor = true;
            this.averageCheckBox.CheckedChanged += new System.EventHandler(this.averageCheckBox_CheckedChanged);
            this.averageCheckBox.CheckStateChanged += new System.EventHandler(this.averageCheckBox_CheckStateChanged);
            this.averageCheckBox.Click += new System.EventHandler(this.averageCheckBox_Click);
            // 
            // PopupInformationBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 379);
            this.Controls.Add(this.averageCheckBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tahunCombo);
            this.Controls.Add(this.bulanCombo);
            this.Controls.Add(this.rasioChart);
            this.Name = "PopupInformationBoard";
            this.Text = "PopupInformationBoard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private LiveCharts.WinForms.CartesianChart rasioChart;
        private System.Windows.Forms.ComboBox bulanCombo;
        private System.Windows.Forms.ComboBox tahunCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.CheckBox averageCheckBox;
    }
}