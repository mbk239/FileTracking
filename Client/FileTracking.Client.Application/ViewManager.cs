using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using FileTracking.Client.Forms;
using System.Timers;
using System.ComponentModel;
using FileTracking.Client.WebSocketManager;
using System.IO;
using FileTracking.Client.Common;
using System.Diagnostics;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.Application
{
    public class ViewManager
    {        
        RegClientInfor _clientInfor;
        // This allows code to be run on a GUI thread
        private System.Windows.Window _hiddenWindow;

        private System.ComponentModel.IContainer _components;
        // The Windows system tray class
        private NotifyIcon _notifyIcon;

        private frmConfigure _frmConfigure;
        private frmLogging _frmLog;
        private frmFilesWorking _frmFilesWorking;

        private ToolStripMenuItem _controlWebSocketMenuItem;
        private ToolStripMenuItem _logMenuItem;
        private ToolStripMenuItem _exitMenuItem;

        static WSConnectionNew _connection;
        BackgroundWorker bgwConnectWS = null;
        FileSystemWatcher watcher;
        System.Timers.Timer checkWebSocketTimer;
        public ViewManager(RegClientInfor clientInfor)
        {
            _clientInfor = clientInfor;
            _components = new System.ComponentModel.Container();
            _notifyIcon = new System.Windows.Forms.NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = FileTracking.Client.Application.Properties.Resources.NotReadyIcon,
                Text = "File Tracking",
                Visible = true,
            };


            _notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;            
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;

            _hiddenWindow = new System.Windows.Window();
            _hiddenWindow.Hide();

            _frmLog = new frmLogging();
            _frmLog.LoggingText = "";

            InitConnect();
            checkWebSocketTimer = new System.Timers.Timer();
            checkWebSocketTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            checkWebSocketTimer.Interval = 60000;
            checkWebSocketTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!_connection.Connected)
            {
                _notifyIcon.Icon = Properties.Resources.NotReadyIcon;
                if ((bgwConnectWS == null) || (!bgwConnectWS.IsBusy))
                    InitConnect();
            }
        }

        System.Windows.Media.ImageSource AppIcon
        {
            get
            {
                System.Drawing.Icon icon = (_connection.Connected) ? Properties.Resources.ReadyIcon : Properties.Resources.NotReadyIcon;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        private void DisplayStatusMessage(string text)
        {
            _hiddenWindow.Dispatcher.Invoke(delegate
            {
                _notifyIcon.ShowBalloonTip(200, DisplayMessages.ApplicationName, text, ToolTipIcon.Info);
            });
        }

        void InitConnect()
        {            
            if (_controlWebSocketMenuItem != null)
            {
                _controlWebSocketMenuItem.Text = DisplayMessages.TryingConnectToServer;
                _controlWebSocketMenuItem.ToolTipText = DisplayMessages.TryingConnectToServer;
            }
            _notifyIcon.Icon = Properties.Resources.NotReadyIcon;

            bgwConnectWS = new BackgroundWorker();
            bgwConnectWS.WorkerReportsProgress = true;
            bgwConnectWS.WorkerSupportsCancellation = true;
            bgwConnectWS.DoWork += new DoWorkEventHandler(bgwConnectWSl_DoWork);
            //bgwConnectWS.ProgressChanged += new ProgressChangedEventHandler(bgwConnectWS_ProgressChanged);
            bgwConnectWS.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwConnectWS_RunWorkerCompleted);
            bgwConnectWS.RunWorkerAsync();
        }

        public async Task StartConnectionAsync()
        {
            await _connection.StartConnectionAsync(_clientInfor.UserInfor);
        }

        public async Task StopConnectionAsync()
        {
            await _connection.StopConnectionAsync();
        }

        void bgwConnectWSl_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while ((_connection == null) || (!_connection.Connected))
                {
                    _connection = new WSConnectionNew(Constants.ServerAddress);
                    _connection.OnFileMessage += _connection_OnFileMessage;
                    _connection.OnMessage += _connection_OnMessage;
                    DisplayStatusMessage(DisplayMessages.TryingConnectToServer);
                    StartConnectionAsync();
                    System.Threading.Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Cannot connect to server", "Connect to server error", MessageBoxButton.OK);
            }
        }

        void bgwConnectWS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (System.IO.Directory.Exists(_clientInfor.FolderPath))
            {
                watcher = new FileSystemWatcher();
                watcher.Path = _clientInfor.FolderPath;
                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*";
                watcher.Changed += new FileSystemEventHandler(FileOnchanged);
            }
            else
            {
                DisplayStatusMessage(string.Format(DisplayMessages.FolderNotExisted, _clientInfor.FolderPath));
            }

            if (_connection.Connected)
            {
                DisplayStatusMessage(DisplayMessages.Connected);
                _notifyIcon.Icon = Properties.Resources.ReadyIcon;
            }
            else
            {
                DisplayStatusMessage(DisplayMessages.Disconnect);
                _notifyIcon.Icon = Properties.Resources.NotReadyIcon;
            }
            bgwConnectWS.Dispose();
        }

        private void _connection_OnMessage(MessageObject message)
        {
            string Message = $"{message.UserName} said: {message.Message}";
            DisplayStatusMessage(Message);
        }

        private void _connection_OnFileMessage(FileMessage fileMessage)
        {
            string Message = $"{fileMessage.UserName} : file {fileMessage.FilePath + fileMessage.FileName} -> {fileMessage.Action}";
            DisplayStatusMessage(Message);
        }

        private void FileOnchanged(object sender, FileSystemEventArgs e)
        {
            FileMessage fm = new FileMessage();
            fm.FileName = e.Name;
            fm.FilePath = e.FullPath;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created: fm.Action = FileAction.Created; break;
                case WatcherChangeTypes.Changed: fm.Action = FileAction.Changed; break;
                case WatcherChangeTypes.Deleted: fm.Action = FileAction.Deleted; break;
                case WatcherChangeTypes.Renamed: fm.Action = FileAction.Renamed; break;
            }
            Task responseTask = SendFileMessage(fm);
            responseTask.Wait();
        }

        public async Task SendCommand(string Command)
        {
            _connection.SendCommand(Command).Wait();
        }

        public async Task SendFileMessage(FileMessage fm)
        {
            _connection.SendFileAction(fm).Wait();            
        }

        private void LogMessage(string message)
        {
            if (_frmLog == null) _frmLog = new frmLogging();
            _frmLog.LoggingText = message;
        }

        private void startStopWebSocket_Click(object sender, EventArgs e)
        {
            if (!_connection.Connected)
            {
                InitConnect();
                _controlWebSocketMenuItem.Text = DisplayMessages.DisconnectFromServer;
                _controlWebSocketMenuItem.ToolTipText = DisplayMessages.DisconnectFromServer;
                _notifyIcon.Icon = Properties.Resources.ReadyIcon;
            }
            else
            {
                bgwConnectWS.CancelAsync();
                StopConnectionAsync();
                _controlWebSocketMenuItem.Text = DisplayMessages.ConnectedToServer;
                _controlWebSocketMenuItem.ToolTipText = DisplayMessages.ConnectedToServer;
                _notifyIcon.Icon = Properties.Resources.NotReadyIcon;
            }
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, string tooltipText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }

            item.ToolTipText = tooltipText;
            return item;
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            if (bgwConnectWS != null)
                bgwConnectWS.CancelAsync();
            StopConnectionAsync();
            System.Windows.Forms.Application.Exit();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if ((_frmFilesWorking == null) || (!_frmFilesWorking.Visible))
            {
                _frmFilesWorking = new frmFilesWorking();
                _frmFilesWorking._clientInfor = _clientInfor;
                _frmFilesWorking.WorkingFolder = _clientInfor.FolderPath;
                _frmFilesWorking.Show();
            }
        }

        private void ShowConfigureFrom_Click(object sender, EventArgs e)
        {
            if ((_frmConfigure == null) || (!_frmConfigure.Visible))
            {
                _frmConfigure = new frmConfigure();
                var result = _frmConfigure.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _clientInfor = RegManager.LoadConfig();
                }
            }
        }

        private void ShowLogFrom_Click(object sender, EventArgs e)
        {
            if ((_frmLog == null) || (!_frmLog.Visible))
            {
                _frmLog = new frmLogging();
                _frmLog.Show();
            }
        }

        private void ShowWorkingFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(_clientInfor.FolderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = _clientInfor.FolderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format(DisplayMessages.FolderNotExisted, _clientInfor.FolderPath));
            }
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _notifyIcon.ContextMenuStrip.Left = _notifyIcon.ContextMenuStrip.Left - 20;
                _notifyIcon.ContextMenuStrip.Top = _notifyIcon.ContextMenuStrip.Top - 20;
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }

        private void SetMenuItems()
        {
            //TODO            
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;            
            if (_notifyIcon.ContextMenuStrip.Items.Count == 0)
            {
                if (bgwConnectWS.IsBusy)
                {
                    _controlWebSocketMenuItem = ToolStripMenuItemWithHandler(DisplayMessages.ConnectingToServer, DisplayMessages.ConnectingToServer, null);
                }
                else
                {
                    if (!_connection.Connected)
                        _controlWebSocketMenuItem = ToolStripMenuItemWithHandler(DisplayMessages.ConnectedToServer, DisplayMessages.ConnectedToServer, null);
                    else
                        _controlWebSocketMenuItem = ToolStripMenuItemWithHandler(DisplayMessages.DisconnectFromServer, DisplayMessages.DisconnectFromServer, null);
                }
                _notifyIcon.ContextMenuStrip.Items.Add(_controlWebSocketMenuItem);
                _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(DisplayMessages.ConfigMenu, DisplayMessages.ConfigMenuTooltip, ShowConfigureFrom_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(DisplayMessages.WorkingFolderMenu, DisplayMessages.WorkingFolderMenuTooltip, ShowWorkingFolder_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(DisplayMessages.LogMenu, DisplayMessages.LogMenuTooltip, ShowLogFrom_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                _exitMenuItem = ToolStripMenuItemWithHandler(DisplayMessages.ExitMenu, DisplayMessages.ExitMenuTooltip, exitItem_Click);
                _notifyIcon.ContextMenuStrip.Items.Add(_exitMenuItem);
            }

            SetMenuItems();
        }
    }
}
