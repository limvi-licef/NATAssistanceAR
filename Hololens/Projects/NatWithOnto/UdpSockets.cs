/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace UdpSockets
{
    //###############################################################################################
    //#                                    BaseUdpSocket                                            #
    //#                                                                                             #
    //#  public BaseUdpSocket()                                                                     #
    //#  public bool IsConnected()                                                                  #
    //#  public void Send(string s)                                                                 #
    //#  public void Send(byte b)                                                                   #
    //#  public void Send(byte[] b, int start, int size)                                            #
    //#  public void Send(byte[] b)                                                                 #
    //#  public async Task SendAsync(byte[] b, int start, int size)                                 #
    //#  public async Task SendAsync(byte[] b)                                                      #
    //#  public async Task<bool> Connect(string host, string port)                                  #
    //#  public async Task AwaitDisconnection()                                                     #
    //#  public bool Close()                                                                        #
    //#                                                                                             #
    //#  protected virtual void ReadMessage(DatagramSocketMessageReceivedEventArgs args)            #
    //#  protected virtual void Reply()                                                             #
    //#                                                                                             #
    //#  private string LocalIp()                                                                   #
    //#  private void BuildConnectSequence()                                                        #
    //#  private void SetAddress(string host, string port)                                          #
    //#  private async Task ConnectRequest()                                                        #
    //#  private async Task Connect()                                                               #
    //#  private void Recv(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)      #
    //#                                                                                             #
    //###############################################################################################
    /// <summary>
    /// Base class representing a socket that could connect a distant host over a private network.
    /// </summary>
    public partial class BaseUdpSocket
    {

        private DatagramSocket _socket;

        private HostName _host;
        private string _port;

        /// <summary>
        /// this is a byte array sended to the host for connection that contains the IP adress and the port to use for communication.
        /// Format : { IP, IP, IP, IP, PORT, PORT}
        /// </summary>
        private byte[] _connect_sequence = new byte[6];

        private bool _connected = false;

        /// <summary>
        /// a stream to use for sending message.
        /// </summary>
        Stream _outStream;


        //###########################################################################################
        //####################################      public      #####################################


        //###########################################################################################
        /// <summary>
        /// Constructor that create the UDP socket make it ready to receive
        /// </summary>
        public BaseUdpSocket()
        {
            _socket = new DatagramSocket();
            _socket.MessageReceived += Recv; // message treatment at RECV method
        }


        //###########################################################################################
        /// <summary>
        /// Indicate if socket is connected to a distant host.
        /// </summary>
        /// <returns> true : the socket is connected </returns>
        public bool IsConnected() { return _connected; }


        //###########################################################################################
        /// <summary>
        /// Send a message to host.
        /// </summary>
        /// <param name="s"> message as string </param>
        public void Send(string s)
        {
            var streamWriter = new StreamWriter(_outStream); // allow to write string in the stream
            streamWriter.Write(s);
            streamWriter.Flush();
        }


        //###########################################################################################
        /// <summary>
        /// Send a message to host.
        /// </summary>
        /// <param name="b"> a single byte </param>
        public void Send(byte b)
        {
            _outStream.WriteByte(b);
            _outStream.Flush();
        }


        //###########################################################################################
        /// <summary>
        /// Send a message to host.
        /// </summary>
        /// <param name="b"> the message as byte array </param>
        /// <param name="start"> the start index for reading the byte array </param>
        /// <param name="len"> the length of byte array to read </param>
        public void Send(byte[] b, int start, int len)
        {
            // if message length is bigger than byte array length from start index
            if (len > b.Length - start) len = b.Length - start;

            _outStream.Write(b, start, len);
            _outStream.Flush();
        }


        //###########################################################################################
        /// <summary>
        /// Send a message to host. The byte array is fully sended.
        /// </summary>
        /// <param name="b"> the message as byte array </param>
        public void Send(byte[] b) => Send(b, 0, b.Length);


        //###########################################################################################
        /// <summary>
        /// Send a message to host using asynchronous method.
        /// </summary>
        /// <param name="b"> the message as byte array </param>
        /// <param name="start"> the start index for reading the byte array </param>
        /// <param name="len"> the length of byte array to read </param>
        public async Task SendAsync(byte[] sequence, int start, int size)
        {
            // if message length is bigger than byte array length from start index
            if (size > sequence.Length - start) size = sequence.Length - start;

            await _outStream.WriteAsync(sequence, start, size);
            _outStream.Flush();
        }


        //###########################################################################################
        /// <summary>
        /// Send a message to host using asynchronous method. The byte array is fully sended.
        /// </summary>
        /// <param name="b"> the message as byte array </param>
        public async Task SendAsync(byte[] sequence) => await SendAsync(sequence, 0, sequence.Length);


        //###########################################################################################
        /// <summary>
        /// Try to connect a distant host. Awaitable.
        /// </summary>
        /// <param name="host"> distant host represented by IP adress </param>
        /// <param name="port"> port number to reach </param>
        /// <returns> true : the connection is done </returns>
        public async Task<bool> Connect(string host, string port)
        {
            if (!_connected)
            {
                SetAddress(host, port);
                try
                {
                    await Connect();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return false;
                }
                return true;
            }
            return false;
        }


        //###########################################################################################
        /// <summary>
        /// Awaitable method that indicate when socket is disconnected.
        /// </summary>
        public async Task AwaitDisconnection()
        {
            while (_connected)
            {
                await Task.Delay(1);
            }
        }


        //###########################################################################################
        /// <summary>
        /// Close the connection to distant host. Indicate if the operation works by returning boolean.
        /// It work as well if the socket is not connected (and return true).
        /// </summary>
        /// <returns> true : the socket is disconnected </returns>
        public bool Close()
        {
            if (_connected)
            {
                _socket.Dispose();
                _socket = null;

                _connected = false;
                return true;
            }
            return false;
        }


        //###########################################################################################
        //##################################    protected     #######################################


        //###########################################################################################
        /// <summary>
        /// Overridable method for specify how to read the received message.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void ReadMessage(DatagramSocketMessageReceivedEventArgs args) { }


        //###########################################################################################
        /// <summary>
        ///  Overridable method for setting a reply when a message is received.
        /// </summary>
        protected virtual void Reply() { }


        //###########################################################################################
        //#################################     private      ########################################


        //###########################################################################################
        /// <summary>
        /// Check the local IP adress of the device itself.
        /// If the current adress format is not IPv4, return the loopback IPv4 adress.
        /// </summary>
        /// <returns> IPv4 local adress </returns>
        private string LocalIp()
        {
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.Type == HostNameType.Ipv4)
                {
                    return localHostName.ToString();
                }
            }
            return "127.0.0.1";
        }


        //###########################################################################################
        /// <summary>
        /// Build the sequence that must be send to the host to reach.
        /// </summary>
        private void BuildConnectSequence()
        {
            // str host to byte host
            string[] str_ip = LocalIp().Split('.');
            byte[] byte_ip = new byte[4];
            for (int n = 0; n < 4; n++) // convert each value from str to byte and put it in the connect sequence
            {
                byte_ip[n] = Byte.Parse(str_ip[n]);
                _connect_sequence[n] = byte_ip[n];
            }

            // str port to byte port
            int int_port = Int32.Parse(_port); // convert str to int
            byte[] byte_port = BitConverter.GetBytes(int_port); // convert int to byte
            // put it inside connect sequence
            _connect_sequence[4] = byte_port[1];
            _connect_sequence[5] = byte_port[0];
        }


        //###########################################################################################
        /// <summary>
        /// Set host and port parameters as attributes and build the coresponding connect sequence.
        /// </summary>
        /// <param name="host"> string representing the IP adress </param>
        /// <param name="port"> string representing the port to reach </param>
        private void SetAddress(string host, string port)
        {
            _host = new HostName(host);
            _port = port;

            BuildConnectSequence();
        }


        //###########################################################################################
        /// <summary>
        /// Method for connection to host. While not connected, send the connect sequence and wait 250ms for a reply.
        /// </summary>
        private async Task ConnectRequest()
        {
            do
            {
                await SendAsync(_connect_sequence);
                await Task.Delay(250);
            }
            while (!_connected);
        }


        //###########################################################################################
        /// <summary>
        /// Connection to the distant host. Close the socket if it's already connected.
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            if (_connected) Close();

            // check the local ip for creating EndpointPair that link the socket and the distant host
            var local_ip = LocalIp();
            var endpoint = new EndpointPair(new HostName(local_ip), _port, _host, _port);

            // build the link and get the output strem for sending message
            await _socket.ConnectAsync(endpoint);
            _outStream = _socket.OutputStream.AsStreamForWrite();

            // try to connect
            await ConnectRequest();
        }


        //###########################################################################################
        /// <summary>
        /// Treatment when receive a message. Read the message with <see cref="ReadMessage"/> and decide what to do with <see cref="Reply"/>
        /// </summary>
        /// <param name="sender"> the socket that raised this event </param>
        /// <param name="args"> contains information about message and message itself </param>
        private void Recv(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            if (!_connected)
            {
                _connected = true;
            }
            else
            {
                ReadMessage(args);
                Reply();
            }
        }

    }



    //###############################################################################################
    //#                             DataUdpSocket : BaseUdpSocket                                   #
    //#                                                                                             #
    //# public void BindData(byte[] data, int packet_size)                                          #
    //#                                                                                             #
    //# protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)            #
    //# protected virtual void PrepareData()                                                        #
    //# protected override void Reply()                                                             #
    //# protected void SendNbPacket()                                                               #
    //#                                                                                             #
    //# private void SendPacket()                                                                   #
    //#                                                                                             #
    //###############################################################################################
    /// <summary>
    /// Inherited from BaseUdpSocket. Socket designed for send data as long message.
    /// </summary>
    public partial class DataUdpSocket : BaseUdpSocket
    {

        /// <summary>
        /// the data to send when asked by the host.
        /// </summary>
        private byte[] _data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        /// <summary>
        /// parameter that indicate the size of requested packets
        /// </summary>
        private int _packet_size = 2;

        /// <summary>
        /// the number of packet necessary for sending all data
        /// </summary>
        private int _nb_packet;

        /// <summary>
        /// the current packet number during the sending
        /// </summary>
        private int _current_packet;

        /// <summary>
        /// the received message. For this socket, it's only a byte of value 255 for new request, 254 for asking packet, 1 for close
        /// </summary>
        private byte _message;


        //###########################################################################################
        /// <summary>
        /// Set attributes for coresponding to new data
        /// </summary>
        /// <param name="data"> data format to a byte array </param>
        /// <param name="packet_size"> the packet size number for data segmentation, must not be greater than </param>
        public void BindData(byte[] data, int packet_size)
        {
            _data = data;
            _packet_size = packet_size;

            SetNbPacket();
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method that read message as a single byte.
        /// </summary>
        /// <param name="args"> message informations </param>
        protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                _message = dataReader.ReadByte();
            }
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method for coonfigure packet number and send it to confirm reception.
        /// By default, set data and confirm request by sending the number of packets to receive.
        /// </summary>
        protected virtual void PrepareData()
        {
            SetNbPacket();
            SendNbPacket();
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method that specify action to do depending on message
        /// </summary>
        protected override void Reply()
        {
            // new data
            if (_message == 255)
            {
                PrepareData();
                _current_packet = 0;
            }
            // send next packet
            else if (_message == 254)
            {
                SendPacket();
                _current_packet += 1;
            }
            // close socket
            else if (_message == 1)
            {
                Close();
            }
        }


        //###########################################################################################
        /// <summary>
        /// Send the number of packet as a byte using BitConverter object
        /// </summary>
        protected void SendNbPacket() => Send(BitConverter.GetBytes(_nb_packet)[0]);


        //###########################################################################################
        /// <summary>
        /// Calculate the number of packet needed for sending all data
        /// </summary>
        private void SetNbPacket()
        {
            if (_data.Length % _packet_size == 0)
            {
                _nb_packet = _data.Length / _packet_size;
            }
            else
            {
                _nb_packet = _data.Length / _packet_size + 1;
            }
        }


        //###########################################################################################
        /// <summary>
        /// Specific sending for a packet.
        /// </summary>
        private void SendPacket()
        {
            if (_current_packet < _nb_packet)
            {
                Send(_data, _current_packet * _packet_size, _packet_size);
            }
        }

    }



    //###############################################################################################
    //#                             DisplayUdpSocket : BaseUdpSocket                                #
    //#                                                                                             #
    //#  protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)           #
    //#  protected override void Reply()                                                            #
    //#  protected void Parse()                                                                     #
    //#                                                                                             #
    //###############################################################################################
    /// <summary>
    /// Inherited from BaseUdpSocket. Specific socket for displaying a UrhoSharp scene.
    /// </summary>
    public partial class DisplayUdpSocket : BaseUdpSocket
    {
        /// <summary>
        /// message is a command for display format : command;vectorx;vectory;vectorz;arg1;arg2;...
        /// </summary>
        protected string _message;
        protected string _cmd;
        protected string[] _args;

        /// <summary>
        /// Callback designed for calling a method of the main app in order to update its display
        /// </summary>
        /// <param name="cmd"> the display command </param>
        /// <param name="args"> the args </param>
        public delegate void DisplayCallback(string cmd, string[] args);
        public DisplayCallback OnReceivedDisplayCommand;


        //###########################################################################################
        /// <summary>
        /// Overrided method that read message as a string.
        /// </summary>
        /// <param name="args"> message informations </param>
        protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                _message = dataReader.ReadString(dataReader.UnconsumedBufferLength).Trim();
            }
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method that specify action to do after receiving a message.
        /// Analyze the message, do the coresponding display and send a byte to confirm.
        /// </summary>
        protected override void Reply()
        {
            Parse();
            OnReceivedDisplayCommand?.Invoke(_cmd, _args);
            Send(255);
        }


        //###########################################################################################
        /// <summary>
        /// Extract from message a command and optional parameters. Calculate the number of arguments
        /// </summary>
        protected virtual void Parse()
        {
            var message = _message.Split(';');

            // command is the first argument
            _cmd = message[0];

            // other arguments are copied to dedied array
            _args = new string[message.Length-1];
            Array.Copy(message, 1, _args, 0, _args.Length);
        }

    }


    //###############################################################################################
    //#                            RayCollisionUdpSocket : BaseUdpSocket                            #
    //#                                                                                             #
    //#  public void RayHitPositionReceived(float x, float y, float z)                              #
    //#                                                                                             #
    //#  protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)           #
    //#  protected override void Reply()                                                            #
    //#                                                                                             #
    //###############################################################################################
    /// <summary>
    /// Inherited from BaseUdpSocket. Specific socket for interact with collision 
    /// </summary>
    public partial class RayCollisionUdpSocket : BaseUdpSocket
    {
        /// <summary>
        /// number of reply to send coresponding to the number of requested ray hit test
        /// </summary>
        int _nb_reply;


        protected string _message;
        protected string _reply;

        public delegate void Callback(float x, float y);
        public Callback DepthTestFunction;


        //###########################################################################################
        /// <summary>
        /// Method to set as callback to the collision application for receive th result of requested ray hit test
        /// </summary>
        /// <param name="x"> x world position (meters) </param>
        /// <param name="y"> y world position (meters) </param>
        /// <param name="z"> z world position (meters) </param>
        public void RayHitPositionReceived(float x, float y, float z)
        {
            _reply += string.Format("{0},{1},{2};", x.ToString(), y.ToString(), z.ToString());
            _nb_reply--;

            if (_nb_reply == 0) Send(_reply);
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method that read message as a string.
        /// </summary>
        /// <param name="args"> message informations </param>
        protected override void ReadMessage(DatagramSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                _message = dataReader.ReadString(dataReader.UnconsumedBufferLength).Trim();
            }
        }


        //###########################################################################################
        /// <summary>
        /// Overrided method that specify action to do after receiving a message.
        /// Split the message in order to get the list of received command.
        /// </summary>
        protected override void Reply()
        {
            if (_message != "" && _message.Length > 1)
            {
                var message = _message.Split(';');

                _reply = "";
                _nb_reply = message.Length;

                foreach (var m in message)
                {
                    var coords = m.Split(',');
                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[1], CultureInfo.InvariantCulture);

                    DepthTestFunction?.Invoke(x, y);
                }
            }
        }

    }

}
