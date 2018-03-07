using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracking.Client.Common
{
    public class WSCommonObject
    {
        public class MessageType
        {
            public const string Login = "login";
            public const string FileAction = "fileaction";
            public const string Message = "message";
            public const string APICommand = "apicommand";
            public const string GetAllFileInformation = "getallfileinfor";
            public const string CheckFileMD5 = "checkfilemd5";
            public const string GetConcurrentUsers = "GetConcurrentUsers";
        }

        public class csockdata
        {
            public string MessageType;
            public string Value;
            public csockdata()
            {
                MessageType = string.Empty;
                Value = string.Empty;
            }
            public csockdata(string messageType)
            {
                MessageType = messageType;
                Value = string.Empty;
            }
            public csockdata(string messageType, string value)
            {
                MessageType = messageType;
                Value = value;
            }
        }

        public enum FileAction
        {
            Created = 1,
            Deleted = 2,
            Changed = 4,
            Renamed = 8,
            All = 15,
            Checkout = 10,
            Checkin = 12,
            CheckEditable = 13
        }
        public class FileMessage
        {
            public string UserName;
            public string FullName;
            public string FilePath;
            public string FileName;
            public FileAction Action;
            public string LockedByUserName;
            public string LockedByFullName;
            public string FileMD5;
        }
        public class FileDownload
        {
            public string FilePath;
            public string FileName;
            public string LockedByUserName;
            public string LockedByFullName;
            public string FileMD5;
            public byte[] FileContent;
        }

        public class MessageObject
        {
            public string UserName;
            public string FullName;
            public string Message;
        }

        public class APIMessageObject
        {
            public string UserName;
            public string FullName;
            public List<string> ToList;
            public string Message;
        }

        public class LoginObject
        {
            public string Username;
            public string Password;
            public string token;
            public string FullName;
            public string Remember;
        }
    }
}
