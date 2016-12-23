using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HSWindowsForms.Properties;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class SplashForm : ShapedForm
    {
        public SplashForm()
        {
            InitializeComponent();
            this.Width = 400;
            this.Height = 270;
        }

        private void SplashForm_SizeChanged(object sender, EventArgs e)
        {
            this.Width = 400;
            this.Height = 270;
        }
    }
}
