using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MassRename
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            // Load the settings and the all the account info
            Settings.Get.Load();

            // Start the app
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());

            // Save the settings
            Settings.Get.Save();
        }
    }
}
