namespace FileTracking.Client.Forms
{
    partial class frmFilesWorking
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.Filename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LockStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.md5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeViewFolder = new System.Windows.Forms.TreeView();
            this.btnUpdateAll = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnTestDownload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewFiles
            // 
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Filename,
            this.LockStatus,
            this.md5});
            this.listViewFiles.Location = new System.Drawing.Point(259, 12);
            this.listViewFiles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(671, 542);
            this.listViewFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewFiles.TabIndex = 0;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            // 
            // Filename
            // 
            this.Filename.Text = "Filename";
            this.Filename.Width = 234;
            // 
            // LockStatus
            // 
            this.LockStatus.Text = "Lock Status";
            this.LockStatus.Width = 109;
            // 
            // md5
            // 
            this.md5.Text = "MD5";
            // 
            // treeViewFolder
            // 
            this.treeViewFolder.Location = new System.Drawing.Point(13, 14);
            this.treeViewFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeViewFolder.Name = "treeViewFolder";
            this.treeViewFolder.Size = new System.Drawing.Size(229, 540);
            this.treeViewFolder.TabIndex = 1;
            // 
            // btnUpdateAll
            // 
            this.btnUpdateAll.Location = new System.Drawing.Point(13, 558);
            this.btnUpdateAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUpdateAll.Name = "btnUpdateAll";
            this.btnUpdateAll.Size = new System.Drawing.Size(229, 34);
            this.btnUpdateAll.TabIndex = 2;
            this.btnUpdateAll.Text = "Update All";
            this.btnUpdateAll.UseVisualStyleBackColor = true;
            this.btnUpdateAll.Click += new System.EventHandler(this.btnUpdateAll_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 623);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(917, 23);
            this.progressBar.TabIndex = 3;
            // 
            // btnTestDownload
            // 
            this.btnTestDownload.Location = new System.Drawing.Point(259, 558);
            this.btnTestDownload.Name = "btnTestDownload";
            this.btnTestDownload.Size = new System.Drawing.Size(224, 34);
            this.btnTestDownload.TabIndex = 4;
            this.btnTestDownload.Text = "Test download file";
            this.btnTestDownload.UseVisualStyleBackColor = true;
            this.btnTestDownload.Click += new System.EventHandler(this.btnTestDownload_Click);
            // 
            // frmFilesWorking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 658);
            this.Controls.Add(this.btnTestDownload);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpdateAll);
            this.Controls.Add(this.treeViewFolder);
            this.Controls.Add(this.listViewFiles);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmFilesWorking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Files Working";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFilesWorking_Load);
            this.Resize += new System.EventHandler(this.FilesWorking_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.TreeView treeViewFolder;
        private System.Windows.Forms.Button btnUpdateAll;
        private System.Windows.Forms.ColumnHeader Filename;
        private System.Windows.Forms.ColumnHeader LockStatus;
        private System.Windows.Forms.ColumnHeader md5;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnTestDownload;
    }
}