namespace testProjectBCA
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aTMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revisiDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputOptiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rekapApprovalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cashpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputDataPktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputDataDenomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aTMToolStripMenuItem,
            this.cashpointToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1004, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aTMToolStripMenuItem
            // 
            this.aTMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inputDataToolStripMenuItem,
            this.revisiDataToolStripMenuItem,
            this.inputOptiToolStripMenuItem,
            this.informationBoardToolStripMenuItem,
            this.rekapApprovalToolStripMenuItem});
            this.aTMToolStripMenuItem.Name = "aTMToolStripMenuItem";
            this.aTMToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aTMToolStripMenuItem.Text = "ATM";
            // 
            // inputDataToolStripMenuItem
            // 
            this.inputDataToolStripMenuItem.Name = "inputDataToolStripMenuItem";
            this.inputDataToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.inputDataToolStripMenuItem.Text = "Input Data";
            this.inputDataToolStripMenuItem.Click += new System.EventHandler(this.inputDataToolStripMenuItem_Click);
            // 
            // revisiDataToolStripMenuItem
            // 
            this.revisiDataToolStripMenuItem.Name = "revisiDataToolStripMenuItem";
            this.revisiDataToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.revisiDataToolStripMenuItem.Text = "Revisi Data";
            this.revisiDataToolStripMenuItem.Click += new System.EventHandler(this.revisiDataToolStripMenuItem_Click);
            // 
            // inputOptiToolStripMenuItem
            // 
            this.inputOptiToolStripMenuItem.Name = "inputOptiToolStripMenuItem";
            this.inputOptiToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.inputOptiToolStripMenuItem.Text = "Input OptiCash";
            this.inputOptiToolStripMenuItem.Click += new System.EventHandler(this.inputOptiToolStripMenuItem_Click);
            // 
            // informationBoardToolStripMenuItem
            // 
            this.informationBoardToolStripMenuItem.Name = "informationBoardToolStripMenuItem";
            this.informationBoardToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.informationBoardToolStripMenuItem.Text = "Information Board";
            this.informationBoardToolStripMenuItem.Click += new System.EventHandler(this.informationBoardToolStripMenuItem_Click);
            // 
            // rekapApprovalToolStripMenuItem
            // 
            this.rekapApprovalToolStripMenuItem.Name = "rekapApprovalToolStripMenuItem";
            this.rekapApprovalToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.rekapApprovalToolStripMenuItem.Text = "Rekap Approval";
            // 
            // cashpointToolStripMenuItem
            // 
            this.cashpointToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inputDataPktToolStripMenuItem,
            this.inputDataDenomToolStripMenuItem});
            this.cashpointToolStripMenuItem.Name = "cashpointToolStripMenuItem";
            this.cashpointToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.cashpointToolStripMenuItem.Text = "Cashpoint";
            // 
            // inputDataPktToolStripMenuItem
            // 
            this.inputDataPktToolStripMenuItem.Name = "inputDataPktToolStripMenuItem";
            this.inputDataPktToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.inputDataPktToolStripMenuItem.Text = "Input data pkt";
            this.inputDataPktToolStripMenuItem.Click += new System.EventHandler(this.inputDataPktToolStripMenuItem_Click);
            // 
            // inputDataDenomToolStripMenuItem
            // 
            this.inputDataDenomToolStripMenuItem.Name = "inputDataDenomToolStripMenuItem";
            this.inputDataDenomToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.inputDataDenomToolStripMenuItem.Text = "Input data denom";
            this.inputDataDenomToolStripMenuItem.Click += new System.EventHandler(this.inputDataDenomToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 502);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aTMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputOptiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rekapApprovalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revisiDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cashpointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputDataPktToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputDataDenomToolStripMenuItem;
    }
}