namespace testProjectBCA
{
    partial class InputForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.getDataButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.sheetNameLbl = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.inputButton = new System.Windows.Forms.Button();
            this.tanggalLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // getDataButton
            // 
            this.getDataButton.Location = new System.Drawing.Point(12, 550);
            this.getDataButton.Name = "getDataButton";
            this.getDataButton.Size = new System.Drawing.Size(91, 32);
            this.getDataButton.TabIndex = 0;
            this.getDataButton.Text = "Get Data";
            this.getDataButton.UseVisualStyleBackColor = true;
            this.getDataButton.Click += new System.EventHandler(this.getDataButtonClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 39);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Size = new System.Drawing.Size(795, 505);
            this.dataGridView1.TabIndex = 1;
            // 
            // sheetNameLbl
            // 
            this.sheetNameLbl.AutoSize = true;
            this.sheetNameLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheetNameLbl.Location = new System.Drawing.Point(8, 8);
            this.sheetNameLbl.Name = "sheetNameLbl";
            this.sheetNameLbl.Size = new System.Drawing.Size(69, 24);
            this.sheetNameLbl.TabIndex = 2;
            this.sheetNameLbl.Text = "Sheet: ";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(75, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            // 
            // inputButton
            // 
            this.inputButton.Location = new System.Drawing.Point(109, 550);
            this.inputButton.Name = "inputButton";
            this.inputButton.Size = new System.Drawing.Size(87, 32);
            this.inputButton.TabIndex = 4;
            this.inputButton.Text = "Input";
            this.inputButton.UseVisualStyleBackColor = true;
            this.inputButton.Click += new System.EventHandler(this.inputButton_Click);
            // 
            // tanggalLbl
            // 
            this.tanggalLbl.AutoSize = true;
            this.tanggalLbl.Location = new System.Drawing.Point(667, 13);
            this.tanggalLbl.Name = "tanggalLbl";
            this.tanggalLbl.Size = new System.Drawing.Size(0, 13);
            this.tanggalLbl.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(218, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "PKT: ";
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(272, 10);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(121, 21);
            this.pktComboBox.TabIndex = 7;
            this.pktComboBox.SelectionChangeCommitted += new System.EventHandler(this.pktComboBox_SelectionChangeCommitted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(698, 553);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 32);
            this.button1.TabIndex = 8;
            this.button1.Text = "Select Parent Dir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 597);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pktComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tanggalLbl);
            this.Controls.Add(this.inputButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.sheetNameLbl);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.getDataButton);
            this.Name = "InputForm";
            this.Text = "InputForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button getDataButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label sheetNameLbl;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button inputButton;
        private System.Windows.Forms.Label tanggalLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.Button button1;
    }
}