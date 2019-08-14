/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Annotations;
using Collisions;
using UdpCameras;
using UdpSockets;


namespace Application
{

    //#######################################################################################################################
    //#######################################################################################################################
    internal class Program
    {
        [MTAThread]
        static void Main()
        {
            var appViewSource = new UrhoAppViewSource<HoloObjectDetectionApp>(new ApplicationOptions("Data"));
            CoreApplication.Run(appViewSource);
        }
    }


    //#######################################################################################################################
    //#######################################################################################################################
    public class HoloObjectDetectionApp : CollisionApplication
    {
        protected string _ip = "IPV4 ADRESS";
        protected string _cameraSocketPort = "xxxx";
        protected string _rayCollisionSocketPort = "xxxx";
        protected string _displaySocketPort = "xxxx";

        UdpFRCamera _fRCamera;
        RayCollisionUdpSocket _rcSocket;
        DisplayUdpSocket _socket;

        //
        // define objects used for display
        //


        //###################################################################################################################
        public HoloObjectDetectionApp(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        protected override async void Start()
        {
            base.Start(); // call the inherited method

            //
            // insert display initialisation here
            //

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
        private void DepthTestFunction(float x, float y)
        {
            this.RayHitPositionAsync(CullingCamera, x, y);
        }


        //###################################################################################################################
        protected override void OnReceivedDisplayCommand(string cmd, string[] args)
        {
            Urho.Application.InvokeOnMain(() =>
            {
                //
                //  update the display according to the received command
                //
            });
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            //
            // update the display for each frame
            //
        }

    }

}