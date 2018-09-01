namespace testProjectBCA
{
    partial class ReadBeehiveForm
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
            this.inputBtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.inputButtonMCS = new System.Windows.Forms.Button();
            this.buttonProses = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.buttonSaveBeehive = new System.Windows.Forms.Button();
            this.buttonSaveMcs = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // inputBtn
            // 
            this.inputBtn.Location = new System.Drawing.Point(12, 12);
            this.inputBtn.Name = "inputBtn";
            this.inputBtn.Size = new System.Drawing.Size(120, 22);
            this.inputBtn.TabIndex = 0;
            this.inputBtn.Text = "Select File - Beehive";
            this.inputBtn.UseVisualStyleBackColor = true;
            this.inputBtn.Click += new System.EventHandler(this.inputBtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 74);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(379, 364);
            this.dataGridView1.TabIndex = 1;
            // 
            // inputButtonMCS
            // 
            this.inputButtonMCS.Location = new System.Drawing.Point(138, 12);
            this.inputButtonMCS.Name = "inputButtonMCS";
            this.inputButtonMCS.Size = new System.Drawing.Size(116, 22);
            this.inputButtonMCS.TabIndex = 2;
            this.inputButtonMCS.Text = "Select File - MCS";
            this.inputButtonMCS.UseVisualStyleBackColor = true;
            this.inputButtonMCS.Click += new System.EventHandler(this.inputButtonMCS_Click);
            // 
            // buttonProses
            // 
            this.buttonProses.Location = new System.Drawing.Point(650, 40);
            this.buttonProses.Name = "buttonProses";
            this.buttonProses.Size = new System.Drawing.Size(124, 23);
            this.buttonProses.TabIndex = 3;
            this.buttonProses.Text = "Proses";
            this.buttonProses.UseVisualStyleBackColor = true;
            this.buttonProses.Click += new System.EventHandler(this.buttonProses_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(397, 74);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(377, 364);
            this.dataGridView2.TabIndex = 4;
            // 
            // buttonSaveBeehive
            // 
            this.buttonSaveBeehive.Location = new System.Drawing.Point(12, 40);
            this.buttonSaveBeehive.Name = "buttonSaveBeehive";
            this.buttonSaveBeehive.Size = new System.Drawing.Size(120, 23);
            this.buttonSaveBeehive.TabIndex = 5;
            this.buttonSaveBeehive.Text = "save beehive";
            this.buttonSaveBeehive.UseVisualStyleBackColor = true;
            this.buttonSaveBeehive.Click += new System.EventHandler(this.buttonSaveBeehive_Click);
            // 
            // buttonSaveMcs
            // 
            this.buttonSaveMcs.Location = new System.Drawing.Point(138, 40);
            this.buttonSaveMcs.Name = "buttonSaveMcs";
            this.buttonSaveMcs.Size = new System.Drawing.Size(116, 23);
            this.buttonSaveMcs.TabIndex = 6;
            this.buttonSaveMcs.Text = "save mcs";
            this.buttonSaveMcs.UseVisualStyleBackColor = true;
            this.buttonSaveMcs.Click += new System.EventHandler(this.buttonSaveMcs_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(368, 12);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 7;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(574, 12);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 8;
            // 
            // ReadBeehiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 450);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.buttonSaveMcs);
            this.Controls.Add(this.buttonSaveBeehive);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.buttonProses);
            this.Controls.Add(this.inputButtonMCS);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.inputBtn);
            this.Name = "ReadBeehiveForm";
            this.Text = "ReadBeehiveForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button inputBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button inputButtonMCS;
        private System.Windows.Forms.Button buttonProses;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button buttonSaveBeehive;
        private System.Windows.Forms.Button buttonSaveMcs;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
    }
}