using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;

// Sockets
using System.Net.Sockets;
using System.Net;

// Threads
using System.Threading;

namespace UDP_Client
{
    class Model : INotifyPropertyChanged
    {
        // some data that keeps track of ports and addresses
        private UInt32 _remotePort = 8000;
        private String _remoteIPAddress = "127.0.0.1";
        // this is the UDP socket that will be used to communicate over the network
        private UdpClient _dataSocket;

        private int _localPort = 9001;
        private String _localIPAddress = "127.0.0.1";

        // this is the thread that will run in the background waiting for incomming data
        private Thread _receiveDataThread;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Model()
        {
            try
            {
                // set up generic UDP socket and bind to local port
                //
                _dataSocket = new UdpClient(_localPort);
            }
            catch (Exception ex)
            {
                StatusBlock = ex.ToString();
                return;
            }
            StartThread();
        }

        private String _statusBlock = String.Empty;
        public String StatusBlock
        {
            get { return _statusBlock; }
            set
            {
                _statusBlock = value;
                OnPropertyChanged("StatusBlock");
            }
        }

        private String _textBlock = String.Empty;
        public String TextBlock
        {
            get { return _textBlock; }
            set
            {
                _textBlock = value;
                OnPropertyChanged("TextBlock");
            }
        }

        private String _loopBlock;
        public String LoopBlock
        {
            get { return _loopBlock; }
            set
            {
                _loopBlock = value;
                OnPropertyChanged("LoopBlock");
            }
        }

        private void ReceiveThreadFunction()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_localIPAddress), (int)_localPort);
            while (true)
            {
                try
                {
                    // wait for data this is a blocking call
                    Byte[] receiveData = _dataSocket.Receive(ref endPoint);

                    // convert byte array to a string
                    LoopBlock = DateTime.Now + ": " + System.Text.Encoding.Default.GetString(receiveData);
                }
                catch (SocketException ex)
                {
                    // got here because either the Receive failed, or more or more likely the socket was destroyed by exiting from the JoystickPositionWindow form MessageBox.Show(ex.ToString(), "UDP Server");
                    StatusBlock = "Error: " + ex.ToString();
                    return;
                }
            }
        }

        public void StartThread()
        {
            // start the thread to listen for data from other UDP peer
            ThreadStart threadFunction = new ThreadStart(ReceiveThreadFunction);
            _receiveDataThread = new Thread(threadFunction);
            _receiveDataThread.Start();
        }


        public void SendMessage()
        {
            IPEndPoint remoteHost = new IPEndPoint(IPAddress.Parse(_remoteIPAddress), (int)_remotePort);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(TextBlock);

            try
            {
                _dataSocket.Send(sendBytes, sendBytes.Length, remoteHost);
            }
            catch (SocketException ex)
            {
                StatusBlock = ex.ToString();
                return;
            }
            StatusBlock = "Message Successfully sent at " + DateTime.Now;
        }

    }
}
