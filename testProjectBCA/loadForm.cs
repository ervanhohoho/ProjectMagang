using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class loadForm : Form
    {
        public loadForm()
        {
            InitializeComponent();
            progressBar1.MarqueeAnimationSpeed = 25;
        }
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static loadForm LoadForm;

        static public void ShowSplashScreen()
        {
            // Make sure it is only launched once.

            if (LoadForm != null)
                return;
            Thread thread = new Thread(new ThreadStart(loadForm.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static private void ShowForm()
        {
            LoadForm = new loadForm();
            Application.Run(LoadForm);
        }

        static public void CloseForm()
        {
            LoadForm.Invoke(new CloseDelegate(loadForm.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            LoadForm.Close();
            LoadForm = null;
        }

        private void loadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }

}
