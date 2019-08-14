/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


using UdpSockets;


namespace UdpDataSegmentation
{
    public sealed partial class MainPage : Page
    {

        DataUdpSocket socket;


        //###################################################################################
        public MainPage()
        {
            this.InitializeComponent();
        }


        //###################################################################################
        public async void ConnectEvent(Object sender, RoutedEventArgs args)
        {
            if (socket == null)
            {
                socket = new DataUdpSocket();
                output.Items.Add(String.Format("Connection to {0}:{1}", hostBox.Text, portBox.Text));

                bool connected = await socket.Connect(hostBox.Text, portBox.Text);
                if (connected)
                {
                    output.Items.Add("Connected");

                    await socket.AwaitDisconnection();
                    socket = null;

                    output.Items.Add("Disconnected");
                }
            }
        }


        //###################################################################################
        public void DisconnectEvent(Object sender, RoutedEventArgs args)
        {
            if (socket != null && socket.IsConnected())
            {
                bool disconnected = socket.Close();
            }
        }


    }
}
