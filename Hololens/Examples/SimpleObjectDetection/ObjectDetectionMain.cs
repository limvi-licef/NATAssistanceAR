/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Annotations;

namespace Application
{

    //#######################################################################################################################
    //#######################################################################################################################
    internal class Program
    {
        [MTAThread]
        static void Main()
        {
            var appViewSource = new UrhoAppViewSource<MainApp>(new ApplicationOptions("Data"));
            CoreApplication.Run(appViewSource);
        }
    }


    //#######################################################################################################################
    //#######################################################################################################################
    public class MainApp : HoloObjectDetectionApp
    {

        Text3DAnnotationList Text3DList;


        //###################################################################################################################
        public MainApp(ApplicationOptions opts) : base(opts)
        {
            _ip = "192.168.137.1";
            _cameraSocketPort = "9999";
            _rayCollisionSocketPort = "9998";
            _displaySocketPort = "9997";
        }


        //###################################################################################################################
        protected override void Init()
        {
            Text3DList = new Text3DAnnotationList(this);
        }


        //###################################################################################################################
        protected override void OnReceivedDisplayCommand(string cmd, string[] args)
        {
            Urho.Application.InvokeOnMain(() =>
            {
                if (cmd == "new") New(args);
                else if (cmd == "update") Upd(args);
            });
        }


        //###################################################################################################################
        public void New(string[] args)
        {
            var x = float.Parse(args[0]);
            var y = float.Parse(args[1]);
            var z = float.Parse(args[2]);
            var text = args[3];

            var pos = new Vector3(x, y, z);
            var annotation = Annotation.Text(this, text, pos);

            Text3DList.Add(annotation);
        }


        //###################################################################################################################
        public void Upd(string[] args)
        {
            var x = float.Parse(args[0]);
            var y = float.Parse(args[1]);
            var z = float.Parse(args[2]);
            var text = args[3];
            var color = ColorPalette.FromString(args[4]);

            foreach (var annotation in Text3DList)
            {
                if (annotation.Text == text)
                {
                    annotation.Node.Position = new Vector3(x, y + 0.05f, z);
                    annotation.SetColor(color);
                }
            }
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            Text3DList.Update(CullingCamera.Node.Rotation);
        }

    }
}