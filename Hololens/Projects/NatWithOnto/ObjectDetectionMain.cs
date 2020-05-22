/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.Shapes;
using Annotations;
using System.Globalization;

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

        Cylinder LeftArea, RightArea;
        Text3DAnnotationList Text3DList;


        //###################################################################################################################
        public MainApp(ApplicationOptions opts) : base(opts)
        {
            /* _ip = "192.168.137.1";
             _cameraSocketPort = "9999";
             _rayCollisionSocketPort = "9998";
             _displaySocketPort = "9997";*/
            CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
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
                if (cmd == "new_text") NewText(args);
                else if (cmd == "update_text") UpdateText(args);
                else if (cmd == "new_area") NewArea(args);
                else if (cmd == "update_area") UpdateArea(args);
            });
        }


        //###################################################################################################################
        public void NewText(string[] args)
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
        public void UpdateText(string[] args)
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
                    annotation.Node.Position = new Vector3(x, y + 0.1f, z);
                    annotation.SetColor(color);
                }
            }
        }


        //###################################################################################################################
        public void NewArea(string[] args)
        {
            var x = float.Parse(args[0]);
            var y = float.Parse(args[1]);
            var z = float.Parse(args[2]);
            var side = args[3];
            var r = float.Parse(args[4]);
            var color = ColorPalette.FromString(args[5]);

            if (side == "left")
            {
                LeftArea = Annotation.Disc(this, color, new Vector3(x, y, z), r);
            }
            else
            {
                RightArea = Annotation.Disc(this, color, new Vector3(x, y, z), r);
            }
                
        }


        //###################################################################################################################
        public void UpdateArea(string[] args)
        {
            var side = args[0];
            var color = ColorPalette.FromString(args[1]);

            if (side == "left")
            {
                LeftArea.Color = color;
            }
            else
            {
                RightArea.Color = color;
            }
        }


        //###################################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            Text3DList.Update(CullingCamera.Node.Rotation);
        }

    }
}