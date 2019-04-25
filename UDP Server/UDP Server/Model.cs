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

namespace UDP_Server
{
    class Model : INotifyPropertyChanged
    {

        // some data that keeps track of ports and addresses
        private int _localPort = 8000;
        private string _localIPAddress = "127.0.0.1";
        // this is the thread that will run in the background waiting for incomming data
        private Thread _receiveDataThread;
        // this is the UDP socket that will be used to communicate over the network
        private UdpClient _dataSocket;


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
                MessageBox = "UDP Server is Running!!\n";
                _dataSocket = new UdpClient(_localPort);
            }
            catch(Exception e)
            {
                MessageBox = "Error: " + e;
            }
            StartThread();
        }

        private String _messageBox = String.Empty;
        public String MessageBox
        {
            get { return _messageBox; }
            set
            {
                _messageBox = value;
                OnPropertyChanged("MessageBox");
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
                    MessageBox = DateTime.Now + ": " + System.Text.Encoding.Default.GetString(receiveData);

                    //For loop back
                    _dataSocket.Send(receiveData, receiveData.Length, endPoint);
                }
                catch (SocketException ex)
                {
                    // got here because either the Receive failed, or more or more likely the socket was destroyed by exiting from the JoystickPositionWindow form MessageBox.Show(ex.ToString(), "UDP Server");
                    MessageBox = "Error: " + ex.ToString();
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
    }

}

