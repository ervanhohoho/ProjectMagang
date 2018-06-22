namespace testProjectBCA
{
    partial class RekapApprovalForm
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
            this.InputGridView = new System.Windows.Forms.DataGridView();
            this.TanggalPicker = new System.Windows.Forms.DateTimePicker();
            this.InputButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.kanwilCheckListBox = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.InputGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // InputGridView
            // 
            this.InputGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGridView.Location = new System.Drawing.Point(12, 91);
            this.InputGridView.Name = "InputGridView";
            this.InputGridView.Size = new System.Drawing.Size(959, 428);
            this.InputGridView.TabIndex = 0;
            this.InputGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGridView_CellEndEdit);
            // 
            // TanggalPicker
            // 
            this.TanggalPicker.Location = new System.Drawing.Point(12, 12);
            this.TanggalPicker.Name = "TanggalPicker";
            this.TanggalPicker.Size = new System.Drawing.Size(200, 20);
            this.TanggalPicker.TabIndex = 1;
            this.TanggalPicker.ValueChanged += new System.EventHandler(this.TanggalPicker_ValueChanged);
            // 
            // InputButton
            // 
            this.InputButton.Location = new System.Drawing.Point(896, 549);
            this.InputButton.Name = "InputButton";
            this.InputButton.Size = new System.Drawing.Size(75, 23);
            this.InputButton.TabIndex = 2;
            this.InputButton.Text = "Save";
            this.InputButton.UseVisualStyleBackColor = true;
            this.InputButton.Click += new System.EventHandler(this.InputButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(815, 549);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(75, 23);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // kanwilCheckListBox
            // 
            this.kanwilCheckListBox.FormattingEnabled = true;
            this.kanwilCheckListBox.Location = new System.Drawing.Point(242, 12);
            this.kanwilCheckListBox.Name = "kanwilCheckListBox";
            this.kanwilCheckListBox.Size = new System.Drawing.Size(133, 64);
            this.kanwilCheckListBox.TabIndex = 4;
            // 
            // RekapApprovalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(983, 584);
            this.Controls.Add(this.kanwilCheckListBox);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.InputButton);
            this.Controls.Add(this.TanggalPicker);
            this.Controls.Add(this.InputGridView);
            this.Name = "RekapApprovalForm";
            this.Text = "RekapApprovalForm";
            ((System.ComponentModel.ISupportInitialize)(this.InputGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView InputGridView;
        private System.Windows.Forms.DateTimePicker TanggalPicker;
        private System.Windows.Forms.Button InputButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.CheckedListBox kanwilCheckListBox;
    }
}