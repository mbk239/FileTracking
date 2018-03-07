using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracking.Client.Common
{
    public class Constants
    {
        public const bool IsService = true;
        public const bool DiagnosticsDebugger = false;
        private const bool DeployProduction = true;
        public const string ServerAddress = "ws://api.odss.vn:83";
        //public const string ServerAddress = "ws://file.pingg.vn:83/filetracking";
        //public const string ServerAddress = "ws://localhost:83";
        public const int ServerPort = 83;
        //public const string ServerFolderPath = @"D:\FileTrackingServerFolder";
        //public const string LocalAddressBase = "localhost";
        //public const int LocalPort = 83;
        //public const string MQTTBrokerServerAddress = "test.mosquitto.org";
        //public const string MQTTBrokerServerAddress = "localhost";
        public const string RegSubKey = @"SOFTWARE\FileTracking";
        public const string RegToken = "token";
        public const string RegFullname = "Fullname";
        public const string RegUserName = "Username";
        public const string RegPassword = "Password";
        public const string RegFolderPath = "FolderPath";
        public const string RegRemeber = "RegRemeber";


    }
}
