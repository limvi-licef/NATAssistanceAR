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

        //
        // define objects used for display
        //


        //###################################################################################################################
        public MainApp(ApplicationOptions opts) : base(opts)
        {
            _ip = "IPV4 ADRESS";
            _cameraSocketPort = "xxxx";
            _rayCollisionSocketPort = "xxxx";
            _displaySocketPort = "xxxx";
        }


        //###################################################################################################################
        protected override void Init()
        {
            //
            // called when the application start
            //
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