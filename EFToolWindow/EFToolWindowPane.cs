using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using NetMQ;
using NetMQ.Sockets;

namespace Microsoft.EFToolWindow
{
    [Guid(Constants.ToolWindow)]
    public class EFToolWindowPane : ToolWindowPane
    {
        private readonly TextBox _textBox = new TextBox
        {
            IsReadOnly = true
        };

        private SubscriberSocket _subscriberSocket;
        private NetMQPoller _netMqPoller;

        public EFToolWindowPane() :
            base(provider: null)
        {
            Caption = Resources.ToolWindowTitle;
        }

        protected override void Initialize()
        {
            Content = _textBox;

            _subscriberSocket = new SubscriberSocket();
            _subscriberSocket.Options.ReceiveHighWatermark = 1000;
            _subscriberSocket.Connect(address: "tcp://localhost:12345");
            _subscriberSocket.Subscribe(topic: "");
            _subscriberSocket.ReceiveReady += SubSocket_ReceiveReady;

            WriteLine(message: "Subscriber socket connecting...");

            _netMqPoller = new NetMQPoller { _subscriberSocket };
            _netMqPoller.RunAsync();
        }

        private void SubSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var message = e.Socket.ReceiveFrameString();

            _textBox.Dispatcher.Invoke(callback: () => WriteLine(message));
        }

        private void WriteLine(string message)
        {
            _textBox.AppendText(message + "\r\n");
        }

        protected override void Dispose(bool disposing)
        {
            _netMqPoller.Dispose();
            _subscriberSocket.Dispose();

            base.Dispose(disposing);
        }
    }
}