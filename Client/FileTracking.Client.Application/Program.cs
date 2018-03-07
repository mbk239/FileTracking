using FileTracking.Client.Common;
using FileTracking.Client.WebSocketManager;
using System;
using System.Windows.Forms;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.Application
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Use the assembly GUID as the name of the mutex which we use to detect if an application instance is already running
            bool createdNew = false;
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    // Only allow one instance
                    return;
                }

                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                RegClientInfor clientInfor = RegManager.LoadConfig();

                //TODO: for test
                if (string.IsNullOrEmpty(clientInfor.UserInfor.Username))
                {
                    clientInfor = new RegClientInfor(); clientInfor.UserInfor = new LoginObject();
                    clientInfor.UserInfor.Username = "ClientApp";
                    clientInfor.UserInfor.Password = "password";
                    clientInfor.FolderPath = @"C:\ApplicationFiles";
                    RegManager.SaveConfig(clientInfor);
                }

                try
                {
                    STAApplicationContext context = new STAApplicationContext(clientInfor);
                    System.Windows.Forms.Application.Run(context);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
        }
    }
}
