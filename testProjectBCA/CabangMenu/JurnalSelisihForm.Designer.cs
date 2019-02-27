namespace testProjectBCA.CabangMenu
{
    partial class JurnalSelisihForm
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
            this.bulanComboBox = new System.Windows.Forms.ComboBox();
            this.tahunComboBox = new System.Windows.Forms.ComboBox();
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.loadBtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.exportBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // bulanComboBox
            // 
            this.bulanComboBox.FormattingEnabled = true;
            this.bulanComboBox.Location = new System.Drawing.Point(47, 12);
            this.bulanComboBox.Name = "bulanComboBox";
            this.bulanComboBox.Size = new System.Drawing.Size(121, 21);
            this.bulanComboBox.TabIndex = 0;
            this.bulanComboBox.SelectedIndexChanged += new System.EventHandler(this.bulanComboBox_SelectedIndexChanged);
            // 
            // tahunComboBox
            // 
            this.tahunComboBox.FormattingEnabled = true;
            this.tahunComboBox.Location = new System.Drawing.Point(227, 12);
            this.tahunComboBox.Name = "tahunComboBox";
            this.tahunComboBox.Size = new System.Drawing.Size(121, 21);
            this.tahunComboBox.TabIndex = 0;
            this.tahunComboBox.SelectionChangeCommitted += new System.EventHandler(this.tahunComboBox_SelectionChangeCommitted);
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(586, 12);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(121, 21);
            this.pktComboBox.TabIndex = 0;
            this.pktComboBox.SelectionChangeCommitted += new System.EventHandler(this.pktComboBox_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tahun";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Bulan";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(552, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PKT";
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(713, 10);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(75, 23);
            this.loadBtn.TabIndex = 3;
            this.loadBtn.Text = "Load";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 39);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(855, 399);
            this.dataGridView1.TabIndex = 4;
            // 
            // exportBtn
            // 
            this.exportBtn.Location = new System.Drawing.Point(794, 10);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBtn.TabIndex = 5;
            this.exportBtn.Text = "Export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // JurnalSelisihForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 450);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.loadBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pktComboBox);
            this.Controls.Add(this.tahunComboBox);
            this.Controls.Add(this.bulanComboBox);
            this.Name = "JurnalSelisihForm";
            this.Text = "JurnalSelisihForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox bulanComboBox;
        private System.Windows.Forms.ComboBox tahunComboBox;
        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button exportBtn;
    }
}