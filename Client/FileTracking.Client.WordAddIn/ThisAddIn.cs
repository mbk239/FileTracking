using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Windows.Forms;
using System.Threading.Tasks;
using FileTracking.Client.WebSocketManager;
using FileTracking.Client.Common;
using System.IO;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.WordAddIn
{    
    public partial class ThisAddIn
    {
        private static WSConnection _connection;
        RegClientInfor clientInfor;
        Word.Application wb;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            wb = this.Application;            
            wb.DocumentChange += Wb_DocumentChange;
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
                MessageBox.Show(this.Application.ActiveDocument.Name + " locked by " + fileMessage.LockedByFullName, "File Tracking");
                this.Application.ActiveDocument.Protect(Word.WdProtectionType.wdAllowOnlyReading, false, System.String.Empty, false, false);
            }
            else
            {
                MessageBox.Show("You can edit and checkout this file.", "File Tracking");
            }
        }

        private void Wb_DocumentChange()
        {
            try
            {
                //TODO: Check current file manage base on clientInfor.FolderPath
                if (this.Application.ActiveDocument.FullName.StartsWith(clientInfor.FolderPath))
                {
                    if (_connection.Connected)
                    {
                        FileMessage fm = new FileMessage();
                        fm.Action = FileAction.CheckEditable;
                        FileInfo fileOpened = new FileInfo(this.Application.ActiveDocument.FullName);
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
                _connection = new WSConnection();
                await _connection.StartConnectionAsync(Constants.ServerAddress, loginInfor);
            }catch{ }
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
