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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.dataGridView1.Location = new System.Drawing.Point(12, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(762, 398);
            this.dataGridView1.TabIndex = 1;
            // 
            // inputButtonMCS
            // 
            this.inputButtonMCS.Location = new System.Drawing.Point(138, 12);
            this.inputButtonMCS.Name = "inputButtonMCS";
            this.inputButtonMCS.Size = new System.Drawing.Size(116, 23);
            this.inputButtonMCS.TabIndex = 2;
            this.inputButtonMCS.Text = "Select File - MCS";
            this.inputButtonMCS.UseVisualStyleBackColor = true;
            this.inputButtonMCS.Click += new System.EventHandler(this.inputButtonMCS_Click);
            // 
            // ReadBeehiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 450);
            this.Controls.Add(this.inputButtonMCS);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.inputBtn);
            this.Name = "ReadBeehiveForm";
            this.Text = "ReadBeehiveForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button inputBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button inputButtonMCS;
    }
}