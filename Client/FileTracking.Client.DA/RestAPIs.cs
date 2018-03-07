using FileTracking.Client.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.DA
{
    public static class RestAPIs
    {        
        public static async Task<List<FileMessage>> GetAllFileInformationAsync(string username, string password, string path)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + Constants.ServerApiAddress+  "/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<FileMessage> result = null;
            HttpResponseMessage response = await client.GetAsync(string.Format(RestAPIsPath.GetAllFileInformation, username, password, path));
            if (response.IsSuccessStatusCode)
            {
                var sResult = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<FileMessage>>(sResult);
            }
            return result;
        }

        public static async Task<FileDownload> DownloadFile(string username, string password, string filepath)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://" + Constants.ServerApiAddress + "/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            FileDownload result = null;
            HttpResponseMessage response = await client.GetAsync(string.Format(RestAPIsPath.DownloadFileByFilePath, username, password, HttpUtility.UrlEncode(filepath)));
            if (response.IsSuccessStatusCode)
            {
                var sResult = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<FileDownload>(sResult);
            }
            return result;
        }
    }
}
