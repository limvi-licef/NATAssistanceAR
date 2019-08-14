/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


using UdpSockets;


namespace SocketConnection
{
    public sealed partial class MainPage : Page
    {

        BaseUdpSocket socket;


        //#########################################################################
        public MainPage()
        {
            this.InitializeComponent();
        }


        //#########################################################################
        public async void ConnectEvent(Object sender, RoutedEventArgs args)
        {
            if (socket == null)
            {
                socket = new BaseUdpSocket();
                output.Items.Add(String.Format("Connection to {0}:{1}", hostBox.Text, portBox.Text));

                bool connected = await socket.Connect(hostBox.Text, portBox.Text);
                if (connected) output.Items.Add("Connected");
            }
        }


        //#########################################################################
        public void DisconnectEvent(Object sender, RoutedEventArgs args)
        {
            if (socket != null && socket.IsConnected())
            {
                bool disconnected = socket.Close();
                socket = null;
                if (disconnected) output.Items.Add("Deconnected");
            }
        }


    }
}
