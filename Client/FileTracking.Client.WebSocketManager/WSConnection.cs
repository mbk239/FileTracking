using System;
using System.Collections.Generic;
using System.IO;
//using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static FileTracking.Client.Common.WSCommonObject;
using WebSocket4Net;
namespace FileTracking.Client.WebSocketManager
{
    public class WSConnection
    {
        public bool Connected { get { return _clientWebSocket.State == WebSocketState.Open; } }

        public delegate void OnMessageEvent(MessageObject message);
        public event OnMessageEvent OnMessage;

        public delegate void OnFileMessageEvent(FileMessage fileMessage);
        public event OnFileMessageEvent OnFileMessage;

        public delegate void OnGetAllFileInformationEvent(List<FileMessage> fileMessages);
        public event OnGetAllFileInformationEvent OnGetAllFileInformation;

        private WebSocket _clientWebSocket { get; set; }
        private LoginObject UserInfor { get; set; }

        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public WSConnection(string uri, LoginObject user)
        {
            try
            {
                _clientWebSocket = new WebSocket(uri);
            }
            catch (Exception ex)
            {

                throw;
            }
             
            _clientWebSocket.Opened += new EventHandler(websocket_Opened);
            _clientWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            _clientWebSocket.Closed += new EventHandler(websocket_Closed);
            _clientWebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            _clientWebSocket.Open();
            
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine(_clientWebSocket.State);
            Console.ReadLine();
            SendLogin();
            // _clientWebSocket = new ClientWebSocket();
        }
        public void websocket_Closed(object sender, EventArgs e)
        {

            Console.WriteLine(e.ToString());

        }
        public void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {

            Console.WriteLine(e.ToString());

        }
        public void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _clientWebSocket = (WebSocket)sender;
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                string serializedMessage = null;
                var result = JsonConvert.DeserializeObject<csockdata>(e.Message);

                if (result.MessageType == "loginfail")
                {
                    // var message = JsonConvert.DeserializeObject<csockdata>(serializedMessage);

                }


            }

        }
        //protected void webSocketClient_MessageReceived(object sender, DataReceivedEventArgs e)
        //{
        //    m_CurrentMessage = Encoding.UTF8.GetString(e.Data);
        //    m_MessageReceiveEvent.Set();
        //}
        public void websocket_Opened(object sender, EventArgs e)
        {
            WebSocket websocket = (WebSocket)sender;
            SendLogin();
        }
        public async Task StartConnectionAsync(string uri, LoginObject user)
        {
            UserInfor = user;
            //await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None).ConfigureAwait(false);
            SendLogin();
           // await Receive(_clientWebSocket, (message) =>
           // {
           //     Invoke(message);
           // });
        }


        public void SendLogin()
        {
            try
            {
                string ls = JsonConvert.SerializeObject(UserInfor);
                csockdata messageToSend = new csockdata(MessageType.Login, ls);
                string result = JsonConvert.SerializeObject(messageToSend);
                var encoded = Encoding.UTF8.GetBytes(result);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                _clientWebSocket.Send(buffer.Array,buffer.Offset, encoded.Length);
                //, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        public async Task SendCommand(string Command)
        {
            try
            {
                csockdata messageToSend = new csockdata(Command, "");
                string result = JsonConvert.SerializeObject(messageToSend);
                //await SendAsynWait(result);
                var encoded = Encoding.UTF8.GetBytes(result);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                _clientWebSocket.Send(buffer.Array, buffer.Offset, encoded.Length);
                // Task responseTask = _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
               // responseTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public async Task SendObject(MessageType type, object objSend)
        {
            try
            {
                string sToSend = JsonConvert.SerializeObject(objSend);
                csockdata messageToSend = new csockdata(type.ToString(), sToSend);
                string result = JsonConvert.SerializeObject(messageToSend);
                //await SendAsynWait(result);
                var encoded = Encoding.UTF8.GetBytes(result);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                _clientWebSocket.Send(buffer.Array, buffer.Offset, encoded.Length);
                //Task responseTask = _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                // responseTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public async Task SendMessage(string message)
        {
            try
            {                
                csockdata messageToSend = new csockdata(MessageType.Login, message);
                string result = JsonConvert.SerializeObject(messageToSend);
                //await SendAsynWait(result);
                var encoded = Encoding.UTF8.GetBytes(result);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                _clientWebSocket.Send(buffer.Array, buffer.Offset, encoded.Length);
                //Task responseTask = _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                //responseTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public async Task SendFileAction(FileMessage fmessage)
        {
            try
            {
                string fs = JsonConvert.SerializeObject(fmessage);
                csockdata messageToSend = new csockdata(MessageType.FileAction, fs);
                string result = JsonConvert.SerializeObject(messageToSend);
                //await SendAsynWait(result);
                var encoded = Encoding.UTF8.GetBytes(result);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                _clientWebSocket.Send(buffer.Array, buffer.Offset, encoded.Length);
                //Task responseTask = _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                //responseTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        private void Invoke(csockdata message)
        {
            switch (message.MessageType)
            {
                case MessageType.Message:
                    MessageObject objmes = JsonConvert.DeserializeObject<MessageObject>(message.Value, _jsonSerializerSettings);
                    if (OnMessage != null)
                        OnMessage(objmes);
                    break;
                case MessageType.FileAction:
                    FileMessage fileMessage = JsonConvert.DeserializeObject<FileMessage>(message.Value, _jsonSerializerSettings);
                    if (OnFileMessage != null)
                        OnFileMessage(fileMessage);
                    break;
                case MessageType.GetAllFileInformation:
                    List<FileMessage> fileMessages = JsonConvert.DeserializeObject<List<FileMessage>>(message.Value, _jsonSerializerSettings);
                    if (OnGetAllFileInformation != null)
                        OnGetAllFileInformation(fileMessages);
                    break;
            }
        }

        public async Task StopConnectionAsync()
        {
             _clientWebSocket.Close();
            //await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
        }
      
        private async Task Receive(WebSocket clientWebSocket, Action<csockdata> handleMessage)
        {
            //while (_clientWebSocket.State == WebSocketState.Open)
            //{
            //    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
            //    string serializedMessage = null;
            //    WebSocketReceiveResult result = null;
            //    using (var ms = new MemoryStream())
            //    {
            //        do
            //        {
            //            result = await clientWebSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
            //            ms.Write(buffer.Array, buffer.Offset, result.Count);
            //        }
            //        while (!result.EndOfMessage);

            //        ms.Seek(0, SeekOrigin.Begin);

            //        using (var reader = new StreamReader(ms, Encoding.UTF8))
            //        {
            //            serializedMessage = await reader.ReadToEndAsync().ConfigureAwait(false);
            //        }

            //    }

            //    if (result.MessageType == WebSocketMessageType.Text)
            //    {
            //        var message = JsonConvert.DeserializeObject<csockdata>(serializedMessage);
            //        if (string.Compare(message.MessageType, "loginfail", true) == 0)
            //        {
            //            Console.WriteLine("Login fail");
            //            await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
            //            break;
            //        }
            //        else
            //            handleMessage(message);
            //    }

            //    else if (result.MessageType == WebSocketMessageType.Close)
            //    {
            //        await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
            //        break;
            //    }
            //}
        }
    }
}