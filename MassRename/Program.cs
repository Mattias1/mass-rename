using System;
using System.Windows.Forms;

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

            var mainForm = new Main();
            if (args.Length > 0) {
                mainForm.LoadArgumentsFileOrDir(args[0]);
            }

            Application.Run(mainForm);

            // Save the settings
            Settings.Get.Save();
        }
    }
}
