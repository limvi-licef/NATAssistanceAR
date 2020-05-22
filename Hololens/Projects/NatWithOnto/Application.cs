/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using Urho;

using Collisions;
using UdpCameras;
using UdpSockets;


namespace Application
{

    //#######################################################################################################################
    //#                                     HoloObjectDetectionApp : CollisionApplication                                   #
    //#                                                                                                                     #
    //#  public HoloObjectDetectionApp(ApplicationOptions opts)                                                             #
    //#                                                                                                                     #
    //#  protected override async void Start()                                                                              #
    //#  protected virtual void OnReceivedDisplayCommand(string cmd, string[] args)                                         #
    //#  protected virtual void Init()                                                                                      #
    //#                                                                                                                     #
    //#  private void DepthTestFunction(float x, float y)                                                                   #
    //#                                                                                                                     #
    //#######################################################################################################################
    /// <summary>
    /// Inherited from CollisionApplication. An UrhoSharp application class with integrated sockets.
    /// Designed to be inherited for a fast implementation of an application that need object detection.
    /// </summary>
    public class HoloObjectDetectionApp : CollisionApplication
    {
        
        protected static string _ip = "192.168.1.21";
        protected string _cameraSocketPort = "9999";
        protected string _rayCollisionSocketPort = "9998";
        protected string _displaySocketPort = "9997";

        UdpFRCamera _fRCamera;
        RayCollisionUdpSocket _rcSocket;
        DisplayUdpSocket _socket;


        //###################################################################################################################
        /// <summary>
        /// Constructor that call UrhoSharp StereoApplication constructor.
        /// </summary>
        /// <param name="opts"> UrhoSharp parameters </param>
        public HoloObjectDetectionApp(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        /// <summary>
        /// The start method of an UrhoSharp StereoApplication class. Configure the sockets an launch the connections.
        /// This must not be overrided, use <see cref="Init"/> instead.
        /// </summary>
        protected override async void Start()
        {
            base.Start(); // call the inherited method
            Init(); // display initialisation

            // create and start the camera socket
            _fRCamera = new UdpFRCamera();
            await _fRCamera.Start();

            // create the ray collision socket and bind callback methods for ray hit test
            _rcSocket = new RayCollisionUdpSocket();
            _rcSocket.DepthTestFunction += this.DepthTestFunction; // the socket will call this.DepthTestFunction for a request of ray hit test
            this.RayHitPositionReceived += _rcSocket.RayHitPositionReceived; // the app will call Socket.RayHitPositionReceived for submit 3D coordinate found

            // create the display socket and bind the method 
            _socket = new DisplayUdpSocket();
            _socket.OnReceivedDisplayCommand += this.OnReceivedDisplayCommand; // the socket will call this.OnReceivedDisplayCommand when it receive a command

            // connection for all the sockets
            await _fRCamera.Connect(_ip, _cameraSocketPort);
            await _rcSocket.Connect(_ip, _rayCollisionSocketPort);
            await _socket.Connect(_ip, _displaySocketPort);
        }


        //###################################################################################################################
        /// <summary>
        /// Method called by the display socket. 
        /// </summary>
        /// <param name="cmd"> the command that indicate wich display is requested </param>
        /// <param name="args"> the array containing the arguments </param>
        protected virtual void OnReceivedDisplayCommand(string cmd, string[] args) { }


        //###################################################################################################################
        /// <summary>
        /// An equivalent of Start method for setting the application display.
        /// </summary>
        protected virtual void Init() { }


        //###################################################################################################################
        /// <summary>
        /// Method called by the ray collision socket to do depth test. See <see cref="RayHitPositionAsync"/> from <see cref="RayCollisionUdpSocket"/>.
        /// </summary>
        /// <param name="x"> x axis coordinate (normalised) </param>
        /// <param name="y"> y axis coordinate (normalised) </param>
        private void DepthTestFunction(float x, float y)
        {
            this.RayHitPositionAsync(CullingCamera, x, y);
        }


    }

}