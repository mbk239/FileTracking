using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using DC.API.Controllers;
using DC.API.Models;
using System.Diagnostics;
using static Common.Utilities.LLogging;

namespace FileTracking.Client.Common
{
    public static class CallAPIForOfficeAddIn
    {


        public static string api = "http://api.odss.vn";
        //public static string api = "http://quanlyfileapi.pingg.vn";
        //public static string api = "http://localhost:6013";

        public static async Task<string> GetUserInforWebSocketFromToken(string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters[""] = token.ToString();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);

            HttpResponseMessage response = await client.PostAsync(api + "/api/file/GetUserInforWebSocketFromToken/", new FormUrlEncodedContent(parameters));
            string data = await response.Content.ReadAsStringAsync();
            return data;

        }

        public static async Task<FileInfor> getFileInfor(string pathFile, string userToken)
        {
            var parameters = new Dictionary<string, string>();
            parameters[""] = pathFile;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", userToken);

            HttpResponseMessage response = await client.PostAsync(api + "/api/file/GetFileInfor/", new FormUrlEncodedContent(parameters));

            string data = await response.Content.ReadAsStringAsync();
            Log("CallAPIForOfficeAddin Line 49: getFileInfor: " + Environment.NewLine +
                "pathFile: " + pathFile + Environment.NewLine +
                "userToken: " + userToken + Environment.NewLine
                + data);
            FileInfor result;
            try
            {
                result = JsonConvert.DeserializeObject<FileInfor>(data);
            }
            catch
            {
                result = new FileInfor();
                if ((string.Compare(data, "Không tìm thấy file này", true) == 0) ||(string.Compare(data, "Path rỗng.", true) == 0))
                {
                    result.Id = 0;
                    result.CanEdit = true;
                    result.CheckoutBy = "";
                }
            }
            client.Dispose();

            return result;
        }

        public static async Task<string> UploadFile(long fileID, string fullFilePath, string userToken)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", userToken);
                // 
                var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var fileInfo = new FileInfo(fullFilePath);
                //bool _fileUploaded = false;

                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(fileStream), "\"file\"", string.Format("\"{0}\"", fileInfo.Name));
                content.Add(new StringContent(fileID.ToString()), "ID");
                //Task taskUpload = client.PutAsync(api + "/api/file/UploadFile/", content).ContinueWith(task =>
                var response = client.PostAsync(api + "/api/file/UploadFile/", content).Result;
                string result = await response.Content.ReadAsStringAsync();
                Log("CallAPIForOfficeAddin Line 90: UploadFile: " + Environment.NewLine +
                    "FileId: " + fileID + " FullFilePath: " + fullFilePath + Environment.NewLine +
                    "userToken: " + userToken + Environment.NewLine 
                    + result);
                return result;
            }
            catch(Exception ex)
            {
                Log("CallAPIForOfficeAddin Line 98: UploadFile Exception thrown for file " + fileID + " " + fullFilePath + ": " + Environment.NewLine + ex.Message);
                return "Failed to connect to server. Please try again.";
            }
        }

        public static async Task<string> GetAllUserAssociateToFile(string pathFile, string userToken)
        {
            var parameters = new Dictionary<string, string>();
            parameters[""] = pathFile;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", userToken);

            HttpResponseMessage response = await client.PostAsync(api + "/api/file/GetUserInforWebSocket/", new FormUrlEncodedContent(parameters));

            string data = await response.Content.ReadAsStringAsync();         
            return data;
        }
    }
}
