using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.Common
{
    public static class RegManager
    {
        public static RegClientInfor LoadConfig()
        {
            var result = new RegClientInfor();
            result.UserInfor = new LoginObject();

            RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.RegSubKey);
            result.UserInfor.Username = key.GetValue(Constants.RegUserName) == null? "": (string)key.GetValue(Constants.RegUserName);
            result.UserInfor.Password = key.GetValue(Constants.RegPassword) == null ? "" : (string)key.GetValue(Constants.RegPassword);
            result.UserInfor.token = key.GetValue(Constants.RegToken) == null ? "" : (string)key.GetValue(Constants.RegToken);
            result.UserInfor.FullName = key.GetValue(Constants.RegFullname) == null ? "" : (string)key.GetValue(Constants.RegFullname);
            result.UserInfor.Remember = key.GetValue(Constants.RegRemeber) == null ? "" : (string)key.GetValue(Constants.RegRemeber);
            result.FolderPath = key.GetValue(Constants.RegFolderPath) == null ? "" : (string)key.GetValue(Constants.RegFolderPath);
            return result;
        }
        
        public static bool SaveConfig(RegClientInfor reginfor)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Constants.RegSubKey, true);
                if (!string.IsNullOrEmpty(reginfor.UserInfor.Username))
                {
                    key.SetValue(Constants.RegUserName, reginfor.UserInfor.Username);
                }
                
                if (!string.IsNullOrEmpty(reginfor.UserInfor.Password))
                {
                    key.SetValue(Constants.RegPassword, reginfor.UserInfor.Password);
                }
                if (!string.IsNullOrEmpty(reginfor.FolderPath))
                {
                    key.SetValue(Constants.RegFolderPath, reginfor.FolderPath);
                }
                if (!string.IsNullOrEmpty(reginfor.UserInfor.token))
                {
                    key.SetValue(Constants.RegToken, reginfor.UserInfor.token);
                }
                if (!string.IsNullOrEmpty(reginfor.UserInfor.Remember))
                {
                    key.SetValue(Constants.RegRemeber, reginfor.UserInfor.Remember);
                }
                if (!string.IsNullOrEmpty(reginfor.UserInfor.FullName))
                {
                    key.SetValue(Constants.RegFullname, reginfor.UserInfor.FullName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class RegClientInfor
    {
        public LoginObject UserInfor { get; set; }


        public string FolderPath { get; set; }
    }
}
