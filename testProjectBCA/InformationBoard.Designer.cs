namespace testProjectBCA
{
    partial class InformationBoard
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
            this.pktComboBox = new System.Windows.Forms.ComboBox();
            this.denom100Label = new System.Windows.Forms.Label();
            this.denom50Label = new System.Windows.Forms.Label();
            this.denom20Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pktComboBox
            // 
            this.pktComboBox.FormattingEnabled = true;
            this.pktComboBox.Location = new System.Drawing.Point(349, 12);
            this.pktComboBox.Name = "pktComboBox";
            this.pktComboBox.Size = new System.Drawing.Size(121, 21);
            this.pktComboBox.TabIndex = 0;
            this.pktComboBox.SelectionChangeCommitted += new System.EventHandler(this.pktComboBox_SelectionChangeCommitted);
            // 
            // denom100Label
            // 
            this.denom100Label.AutoSize = true;
            this.denom100Label.Location = new System.Drawing.Point(9, 16);
            this.denom100Label.Name = "denom100Label";
            this.denom100Label.Size = new System.Drawing.Size(35, 13);
            this.denom100Label.TabIndex = 1;
            this.denom100Label.Text = "label1";
            // 
            // denom50Label
            // 
            this.denom50Label.AutoSize = true;
            this.denom50Label.Location = new System.Drawing.Point(9, 47);
            this.denom50Label.Name = "denom50Label";
            this.denom50Label.Size = new System.Drawing.Size(35, 13);
            this.denom50Label.TabIndex = 2;
            this.denom50Label.Text = "label2";
            // 
            // denom20Label
            // 
            this.denom20Label.AutoSize = true;
            this.denom20Label.Location = new System.Drawing.Point(9, 78);
            this.denom20Label.Name = "denom20Label";
            this.denom20Label.Size = new System.Drawing.Size(35, 13);
            this.denom20Label.TabIndex = 3;
            this.denom20Label.Text = "label3";
            // 
            // InformationBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 322);
            this.Controls.Add(this.denom20Label);
            this.Controls.Add(this.denom50Label);
            this.Controls.Add(this.denom100Label);
            this.Controls.Add(this.pktComboBox);
            this.Name = "InformationBoard";
            this.Text = "Information Board";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox pktComboBox;
        private System.Windows.Forms.Label denom100Label;
        private System.Windows.Forms.Label denom50Label;
        private System.Windows.Forms.Label denom20Label;
    }
}