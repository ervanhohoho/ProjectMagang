namespace testProjectBCA.NasabahMenu
{
    partial class ExportAbacasForm
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
            this.tahunCombo = new System.Windows.Forms.ComboBox();
            this.bulanCombo = new System.Windows.Forms.ComboBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tahunCombo
            // 
            this.tahunCombo.FormattingEnabled = true;
            this.tahunCombo.Location = new System.Drawing.Point(62, 12);
            this.tahunCombo.Name = "tahunCombo";
            this.tahunCombo.Size = new System.Drawing.Size(89, 21);
            this.tahunCombo.TabIndex = 0;
            this.tahunCombo.SelectedValueChanged += new System.EventHandler(this.tahunCombo_SelectedValueChanged);
            // 
            // bulanCombo
            // 
            this.bulanCombo.FormattingEnabled = true;
            this.bulanCombo.Location = new System.Drawing.Point(218, 12);
            this.bulanCombo.Name = "bulanCombo";
            this.bulanCombo.Size = new System.Drawing.Size(61, 21);
            this.bulanCombo.TabIndex = 0;
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(334, 12);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tahun";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(178, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bulan";
            // 
            // ExportAbacasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 50);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.bulanCombo);
            this.Controls.Add(this.tahunCombo);
            this.Name = "ExportAbacasForm";
            this.Text = "ExportAbacasForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox tahunCombo;
        private System.Windows.Forms.ComboBox bulanCombo;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}