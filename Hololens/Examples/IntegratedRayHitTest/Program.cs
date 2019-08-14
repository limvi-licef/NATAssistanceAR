/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.Shapes;

using Collisions;


namespace IntegratedRayHitTest
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


        //###################################################################################################################
        public MainApplication(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        protected override void Start()
        {
            base.Start();

            //sphere = this.Scene.CreateChild().CreateComponent<Sphere>();
            sphere = this.Scene.CreateChild().CreateComponent<Sphere>();
            sphere.Node.SetScale(0.1f);
            sphere.ViewMask = 0x80000000; // hide from raycasts
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            var pos = RayHitPosition(CullingCamera, 0.5f, 0.5f);

            if (pos != new Vector3(9,9,9)) // if the ray hit something
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
