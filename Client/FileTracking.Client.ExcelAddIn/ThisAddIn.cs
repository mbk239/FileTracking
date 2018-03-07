using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using System.Windows.Forms;
using FileTracking.Client.Common;
using FileTracking.Client.WebSocketManager;
using static FileTracking.Client.Common.WSCommonObject;
using System.IO;
using System.Threading.Tasks;

namespace FileTracking.Client.ExcelAddIn
{
    public partial class ThisAddIn
    {
        private static WSConnectionNew _connection;
        RegClientInfor clientInfor;
        Excel.Application wb;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            wb = this.Application;
            wb.WorkbookActivate += Wb_WorkbookActivate;
           // ((Excel.Worksheet)Globals.ThisWorkbook.ActiveSheet).SelectionChange += new Excel.DocEvents_SelectionChangeEventHandler(activeSheet_SelectionChange);
            clientInfor = RegManager.LoadConfig();

            //TODO: for test
            if (string.IsNullOrEmpty(clientInfor.UserInfor.Username))
            {
                clientInfor = new RegClientInfor(); clientInfor.UserInfor = new LoginObject();
                clientInfor.UserInfor.Username = "ClientApp";
                clientInfor.UserInfor.Password = "password";
                clientInfor.FolderPath = @"C:\ApplicationFiles";
                RegManager.SaveConfig(clientInfor);
            }

            StartConnectionAsync(clientInfor.UserInfor);
            _connection.OnFileMessage += _connection_OnFileMessage;
            _connection.OnMessage += _connection_OnMessage;
        }

        private void _connection_OnMessage(MessageObject message)
        {
            MessageBox.Show(message.FullName + ": " + message.Message, "File Tracking");
        }

        private void _connection_OnFileMessage(FileMessage fileMessage)
        {
            if (!string.IsNullOrEmpty(fileMessage.LockedByUserName) && (string.Compare(fileMessage.LockedByUserName, clientInfor.UserInfor.Username) != 0))
            {
                string fileToOpen = this.Application.ActiveWorkbook.FullName;
                MessageBox.Show(this.Application.ActiveWorkbook.Name + " locked by " + fileMessage.LockedByFullName, "File Tracking");
                this.Application.ActiveWorkbook.ChangeFileAccess(Excel.XlFileAccess.xlReadOnly, missing, missing);
            }
            else
            {
                MessageBox.Show("You can edit and checkout this file.", "File Tracking");
            }
        }

        private void Wb_WorkbookActivate(Excel.Workbook Wb)
        {
            try
            {
                //TODO: Check current file manage base on clientInfor.FolderPath
                if (this.Application.ActiveWorkbook.FullName.StartsWith(clientInfor.FolderPath))
                {
                    if (_connection.Connected)
                    {
                        FileMessage fm = new FileMessage();
                        fm.Action = FileAction.CheckEditable;
                        FileInfo fileOpened = new FileInfo(this.Application.ActiveWorkbook.FullName);
                        fm.FileName = fileOpened.Name + fileOpened.Extension;
                        fm.FilePath = fileOpened.FullName;
                        SendFileMessage(fm);
                    }
                    else
                    {
                        MessageBox.Show("Cannot connect to server, maybe your edit make conflict with other user", "File Tracking");
                    }
                }
            }
            catch { }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            StopConnectionAsync().Wait(); ;
        }

        public static async Task StartConnectionAsync(LoginObject loginInfor)
        {
            try
            {
                _connection = new WSConnectionNew(Constants.ServerAddress);
                await _connection.StartConnectionAsync(loginInfor);
            }
            catch { }
        }

        public static async Task StopConnectionAsync()
        {
            try
            {
                _connection.StopConnectionAsync().Wait();
            }
            catch { }
        }

        public static async Task SendMessage(string data)
        {
            _connection.SendMessage(data).Wait();
        }

        public async Task SendFileMessage(FileMessage fm)
        {
            _connection.SendFileAction(fm).Wait();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
