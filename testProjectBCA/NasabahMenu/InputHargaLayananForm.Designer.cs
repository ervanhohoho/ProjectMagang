namespace testProjectBCA
{
    partial class InputHargaLayananForm
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
            this.stcGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cosGridView = new System.Windows.Forms.DataGridView();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tukaranGridView = new System.Windows.Forms.DataGridView();
            this.asuransiGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.stcGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cosGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tukaranGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.asuransiGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // stcGridView
            // 
            this.stcGridView.AllowUserToAddRows = false;
            this.stcGridView.AllowUserToDeleteRows = false;
            this.stcGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.stcGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stcGridView.Location = new System.Drawing.Point(12, 33);
            this.stcGridView.Name = "stcGridView";
            this.stcGridView.RowHeadersVisible = false;
            this.stcGridView.Size = new System.Drawing.Size(715, 182);
            this.stcGridView.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Said To Contain (STC)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Count On Site (COS)";
            // 
            // cosGridView
            // 
            this.cosGridView.AllowUserToAddRows = false;
            this.cosGridView.AllowUserToDeleteRows = false;
            this.cosGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.cosGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cosGridView.Location = new System.Drawing.Point(12, 243);
            this.cosGridView.Name = "cosGridView";
            this.cosGridView.RowHeadersVisible = false;
            this.cosGridView.Size = new System.Drawing.Size(715, 182);
            this.cosGridView.TabIndex = 2;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(652, 4);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 428);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Tukaran";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(389, 428);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Asuransi";
            // 
            // tukaranGridView
            // 
            this.tukaranGridView.AllowUserToAddRows = false;
            this.tukaranGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tukaranGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tukaranGridView.Location = new System.Drawing.Point(12, 444);
            this.tukaranGridView.Name = "tukaranGridView";
            this.tukaranGridView.RowHeadersVisible = false;
            this.tukaranGridView.Size = new System.Drawing.Size(342, 59);
            this.tukaranGridView.TabIndex = 7;
            this.tukaranGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.tukaranGridView_CellBeginEdit);
            this.tukaranGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.tukaranGridView_CellEndEdit);
            // 
            // asuransiGridView
            // 
            this.asuransiGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.asuransiGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.asuransiGridView.Location = new System.Drawing.Point(392, 444);
            this.asuransiGridView.Name = "asuransiGridView";
            this.asuransiGridView.Size = new System.Drawing.Size(335, 59);
            this.asuransiGridView.TabIndex = 8;
            this.asuransiGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.asuransiGridView_CellBeginEdit);
            this.asuransiGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.asuransiGridView_CellEndEdit);
            // 
            // InputHargaLayananForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 522);
            this.Controls.Add(this.asuransiGridView);
            this.Controls.Add(this.tukaranGridView);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cosGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stcGridView);
            this.Name = "InputHargaLayananForm";
            this.Text = "InputHargaRingForm";
            ((System.ComponentModel.ISupportInitialize)(this.stcGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cosGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tukaranGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.asuransiGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView stcGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView cosGridView;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView tukaranGridView;
        private System.Windows.Forms.DataGridView asuransiGridView;
    }
}