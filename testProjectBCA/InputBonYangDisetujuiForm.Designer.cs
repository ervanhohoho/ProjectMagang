namespace testProjectBCA
{
    partial class InputBonYangDisetujuiForm
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.bon100Num = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bon50Num = new System.Windows.Forms.NumericUpDown();
            this.bon20Num = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bon100Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bon50Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bon20Num)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(14, 25);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(121, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(14, 51);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // bon100Num
            // 
            this.bon100Num.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.bon100Num.Location = new System.Drawing.Point(112, 107);
            this.bon100Num.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.bon100Num.Name = "bon100Num";
            this.bon100Num.Size = new System.Drawing.Size(160, 20);
            this.bon100Num.TabIndex = 2;
            this.bon100Num.ThousandsSeparator = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bon 100";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Bon 50";
            // 
            // bon50Num
            // 
            this.bon50Num.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.bon50Num.Location = new System.Drawing.Point(112, 144);
            this.bon50Num.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.bon50Num.Name = "bon50Num";
            this.bon50Num.Size = new System.Drawing.Size(160, 20);
            this.bon50Num.TabIndex = 2;
            this.bon50Num.ThousandsSeparator = true;
            // 
            // bon20Num
            // 
            this.bon20Num.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.bon20Num.Location = new System.Drawing.Point(112, 184);
            this.bon20Num.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.bon20Num.Name = "bon20Num";
            this.bon20Num.Size = new System.Drawing.Size(160, 20);
            this.bon20Num.TabIndex = 2;
            this.bon20Num.ThousandsSeparator = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Bon 20";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(166, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Confirm";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // InputBonYangDisetujuiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bon20Num);
            this.Controls.Add(this.bon50Num);
            this.Controls.Add(this.bon100Num);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "InputBonYangDisetujuiForm";
            this.Text = "InputBonYangDisetujuiForm";
            ((System.ComponentModel.ISupportInitialize)(this.bon100Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bon50Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bon20Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.NumericUpDown bon100Num;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown bon50Num;
        private System.Windows.Forms.NumericUpDown bon20Num;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}