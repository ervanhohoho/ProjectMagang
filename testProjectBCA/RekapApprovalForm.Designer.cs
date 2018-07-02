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
            this.button1 = new System.Windows.Forms.Button();
            this.tanggalMinPicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.InputGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // InputGridView
            // 
            this.InputGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGridView.Location = new System.Drawing.Point(12, 109);
            this.InputGridView.Name = "InputGridView";
            this.InputGridView.Size = new System.Drawing.Size(1326, 563);
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
            this.InputButton.Location = new System.Drawing.Point(1263, 694);
            this.InputButton.Name = "InputButton";
            this.InputButton.Size = new System.Drawing.Size(75, 23);
            this.InputButton.TabIndex = 2;
            this.InputButton.Text = "Save";
            this.InputButton.UseVisualStyleBackColor = true;
            this.InputButton.Click += new System.EventHandler(this.InputButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(1173, 694);
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
            this.kanwilCheckListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.kanwilCheckListBox_ItemCheck);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(402, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tanggalMinPicker
            // 
            this.tanggalMinPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tanggalMinPicker.Location = new System.Drawing.Point(653, 15);
            this.tanggalMinPicker.Name = "tanggalMinPicker";
            this.tanggalMinPicker.Size = new System.Drawing.Size(103, 20);
            this.tanggalMinPicker.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(581, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tanggal Min";
            // 
            // RekapApprovalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tanggalMinPicker);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.kanwilCheckListBox);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.InputButton);
            this.Controls.Add(this.TanggalPicker);
            this.Controls.Add(this.InputGridView);
            this.Name = "RekapApprovalForm";
            this.Text = "RekapApprovalForm";
            ((System.ComponentModel.ISupportInitialize)(this.InputGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView InputGridView;
        private System.Windows.Forms.DateTimePicker TanggalPicker;
        private System.Windows.Forms.Button InputButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.CheckedListBox kanwilCheckListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker tanggalMinPicker;
        private System.Windows.Forms.Label label1;
    }
}