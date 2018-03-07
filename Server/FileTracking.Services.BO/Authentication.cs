using FileTracking.Client.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Services.BO
{
    public class Authentication
    {
        public string Username;
        public string Password;
        public string FullName;
        public string Token;
        bool _IsAuthenticated;
        public bool IsAuthenticated
        {
            get
            {
                //TODO check username and password                
                return _IsAuthenticated;
            }
        }

        public string FilePermission(string FullFilePath)
        {
            //TODO check user can access file
            return FullFilePath.Contains(@"\Locked\")? "Server Locked": ""; //should return user checkout this file
        }

        public List<FileMessage> GetAllFileInformation(string username)
        {
            //get information form database            
            List<FileMessage> fileMessages = new List<FileMessage>();
            for (int year = 2012; year < 2017; year++)
            {
                string fileName = year.ToString();
                string filePath = year.ToString();
                FileMessage temp;
                for (int level1 = 1; level1 < 50; level1++)
                {
                    string fileName1 = fileName + "-" + "level " + level1.ToString();
                    string filePath1 = filePath + "\\" + "level" + level1.ToString();
                    for (int level2 = 1; level2 < 7; level2++)
                    {
                        string fileName2 = fileName1 + "-" + "level " + level1.ToString() + "_" + level2.ToString();
                        string filePath2 = filePath1 + "\\" + "level" + level1.ToString() + level2.ToString();
                        for (int level3 = 1; level3 < 10; level3++)
                        {
                            string fileName3 = fileName2 + "-" + "level " + level1.ToString() + "_" + level2.ToString() + "_" + level3.ToString();
                            string filePath3 = filePath2 + "\\" + "level" + level1.ToString() + level2.ToString() + level3.ToString();
                            if (level3 % 2 == 0)
                            {
                                temp = new FileMessage();
                                temp.FilePath = filePath3;
                                temp.FileName = fileName3 + ".docx";
                                temp.UserName = "";
                                temp.FullName = "";
                                temp.Action = FileAction.All;
                                temp.LockedByUserName = "";
                                //TODO: chamge fullname
                                temp.LockedByFullName = "";
                                temp.FileMD5 = "";
                                fileMessages.Add(temp);
                            }
                            else
                            {
                                for (int level4 = 1; level4 < 5; level4++)
                                {
                                    string fileName4 = fileName3 + "-" + "level " + level1.ToString() + "_" + level2.ToString() + "_" + level3.ToString() + "_" + level4.ToString();
                                    string filePath4 = filePath3 + "\\" + "level" + level1.ToString() + level2.ToString() + level3.ToString() + level4.ToString();
                                    temp = new FileMessage();
                                    temp.FilePath = filePath4;
                                    temp.FileName = fileName4 + ".docx";
                                    temp.UserName = "";
                                    temp.FullName = "";
                                    temp.Action = FileAction.All;
                                    temp.LockedByUserName = "server";
                                    //TODO: chamge fullname
                                    temp.LockedByFullName = "server";
                                    temp.FileMD5 = "";
                                    fileMessages.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
            return fileMessages;
        }
        public Authentication(string username, string password, string fullname, string userToken)
        {
            _IsAuthenticated = false;
            try
            {
                string userInfor = CallAPIForOfficeAddIn.GetUserInforWebSocketFromToken(userToken).Result;
                if (!string.IsNullOrEmpty(userInfor))
                {
                    string[] temp = userInfor.Split(';');
                    Username = temp[0].StartsWith("\"") ? temp[0].Substring(1) : temp[0];
                    Password = "";
                    FullName = temp[1].EndsWith("\"") ? temp[1].Substring(0, temp[1].Length - 1) : temp[1];
                    Token = userToken;
                    _IsAuthenticated = true;
                }
            }
            catch {}
        }
    }
}
