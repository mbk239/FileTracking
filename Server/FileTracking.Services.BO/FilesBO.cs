using FileTracking.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Services.BO
{
    public class FilesBO
    {

        public List<FileMessage> GetAllFileInformation(string username, string path)
        {
            try
            {
                List<FileMessage> result = new List<FileMessage>();
                string rootPath = Constants.ServerFolderPath;
                string getPath = rootPath;
                if (string.Compare(path, "root") == 0)
                {
                    getPath = rootPath + path;

                }
                String[] allfiles = System.IO.Directory.GetFiles(rootPath, "*.*", System.IO.SearchOption.AllDirectories);
                Random gen = new Random();
                foreach (string f in allfiles)
                {
                    FileMessage temp = new FileMessage();
                    temp.Action = FileAction.All;
                    temp.FileName = System.IO.Path.GetFileName(f);
                    temp.FilePath = f.Substring(rootPath.Length, f.Length - temp.FileName.Length - rootPath.Length - 1);
                    temp.UserName = username;
                    temp.FullName = username;
                    temp.LockedByFullName = gen.Next(100) < 50 ? "Server Locked" : "";
                    temp.LockedByUserName = string.IsNullOrEmpty(temp.LockedByFullName) ? "" : "ServerLocked";
                    temp.FileMD5 = FileHelper.GetMD5HashFromFile(f);
                    result.Add(temp);
                }
                return result;
            }
            catch{ return null; }
        }

        public bool CheckoutFile(string username, string filepath)
        {
            try
            {
                //checkout and change status on DB
                return System.IO.Path.GetExtension(filepath).ToLower() == "cs";
            }
            catch { return false; }
        }

        public bool CheckinFile(string username, string filepath, byte[] fileContent)
        {
            try
            {
                //save file to server
                return System.IO.Path.GetExtension(filepath).ToLower() == "cs";
            }
            catch { return false; }
        }

        public byte[] GetFileContent(string filepath)
        {
            try
            {
                string fullServerLocalPath = Constants.ServerFolderPath + "\\"+ filepath;
                byte[] file = System.IO.File.ReadAllBytes(fullServerLocalPath);
                return file;
            }
            catch { return null; }
        }
    }
}
