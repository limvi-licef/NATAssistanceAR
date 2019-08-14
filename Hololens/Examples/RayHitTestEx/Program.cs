/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.SharpReality;
using Urho.Shapes;


namespace RayHitTestEx
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
    public class MainApplication : StereoApplication
    {
        Sphere sphere;

        public MainApplication(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        protected override async void Start()
        {
            base.Start();

            sphere = Scene.CreateChild().CreateComponent<Sphere>(); // add a sphere to the scene
            sphere.Node.SetScale(0.1f); // set sphere diameter to 10cm
            sphere.ViewMask = 0x80000000; // avoid ray hits

            // range x=5m, y=3m, z=5m, nb triangles/m3 = 1200
            await StartSpatialMapping(new Vector3(5, 3, 5), 1200);
        }


        //###################################################################################################################
        public override void OnSurfaceAddedOrUpdated(SpatialMeshInfo surface, Model generatedModel)
        {
            // create a node for spatial mapping if not exist
            Node node = this.Scene.GetChild(surface.SurfaceId, false);
            if (node == null) node = this.Scene.CreateChild(surface.SurfaceId);

            // add the spatial map to the node
            StaticModel staticModel = node.CreateComponent<StaticModel>();
            staticModel.Model = generatedModel;

            // set position and orientation of the map according to the world scene
            node.Position = surface.BoundsCenter;
            node.Rotation = surface.BoundsRotation;

            // stop spatial mapping to avoid slowness
            StopSpatialMapping();
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            Ray cameraRay = CullingCamera.GetScreenRay(0.5f, 0.5f); // ray coresponding to center of field of view
            // hit test using the previous ray
            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);

            if (result != null) // if the ray hit something
            {
                // display green sphere at ray hit position
                sphere.Node.Position = result.Value.Position;
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