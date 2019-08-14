/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

// comment the following line for use preview camera instead of frame reader camera
#define FRAME_READER


using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


using UdpCameras;



namespace UdpVideoStream
{
    public sealed partial class MainPage : Page
    {
        #if (FRAME_READER)
        UdpFRCamera camera;
        #else
        UdpPreviewCamera camera;
        #endif

        //####################################################################################
        public MainPage()
        {
            this.InitializeComponent();
        }

        //####################################################################################
        public async void ConnectEvent(Object sender, RoutedEventArgs args)
        {
            if (camera == null)
            {
                // create and set the camera capture
                #if (FRAME_READER)
                camera = new UdpFRCamera();
                #else
                camera = new UdpPreviewCamera(1280, 720);
                #endif

                await camera.Start();
                output.Items.Add("Camera started");

                // try to connect
                output.Items.Add(String.Format("Connection to {0}:{1}", hostBox.Text, portBox.Text));
                bool connected = await camera.Connect(hostBox.Text, portBox.Text);
                if (connected)
                {
                    output.Items.Add("Connected");
                    
                    await camera.AwaitDisconnection();
                    camera = null;

                    output.Items.Add("Disconnected");
                }
            }
        }

        //####################################################################################
        public void DisconnectEvent(Object sender, RoutedEventArgs args)
        {
            if (camera != null && camera.IsConnected())
            {
                bool disconnected = camera.Close();
            }
        }


    }
}
