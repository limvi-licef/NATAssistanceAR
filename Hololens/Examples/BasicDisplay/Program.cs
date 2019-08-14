/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.SharpReality;

using Annotations;


namespace BasicDisplay
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


    //############################################################################################################
    //############################################################################################################
    public class MainApplication : StereoApplication
    {
        
        Text3DAnnotationList Text3DList;


        //########################################################################################################
        public MainApplication(ApplicationOptions opts) : base(opts) { }


        //########################################################################################################
        protected override async void Start()
        {
            base.Start(); // call the inherited masked method

            this.EnableGestureTapped = true; // activate the controls by gesture = simple and double clic
            Text3DList = new Text3DAnnotationList(this); // initialise the list

            await TextToSpeech("Start"); // a signal that indicate that the app is ready
        }


        //########################################################################################################
        public override void OnGestureTapped()
        {
            var text = Text3DList.Count.ToString(); // return the number of annotation already displayed
            var pos =  CullingCamera.Node.Position + CullingCamera.Node.Rotation * new Vector3(0, 0, 1.5f); // 1.5m forward user

            // create a new node in scene and keep it in memory by append it in the text3d list
            var annotation = Annotation.Text(this, text, pos);
            Text3DList.Add(annotation);
        }


        //########################################################################################################
        public override void OnGestureDoubleTapped()
        {
            Text3DList.Remove(); // remove all annotations in the scene
        }


        //########################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            Text3DList.Update(CullingCamera.Node.Rotation); // update text orientation in order to facing the user
        }


    }
}