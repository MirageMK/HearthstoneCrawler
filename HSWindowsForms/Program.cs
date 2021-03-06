﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace HSWindowsForms
{
    internal static class Program
    {
        public static SplashForm splashForm;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [ STAThread ]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //show splash
            Thread splashThread = new Thread(new ThreadStart(
                                                             delegate
                                                             {
                                                                 splashForm = new SplashForm();
                                                                 Application.Run(splashForm);
                                                             }
                                                            ));

            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            MainForm mainForm = new MainForm();
            mainForm.Load += mainForm_Load;
            Application.Run(mainForm);
        }

        private static void mainForm_Load(object sender, EventArgs e)
        {
            //close splash
            if(splashForm == null)
                return;

            splashForm.Invoke(new Action(splashForm.Close));
            splashForm.Dispose();
            splashForm = null;
        }
    }
}