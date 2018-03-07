using FileTracking.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTracking.Client.Forms
{
    public partial class frmConfigure : Form
    {
        RegClientInfor clientInfor;
        public string Username { get; set; }
        public string Password { get; set; }

        public string WorkingFolderPath { get; set; }
       
        public frmConfigure()
        {
            InitializeComponent();
            clientInfor = RegManager.LoadConfig();
            txtFolderPath.Text = clientInfor.FolderPath;
            txtPassword.Text = clientInfor.UserInfor.Password;
            txtUsername.Text = clientInfor.UserInfor.Username;
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            folderBrowser = new FolderBrowserDialog();
            folderBrowser.SelectedPath = txtFolderPath.Text;
            folderBrowser.ShowDialog();
            txtFolderPath.Text = folderBrowser.SelectedPath;
            folderBrowser.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            clientInfor.FolderPath = txtFolderPath.Text;
            clientInfor.UserInfor.Username = txtUsername.Text;
            clientInfor.UserInfor.Password = txtPassword.Text;
            RegManager.SaveConfig(clientInfor);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmConfigure_FormClosing(object sender, FormClosingEventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Text;
            WorkingFolderPath = txtFolderPath.Text;
        }
    }
}
