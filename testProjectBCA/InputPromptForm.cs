using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InputPromptForm : Form
    {
        public String value { set; get; }
        public InputPromptForm()
        {
            InitializeComponent();
        }
        public InputPromptForm(String label, String title)
        {
            InitializeComponent();
            label1.Text = label;
            this.Text = title;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            value = valueTextBox.Text;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
