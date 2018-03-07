using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using static FileTracking.Client.Common.WSCommonObject;

namespace FileTracking.Client.WebSocketManager
{
    public class WSConnectionNew
    {
        private WebSocket _clientWebSocket { get; set; }
        private LoginObject UserInfor { get; set; }
        public bool Connected { get { return _clientWebSocket.State == WebSocketState.Open; } }

        public delegate void OnMessageEvent(MessageObject message);
        public event OnMessageEvent OnMessage;

        public delegate void OnFileMessageEvent(FileMessage fileMessage);
        public event OnFileMessageEvent OnFileMessage;

        public delegate void OnGetAllFileInformationEvent(List<FileMessage> fileMessages);
        public event OnGetAllFileInformationEvent OnGetAllFileInformation;

        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public WSConnectionNew(string uri)
        {
            _clientWebSocket = new WebSocket(uri);
            _clientWebSocket.Opened += new EventHandler(Websocket_Opened);
            _clientWebSocket.Closed += new EventHandler(Websocket_Closed);
            _clientWebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(Websocket_MessageReceived);
        }
        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {            
            var message = JsonConvert.DeserializeObject<csockdata>(e.Message);
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

        private void Websocket_Closed(object sender, EventArgs e)
        {
            //Console.WriteLine("Closed");
        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            //LoginObject UserInfor = new LoginObject();
            SendLogin();
        }

        public async Task SendLogin()
        {
            try
            {
                string ls = JsonConvert.SerializeObject(UserInfor);
                csockdata messageToSend = new csockdata(MessageType.Login, ls);
                string result = JsonConvert.SerializeObject(messageToSend);
                _clientWebSocket.Send(result);
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
                _clientWebSocket.Send(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public async Task SendToListUser(APIMessageObject data)
        {
            try
            {
                string ls = JsonConvert.SerializeObject(data);
                csockdata messageToSend = new csockdata(MessageType.APICommand, ls);
                string result = JsonConvert.SerializeObject(messageToSend);
                _clientWebSocket.Send(result);
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
                _clientWebSocket.Send(result);
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
                csockdata messageToSend = new csockdata(MessageType.Message, message);
                string result = JsonConvert.SerializeObject(messageToSend);
                _clientWebSocket.Send(result);
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
                _clientWebSocket.Send(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        public async Task StartConnectionAsync(LoginObject user)
        {
            UserInfor = user;
            _clientWebSocket.Open();
        }
        public async Task StopConnectionAsync()
        {
            _clientWebSocket.Close();
        }
    }
}

