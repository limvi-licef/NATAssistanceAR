/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.ApplicationModel.Core;

using Urho;
using Urho.SharpReality;

using UdpSockets;
using Annotations;


namespace UdpDisplay
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
        DisplayUdpSocket socket;


        //########################################################################################################
        public MainApplication(ApplicationOptions opts) : base(opts) { }


        //########################################################################################################
        protected override async void Start()
        {
            base.Start();
            
            Text3DList = new Text3DAnnotationList(this);
            socket = new DisplayUdpSocket();

            // the socket will call this.OnReceivedDisplayCommand for update the display
            socket.OnReceivedDisplayCommand += this.OnReceivedDisplayCommand;

            await TextToSpeech("Connection");
            await socket.Connect("192.168.137.1", "9999");
            await TextToSpeech("Connected");
        }


        //########################################################################################################
        public void OnReceivedDisplayCommand(string cmd, string[] args)
        {
            Application.InvokeOnMain(() =>
            {
                if (cmd == "new")
                {
                    // parse data
                    var x = float.Parse(args[0]);
                    var y = float.Parse(args[1]);
                    var z = float.Parse(args[2]);
                    var text = args[3];

                    // calc position and display the text in the scene
                    var pos = CullingCamera.Node.Position + CullingCamera.Node.Rotation * new Vector3(x, y, z); // the vector is a relative position to user
                    var annotation = Annotation.Text(this, text, pos);
                    Text3DList.Add(annotation);
                }
                else if (cmd == "del")
                {
                    Text3DList.Remove();
                }
            });
        }


        //########################################################################################################
        protected override void OnUpdate(float timeStep)
        {
            Text3DList.Update(CullingCamera.Node.Rotation); // update text orientation in order to facing the user
        }


    }
}