using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracking.Client.Common
{
    public class RestAPIsPath
    {
        public const string GetAllFileInformation = "api/user/GetAllFilesInformation/{0}/{1}/{2}";
        public const string DownloadFileByFilePath = "api/file/DownloadFileByFilePath/{0}/{1}/{2}";
    }
}
