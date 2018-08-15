namespace testProjectBCA
{
    partial class PembagianSaldoForm
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
            this.sisaGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.pembagianGridView = new System.Windows.Forms.DataGridView();
            this.sumberDanaGridView = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sisaGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pembagianGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sumberDanaGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // sisaGridView
            // 
            this.sisaGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sisaGridView.Location = new System.Drawing.Point(727, 275);
            this.sisaGridView.Name = "sisaGridView";
            this.sisaGridView.Size = new System.Drawing.Size(631, 224);
            this.sisaGridView.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(724, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kekurangan";
            // 
            // pembagianGridView
            // 
            this.pembagianGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pembagianGridView.Location = new System.Drawing.Point(12, 29);
            this.pembagianGridView.Name = "pembagianGridView";
            this.pembagianGridView.Size = new System.Drawing.Size(709, 470);
            this.pembagianGridView.TabIndex = 2;
            this.pembagianGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.pembagianGridView_CellEndEdit);
            this.pembagianGridView.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.pembagianGridView_DefaultValuesNeeded);
            this.pembagianGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.pembagianGridView_EditingControlShowing);
            // 
            // sumberDanaGridView
            // 
            this.sumberDanaGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sumberDanaGridView.Location = new System.Drawing.Point(727, 29);
            this.sumberDanaGridView.Name = "sumberDanaGridView";
            this.sumberDanaGridView.Size = new System.Drawing.Size(631, 224);
            this.sumberDanaGridView.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(724, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sumber Dana";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pembagian";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PembagianSaldoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1365, 525);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sumberDanaGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sisaGridView);
            this.Controls.Add(this.pembagianGridView);
            this.Name = "PembagianSaldoForm";
            this.Text = "PembagianSaldoForm";
            ((System.ComponentModel.ISupportInitialize)(this.sisaGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pembagianGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sumberDanaGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView sisaGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView pembagianGridView;
        private System.Windows.Forms.DataGridView sumberDanaGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}