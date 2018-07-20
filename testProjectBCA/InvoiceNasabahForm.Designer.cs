namespace testProjectBCA
{
    partial class InvoiceNasabahForm
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
            this.exportButton = new System.Windows.Forms.Button();
            this.bulanComboBox = new System.Windows.Forms.ComboBox();
            this.tahunComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxCosNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.maxTukaranKertasNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.maxTukaranKoinMax = new System.Windows.Forms.NumericUpDown();
            this.loadButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxCosNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTukaranKertasNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTukaranKoinMax)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1109, 418);
            this.dataGridView1.TabIndex = 0;
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(1046, 12);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // bulanComboBox
            // 
            this.bulanComboBox.FormattingEnabled = true;
            this.bulanComboBox.Location = new System.Drawing.Point(223, 14);
            this.bulanComboBox.Name = "bulanComboBox";
            this.bulanComboBox.Size = new System.Drawing.Size(47, 21);
            this.bulanComboBox.TabIndex = 2;
            this.bulanComboBox.SelectionChangeCommitted += new System.EventHandler(this.bulanComboBox_SelectionChangeCommitted);
            // 
            // tahunComboBox
            // 
            this.tahunComboBox.FormattingEnabled = true;
            this.tahunComboBox.Location = new System.Drawing.Point(56, 14);
            this.tahunComboBox.Name = "tahunComboBox";
            this.tahunComboBox.Size = new System.Drawing.Size(121, 21);
            this.tahunComboBox.TabIndex = 3;
            this.tahunComboBox.SelectionChangeCommitted += new System.EventHandler(this.tahunComboBox_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Bulan";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tahun";
            // 
            // maxCosNumeric
            // 
            this.maxCosNumeric.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maxCosNumeric.Location = new System.Drawing.Point(348, 15);
            this.maxCosNumeric.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
            this.maxCosNumeric.Name = "maxCosNumeric";
            this.maxCosNumeric.Size = new System.Drawing.Size(120, 20);
            this.maxCosNumeric.TabIndex = 6;
            this.maxCosNumeric.ThousandsSeparator = true;
            this.maxCosNumeric.Value = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Max COS";
            // 
            // maxTukaranKertasNumeric
            // 
            this.maxTukaranKertasNumeric.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maxTukaranKertasNumeric.Location = new System.Drawing.Point(583, 15);
            this.maxTukaranKertasNumeric.Maximum = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.maxTukaranKertasNumeric.Name = "maxTukaranKertasNumeric";
            this.maxTukaranKertasNumeric.Size = new System.Drawing.Size(120, 20);
            this.maxTukaranKertasNumeric.TabIndex = 8;
            this.maxTukaranKertasNumeric.ThousandsSeparator = true;
            this.maxTukaranKertasNumeric.Value = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(474, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Max Tukaran Kertas";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(721, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Max Tukaran Koin";
            // 
            // maxTukaranKoinMax
            // 
            this.maxTukaranKoinMax.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maxTukaranKoinMax.Location = new System.Drawing.Point(821, 15);
            this.maxTukaranKoinMax.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.maxTukaranKoinMax.Name = "maxTukaranKoinMax";
            this.maxTukaranKoinMax.Size = new System.Drawing.Size(120, 20);
            this.maxTukaranKoinMax.TabIndex = 8;
            this.maxTukaranKoinMax.ThousandsSeparator = true;
            this.maxTukaranKoinMax.Value = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(965, 12);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 11;
            this.loadButton.Text = "Load Data";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // InvoiceNasabahForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 479);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxTukaranKoinMax);
            this.Controls.Add(this.maxTukaranKertasNumeric);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.maxCosNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tahunComboBox);
            this.Controls.Add(this.bulanComboBox);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.dataGridView1);
            this.Name = "InvoiceNasabahForm";
            this.Text = "InvoiceNasabahForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxCosNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTukaranKertasNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTukaranKoinMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.ComboBox bulanComboBox;
        private System.Windows.Forms.ComboBox tahunComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown maxCosNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown maxTukaranKertasNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown maxTukaranKoinMax;
        private System.Windows.Forms.Button loadButton;
    }
}