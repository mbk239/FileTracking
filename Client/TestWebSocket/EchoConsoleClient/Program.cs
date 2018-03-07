using FileTracking.Client.WebSocketManager;
using FileTracking.Client.Common;
using System;
using System.Threading.Tasks;
using static FileTracking.Client.Common.WSCommonObject;

public class Program
{
    private static WSConnection _connection;
    public static void Main(string[] args)
    {
        LoginObject loginInfor = new LoginObject();
        loginInfor.Username = "loguser";
        loginInfor.Password = "1";
        Console.Write("Load test: Press Enter to start");
        Console.ReadLine();

        StartConnectionAsync(loginInfor);
        _connection.OnFileMessage += _connection_OnFileMessage;
        _connection.OnMessage += _connection_OnMessage;
        Console.Write("Waitting connection");
        while (!_connection.Connected)
        {
            Console.Write(".");
            System.Threading.Thread.Sleep(100);
        }

        Console.WriteLine("");
        Console.WriteLine("Connected");        
        Console.ReadLine();
        StopConnectionAsync();
    }

    private static void _connection_OnMessage(MessageObject message)
    {
        Console.WriteLine($"{message.UserName} said: {message.Message}");
    }

    private static void _connection_OnFileMessage(FileMessage fileMessage)
    {
        Console.WriteLine($"{fileMessage.UserName} : file {fileMessage.FilePath + fileMessage.FileName} -> {fileMessage.Action}");
    }

    public static async Task StartConnectionAsync(LoginObject loginInfor)
    {
        _connection = new WSConnection();
        await _connection.StartConnectionAsync("ws://file.pingg.vn:83/filetracking", loginInfor);
        //await _connection.StartConnectionAsync(Constants.ServerAddress, loginInfor);
    }

    public static async Task StopConnectionAsync()
    {
        await _connection.StopConnectionAsync();
    }

    public static async Task SendMessage(string data)
    {
        await _connection.SendMessage(data);
    }
}