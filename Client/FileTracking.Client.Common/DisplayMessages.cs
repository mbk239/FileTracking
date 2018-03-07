using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracking.Client.Common
{
    public class DisplayMessages
    {
        public const string ApplicationName = "File Tracking";
        public const string DisconnectFromServer = "Disconnect from server";
        public const string Disconnect = "Disconnected";
        public const string ConnectedToServer = "Connected to server";
        public const string Connected = "Connected";
        public const string ConnectingToServer = "Connecting to server";
        public const string TryingConnectToServer = "Trying connect to server";
               
        public const string ConfigMenu = "Con&figure";
        public const string ConfigMenuTooltip = "Shows Configure dialog";
        public const string LogMenu = "&Logging";
        public const string LogMenuTooltip = "Shows log dialog";
        public const string ExitMenu = "&Exit";
        public const string ExitMenuTooltip = "Exits File Tracking system";
        public const string WorkingFolderMenu = "&Working folder";
        public const string WorkingFolderMenuTooltip = "Open working folder";

        public const string NotHavePermission = "Bạn không có quyền này.";
        public const string FolderNotExisted = "Folder {0} doesn't existed.";
        public const string SynFaile = "Quá trình đồng bộ thất bại";
        public const string ExecuteSuccess = "Thành công";
        public const string CommonError = "Kết nối tới server gặp vấn đề, xin hãy thử lại";
        // file
        public const string LoginWarning = "Bạn chưa đăng nhập vào hệ thống Quản Lý file. Hãy kiểm tra việc đăng nhập trên ứng dụng Quản Lý File";
        public const string GetContentError = "Không thể truy cập thông tin từ máy chủ Quản Lý File";
        public const string CheckoutedBy = "File này đang được {0} sửa. Bạn không thể sửa file này.";
        public const string DonotHavePermission = "Bạn không có quyền sửa file này.";
        public const string DonotHavePermissionSaveAs = "Bạn không có quyền sửa file này. Hãy lưu thành file khác.";
        public const string FilePathChanged = "Đã cập nhật tài liệu {0}";
        public const string FileNameEmpty = "Tên file không được để trống.";
        public const string FileIsDelete ="File đã bị xóa.";
        public const string FileIsExits = "Tên file đã tồn tại.";
        public const string FileStatusNameEmpty = "Trạng thái không được để trống .";
        //folder
        public const string FolderNotEmpty = "Tên folder không được để trống.";
        public const string FolderIsExits = "Tên folder đã tồn tại.";

        // template 
        public const string TempalteIsSub = "Template phải tạo trong một thư mục";
    }
}
