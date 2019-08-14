/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using Urho;
using Urho.SharpReality;


namespace Collisions
{
    //#######################################################################################################################
    //#                                    CollisionApplication : StereoApplication                                         #
    //#                                                                                                                     #
    //#  public CollisionApplication(ApplicationOptions opts)                                                               #
    //#  public override void OnSurfaceAddedOrUpdated(SpatialMeshInfo surface, Model generatedModel)                        #
    //#  public Vector3 RayHitPosition(Camera camera, float x, float y)                                                     #
    //#  public void RayHitPositionAsync(Camera camera, float x, float y)                                                   #
    //#                                                                                                                     #
    //#  protected override async void Start()                                                                              #
    //#                                                                                                                     #
    //#  private RayQueryResult? RayHit(Camera camera, float x, float y)                                                    #
    //#                                                                                                                     #
    //#######################################################################################################################
    /// <summary>
    /// UrhoSharp application with integrated collision gesture.
    /// </summary>
    public partial class CollisionApplication : StereoApplication
    {
        /// <summary>
        /// Used to hide mapped surfaces.
        /// </summary>
        Material _transparentMaterial;

        /// <summary>
        /// Event used to get the result of ray hit test.
        /// </summary>
        /// <param name="x"> x coordinate </param>
        /// <param name="y"> y coordinate </param>
        /// <param name="z"> z coordinate </param>
        public delegate void RayHitEvent(float x, float y, float z);
        public RayHitEvent RayHitPositionReceived;


        //###################################################################################################################
        /// <summary>
        /// Application constructor.
        /// </summary>
        /// <param name="opts"> options that are used to launch application </param>
        public CollisionApplication(ApplicationOptions opts) : base(opts) { }


        //###################################################################################################################
        /// <summary>
        /// Event raised for each surface detected.
        /// </summary>
        /// <param name="surface"> the surface detected </param>
        /// <param name="generatedModel"> the collision map  </param>
        public override void OnSurfaceAddedOrUpdated(SpatialMeshInfo surface, Model generatedModel)
        {
            // create a node for spatial mapping if not exist
            Node node = this.Scene.GetChild(surface.SurfaceId, false);
            if (node == null) node = this.Scene.CreateChild(surface.SurfaceId);

            // add the spatial map to the node and set it transparent
            StaticModel staticModel = node.CreateComponent<StaticModel>();
            staticModel.Model = generatedModel;
            staticModel.Material = _transparentMaterial;

            // set position and orientation of the map according to the world scene
            node.Position = surface.BoundsCenter;
            node.Rotation = surface.BoundsRotation;

            // stop spatial mapping to avoid slowness
            StopSpatialMapping();
        }


        //###################################################################################################################
        /// <summary>
        /// Synchronous method for calling ray hit test. Must be called in update application method.
        /// In case of null ray value, return (9,9,9) vector.
        /// </summary>
        /// <param name="camera"> the camera where the hit test start, UrhoSharp CullingCamera recommanded </param>
        /// <param name="x"> ray position on x axis from left up corner of camera view, normalised value </param>
        /// <param name="y"> ray position on y axis from left up corner of camera view, normalised value </param>
        /// <returns> the world position of the hit. If ray is null, return a vector of value (9,9,9) </returns>
        public Vector3 RayHitPosition(Camera camera, float x, float y)
        {
            var ray = RayHit(camera, x, y); // main ray hit method

            if (ray != null)
            {
                return ray.Value.Position;
            }
            else
            {
                return new Vector3(9,9,9);
            }
        }


        //###################################################################################################################
        /// <summary>
        /// Asynchronous method for calling ray hit test.
        /// The result are returned using a RayHitEvent to call overridable method RayHitPositionReceived.
        /// </summary>
        /// <param name="camera"> the camera where the hit test start, UrhoSharp CullingCamera recommanded </param>
        /// <param name="x"> ray position on x axis from left up corner of camera view, normalised value </param>
        /// <param name="y"> ray position on y axis from left up corner of camera view, normalised value </param>
        public void RayHitPositionAsync(Camera camera, float x, float y)
        {
            // to avoid multiple i/o conflict due to asychronous call, use the application dispatcher
            Urho.Application.InvokeOnMain(() =>
            {
                var vector = RayHitPosition(camera, x, y);
                RayHitPositionReceived?.Invoke(vector.X, vector.Y, vector.Z);
            });
        }


        //###################################################################################################################
        /// <summary>
        /// Overrided method called at the beginning of the application. Create transparent material and start the spatial mapping.
        /// </summary>
        protected override async void Start()
        {
            base.Start(); // inherited method

            _transparentMaterial = new Material();
            _transparentMaterial.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);

            // range x=5m, y=3m, z=5m, nb triangles/m3 = 1200
            await StartSpatialMapping(new Vector3(5, 3, 5), 1200);
        }


        //###################################################################################################################
        /// <summary>
        /// Ray hit test in space from 2D coordinate of a camera field of view.
        /// </summary>
        /// <param name="camera"> the camera where the hit test start, UrhoSharp CullingCamera recommanded </param>
        /// <param name="x"> ray position on x axis from left up corner of camera view, normalised value </param>
        /// <param name="y"> ray position on y axis from left up corner of camera view, normalised value </param>
        /// <returns> ray hit test information. If no collision detected is detected, return null </returns>
        private RayQueryResult? RayHit(Camera camera, float x, float y)
        {
            Ray cameraRay = CullingCamera.GetScreenRay(x, y); // ray coresponding to center of field of view
            // hit test using the previous ray
            var result = Scene.GetComponent<Octree>().RaycastSingle(cameraRay, RayQueryLevel.Triangle, 100, DrawableFlags.Geometry, 0x70000000);

            return result;
        }

    }

}