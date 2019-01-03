namespace testProjectBCA.ATM
{
    partial class ProyeksiATMForm
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
            this.LoadBtn = new System.Windows.Forms.Button();
            this.EndDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.MetodePenghitunganCombo = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.KodePktCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label5 = new System.Windows.Forms.Label();
            this.e2eComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.rasio100Num = new System.Windows.Forms.NumericUpDown();
            this.rasio50Num = new System.Windows.Forms.NumericUpDown();
            this.rasio20Num = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio100Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio50Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio20Num)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadBtn
            // 
            this.LoadBtn.Location = new System.Drawing.Point(126, 138);
            this.LoadBtn.Name = "LoadBtn";
            this.LoadBtn.Size = new System.Drawing.Size(339, 23);
            this.LoadBtn.TabIndex = 0;
            this.LoadBtn.Text = "Load";
            this.LoadBtn.UseVisualStyleBackColor = true;
            this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
            // 
            // EndDatePicker
            // 
            this.EndDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.EndDatePicker.Location = new System.Drawing.Point(253, 28);
            this.EndDatePicker.Name = "EndDatePicker";
            this.EndDatePicker.Size = new System.Drawing.Size(212, 20);
            this.EndDatePicker.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(126, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tanggal";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Metode Penghitungan";
            // 
            // MetodePenghitunganCombo
            // 
            this.MetodePenghitunganCombo.FormattingEnabled = true;
            this.MetodePenghitunganCombo.Location = new System.Drawing.Point(253, 54);
            this.MetodePenghitunganCombo.Name = "MetodePenghitunganCombo";
            this.MetodePenghitunganCombo.Size = new System.Drawing.Size(212, 21);
            this.MetodePenghitunganCombo.TabIndex = 5;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 182);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(713, 381);
            this.dataGridView1.TabIndex = 6;
            // 
            // KodePktCombo
            // 
            this.KodePktCombo.FormattingEnabled = true;
            this.KodePktCombo.Location = new System.Drawing.Point(253, 82);
            this.KodePktCombo.Name = "KodePktCombo";
            this.KodePktCombo.Size = new System.Drawing.Size(212, 21);
            this.KodePktCombo.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "PKT";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(14, 9);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(106, 13);
            this.label20.TabIndex = 25;
            this.label20.Text = "Bulan Untuk Prediksi";
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(15, 26);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(105, 150);
            this.treeView1.TabIndex = 24;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.ParentChanged += new System.EventHandler(this.treeView1_ParentChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(126, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Jenis E2E";
            // 
            // e2eComboBox
            // 
            this.e2eComboBox.FormattingEnabled = true;
            this.e2eComboBox.Items.AddRange(new object[] {
            "E2E",
            "Non E2E"});
            this.e2eComboBox.Location = new System.Drawing.Point(253, 111);
            this.e2eComboBox.Name = "e2eComboBox";
            this.e2eComboBox.Size = new System.Drawing.Size(212, 21);
            this.e2eComboBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(506, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Rasio 100";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(506, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Rasio 50";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(506, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Rasio 20";
            // 
            // rasio100Num
            // 
            this.rasio100Num.Location = new System.Drawing.Point(581, 54);
            this.rasio100Num.Name = "rasio100Num";
            this.rasio100Num.Size = new System.Drawing.Size(120, 20);
            this.rasio100Num.TabIndex = 28;
            // 
            // rasio50Num
            // 
            this.rasio50Num.Location = new System.Drawing.Point(581, 83);
            this.rasio50Num.Name = "rasio50Num";
            this.rasio50Num.Size = new System.Drawing.Size(120, 20);
            this.rasio50Num.TabIndex = 28;
            // 
            // rasio20Num
            // 
            this.rasio20Num.Location = new System.Drawing.Point(581, 112);
            this.rasio20Num.Name = "rasio20Num";
            this.rasio20Num.Size = new System.Drawing.Size(120, 20);
            this.rasio20Num.TabIndex = 28;
            // 
            // ProyeksiATMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 575);
            this.Controls.Add(this.rasio20Num);
            this.Controls.Add(this.rasio50Num);
            this.Controls.Add(this.rasio100Num);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.e2eComboBox);
            this.Controls.Add(this.KodePktCombo);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.MetodePenghitunganCombo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EndDatePicker);
            this.Controls.Add(this.LoadBtn);
            this.Name = "ProyeksiATMForm";
            this.Text = "Proyeksi ATM";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio100Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio50Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rasio20Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadBtn;
        private System.Windows.Forms.DateTimePicker EndDatePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox MetodePenghitunganCombo;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox KodePktCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox e2eComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown rasio100Num;
        private System.Windows.Forms.NumericUpDown rasio50Num;
        private System.Windows.Forms.NumericUpDown rasio20Num;
    }
}