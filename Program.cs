using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize the error log
            ErrorLog Logger = new ErrorLog();

            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                // Initialize the error log
                ErrorLog Logger = new ErrorLog();

                Exception ex = (Exception)e.ExceptionObject;
                string LogFile = Logger.LogError(ex);

                MessageBox.Show(
                    "The application encountered a fatal error and must exit. This error has been logged and should be reported using the Error Report utility.\n\n" +
                        "Error:\n" +
                        ex.Message,
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                
                ///// TODO: Guardareste log a archivo, no utilizar el servicio en linea porque no va a haber internet donde usan el software

                /*
                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
                 */
            }
            finally
            {
                Application.Exit();
            }
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Abort;
            try
            {
                // Initialize the error log
                ErrorLog Logger = new ErrorLog();
                string LogFile = Logger.LogError(e.Exception);

                result = MessageBox.Show(
                    "The application encountered a error. This error has been logged and should be reported using the Error Report utility.\n\n" +
                        "Error:\n" +
                        e.Exception.Message,
                    "Application Error",
                    MessageBoxButtons.AbortRetryIgnore,
                    MessageBoxIcon.Stop);

                ///// TODO: Guardareste log a archivo, no utilizar el servicio en linea porque no va a haber internet donde usan el software
                /*
                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
                 */
            }
            finally
            {
                if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }
            }
        }


    }
}
