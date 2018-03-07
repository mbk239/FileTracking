using FileTracking.Client.Common;
using FileTracking.Client.DA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FileTracking.Client.Common.WSCommonObject;


namespace FileTracking.Client.Forms
{
    public partial class frmFilesWorking : Form
    {
        BackgroundWorker bgwSync;
        int updatedFile;
        public RegClientInfor _clientInfor;
        public List<FileMessage> FolderData { get; set; }
        public string WorkingFolder { get; set; }
        public void RenderControl()
        {
            treeViewFolder.Nodes.Clear();
            //treeViewFolder = new TreeView();
            //FolderData = fileMessages;
            FolderData.OrderBy(x => x.FilePath);
            Dictionary<string, TreeNode> allNodes = new Dictionary<string, TreeNode>();
            TreeNode parentNode = null;
            TreeNode rootNode = new TreeNode();
            rootNode.Text = DisplayMessages.ApplicationName;
            rootNode.Name = DisplayMessages.ApplicationName  + "\\";
            string treeName = "";
            List<FileMessage> tempData = FolderData;
            foreach (var fm in tempData)
            {
                string nodename = DisplayMessages.ApplicationName + "\\";                
                string[] nodePath = fm.FilePath.Split('\\');
                parentNode = rootNode;
                if (nodePath.Length > 1)
                {
                    for (int i = 0; i < nodePath.Length; i++)
                    {
                        if (nodePath[i].Length > 0)
                        {
                            
                            nodename += nodePath[i] + "\\";
                            TreeNode[] temp = parentNode.Nodes.Find(nodename, false);

                            if (temp.Length > 0) {
                                parentNode = temp[0];
                            }
                            else
                            {
                                TreeNode newNode = new TreeNode();
                                newNode.Text = nodePath[i];
                                newNode.Name = nodename;
                                int newIndex = parentNode.Nodes.Add(newNode);
                                parentNode = parentNode.Nodes[newIndex];
                            }
                        }
                    }
                }
            }
            treeViewFolder.Nodes.Add(rootNode);
        }

        public frmFilesWorking()
        {
            InitializeComponent();
            CustomResize(this);
        }
        private void FilesWorking_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            if (string.Compare(control.Name, "frmFilesWorking", true) == 0)
                CustomResize(control);
        }

        private void CustomResize(Control control)
        {
            treeViewFolder.Width = (int)control.Size.Width / 4;
            treeViewFolder.Height = control.Size.Height - 120;
            listViewFiles.Left = treeViewFolder.Left + treeViewFolder.Width + 20;
            listViewFiles.Top = treeViewFolder.Top;
            listViewFiles.Height = treeViewFolder.Height;
            listViewFiles.Width = control.Size.Width - treeViewFolder.Width - 80;
            progressBar.Top = treeViewFolder.Top + treeViewFolder.Height + 20;
            progressBar.Left = treeViewFolder.Left;
            progressBar.Width = treeViewFolder.Width + listViewFiles.Width - 60;
        }

        List<FileMessage> TempData()
        {
            List<FileMessage> result = new List<FileMessage>();
            string rootPath = @"D:\TSRData\DBs\";
            string getPath = rootPath;
            String[] allfiles = System.IO.Directory.GetFiles(rootPath, "*.*", System.IO.SearchOption.AllDirectories);
            Random gen = new Random();
            foreach (string f in allfiles)
            {
                FileMessage temp = new FileMessage();
                temp.Action = FileAction.All;
                temp.FileName = System.IO.Path.GetFileName(f);
                temp.FilePath = f.Substring(rootPath.Length, f.Length - temp.FileName.Length - rootPath.Length);
                temp.UserName = "NQHUNG";
                temp.FullName = "NQHUNG";
                temp.LockedByFullName = gen.Next(100) < 50 ? "Server Locked" : "";
                temp.LockedByUserName = gen.Next(100) < 50 ? "ServerLocked" : "";
                temp.FileMD5 = FileHelper.GetMD5HashFromFile(f);
                result.Add(temp);
            }
            return (result);
        }
        private void frmFilesWorking_Load(object sender, EventArgs e)
        {
            //FolderData = TempData();

            treeViewFolder.AfterSelect += TreeViewFolder_AfterSelect;
            TreeNode rootNode = new TreeNode();
            rootNode.Text = DisplayMessages.ApplicationName;
            treeViewFolder.Nodes.Clear();
            treeViewFolder.Nodes.Add(rootNode);
            addFolderNode(rootNode, WorkingFolder);
            //RenderControl();
        }

        private void TreeViewFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedFolder = WorkingFolder + e.Node.FullPath.Substring(DisplayMessages.ApplicationName.Length);
            String[] allfiles = System.IO.Directory.GetFiles(selectedFolder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            String[] allDirs = System.IO.Directory.GetDirectories(selectedFolder, "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string f in allDirs)
            {
                listViewFiles.Items.Add(System.IO.Path.GetDirectoryName(f));
            }
            foreach (string f in allfiles)
            {
                listViewFiles.Items.Add(System.IO.Path.GetFileName(f));
            }
        }

        private void addFolderNode(TreeNode node, string FolderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath);
            try
            {
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo directory in directories)
                    {
                        TreeNode nodeTemp = node.Nodes.Add(directory.Name);
                        nodeTemp.ImageIndex = nodeTemp.SelectedImageIndex = 0;
                        addFolderNode(nodeTemp, directory.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            StartGetAllFileInformationAsyncAsync();
        }
        async Task StartGetAllFileInformationAsyncAsync()
        {
            FolderData = await RestAPIs.GetAllFileInformationAsync(_clientInfor.UserInfor.Username, _clientInfor.UserInfor.Password, "root");
            bgwSync = new BackgroundWorker();
            bgwSync.WorkerReportsProgress = true;
            bgwSync.WorkerSupportsCancellation = true;
            bgwSync.DoWork += BgwSync_DoWork;
            bgwSync.ProgressChanged += BgwSync_ProgressChanged;
            bgwSync.RunWorkerCompleted += BgwSync_RunWorkerCompleted;
            updatedFile = 0;
            bgwSync.RunWorkerAsync();
            RenderControl();
        }

        private void BgwSync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BgwSync_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //4throw new NotImplementedException();
        }

        private void BgwSync_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var fm in FolderData)
            {
                StartDownloadFileAsync(_clientInfor.UserInfor.Username, _clientInfor.UserInfor.Password, fm.FilePath + "\\" + fm.FileName);
            }
        }

        async Task StartDownloadFileAsync(string username, string password, string filepath)
        {
            FileDownload temp = new FileDownload();
            temp = await RestAPIs.DownloadFile(username, username, filepath);

            var path = WorkingFolder;
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 2);
            if (temp.FilePath.StartsWith("\\")) temp.FilePath = temp.FilePath.Substring(1);
            path += "\\" + temp.FilePath;
            new System.IO.DirectoryInfo(path).Create();
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 2);

            File.WriteAllBytes(path + "\\" + temp.FileName, temp.FileContent);
            updatedFile++;
            progressBar.Value = (updatedFile / FolderData.Count) * 100;
        }

        private void btnTestDownload_Click(object sender, EventArgs e)
        {
            StartDownloadFileAsync(_clientInfor.UserInfor.Username, _clientInfor.UserInfor.Password, @"Client\CV_E_Vi_Van_Hung.doc");
        }
    }
}
