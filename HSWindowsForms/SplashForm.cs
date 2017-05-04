using System;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class SplashForm : ShapedForm
    {
        public SplashForm()
        {
            InitializeComponent();
            Width = 400;
            Height = 270;
        }

        private void SplashForm_SizeChanged(object sender, EventArgs e)
        {
            Width = 400;
            Height = 270;
        }
    }
}