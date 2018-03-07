using FileTracking.Client.Common;
using FileTracking.Client.WebSocketManager;
using System.Windows.Forms;

namespace FileTracking.Client.Application
{
    public class STAApplicationContext : ApplicationContext
    {
        public STAApplicationContext(RegClientInfor clientInfor)
        {
            _viewManager = new ViewManager(clientInfor);

            //_clientWebSocketManager.OnStatusChange += _viewManager.OnStatusChange;            
        }

        private ViewManager _viewManager;

        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {
            _viewManager = null;
        }
    }
}
