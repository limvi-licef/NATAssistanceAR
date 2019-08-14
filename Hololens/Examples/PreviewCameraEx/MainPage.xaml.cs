/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


using Cameras;


namespace PreviewCameraEx
{
    public sealed partial class MainPage : Page
    {
        PreviewCamera camera;

        //######################################################################################
        // app events
        public MainPage() => InitializeComponent();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            camera = new PreviewCamera(1280, 720);
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e) => await camera.StopCapture();


        //######################################################################################
        // button events
        public async void StartCaptureEvent(object sender, RoutedEventArgs args) => await camera.StartCapture();
        public async void StopCaptureEvent(object sender, RoutedEventArgs args) => await camera.StopCapture();

        public async void DisplayVideoEvent(object sender, RoutedEventArgs args) => await camera.DisplayCapture(VideoWidget);
        public async void HideVideoEvent(object sender, RoutedEventArgs args) => await camera.HideCapture();

        public async void CaptureFrameEvent(object sender, RoutedEventArgs args) => await camera.CaptureFrame();
        public async void DisplayFrameEvent(object sender, RoutedEventArgs args)
        {
            // method 1 : display by Camera object
            await camera.DisplayFrame(FrameWidget1);

            // method 2 : get a copy of frame and display manually
            SoftwareBitmap frame = camera.GetFrame();
            if (frame != null)
            {
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(frame);
                FrameWidget2.Source = source;
            }
        }
        public void HideFrameEvent(object sender, RoutedEventArgs args)
        {
            // method 1
            camera.HideFrame();

            // method 2
            FrameWidget2.Source = null;
        }

        public async void CaptureFrameJpegEvent(object sender, RoutedEventArgs args)
        {
            byte[] jpg = await camera.GetFrameJpg();
        }
    }
}
