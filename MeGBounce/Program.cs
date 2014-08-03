using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeGBounce
{
    static class Program
    {
        public static string MegBounceVersion = "0.1";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log.Info(string.Format("Starting MegBounce v{0}",Program.MegBounceVersion));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("GLOBAL UNHANDLED EXCEPTION CAUGHT...");
            Log.WriteExceptionLog(e.ExceptionObject as Exception);
        }
    }
}
