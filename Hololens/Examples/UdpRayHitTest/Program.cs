/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.Shapes;

using Collisions;
using UdpSockets;


namespace UdpRayHitTest
{

    internal class Program
    {
        [MTAThread]
        static void Main()
        {
            var appViewSource = new UrhoAppViewSource<MainApplication>(new ApplicationOptions("Data"));
            CoreApplication.Run(appViewSource);
        }
    }

    //#######################################################################################################################
    //#######################################################################################################################
    public class MainApplication : CollisionApplication
    {

        Sphere sphere;
        RayCollisionUdpSocket socket;

        //###################################################################################################################
        public MainApplication(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        protected override async void Start()
        {
            base.Start();

            sphere = this.Scene.CreateChild().CreateComponent<Sphere>();
            sphere.Node.SetScale(0.1f);
            sphere.ViewMask = 0x80000000; // hide from raycasts

            socket = new RayCollisionUdpSocket();
            socket.DepthTestFunction += this.DepthTestFunction; // the socket will call DepthTestFunction for each 2D coordinate
            this.RayHitPositionReceived += socket.RayHitPositionReceived; // the app will call socket.RayHitPositionReceived for each 3D coordinate found

            await socket.Connect("192.168.137.1", "9999");
        }


        //###################################################################################################################
        public void DepthTestFunction(float x, float y)
        {
            this.RayHitPositionAsync(CullingCamera, x, y);
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            var pos = RayHitPosition(CullingCamera, 0.5f, 0.5f);

            if (pos != new Vector3(9, 9, 9)) // if the ray hit something
            {
                // display green sphere at ray hit position
                sphere.Node.Position = pos;
                sphere.Color = Color.Green;
            }
            else
            {
                // display red sphere at max range of ray hit test
                sphere.Node.Position = CullingCamera.Node.Position + CullingCamera.Node.Rotation * new Vector3(0, 0, 5);
                sphere.Color = Color.Red;
            }
        }


    }


}
