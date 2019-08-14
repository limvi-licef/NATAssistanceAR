/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


using ImageConversion;
using Cameras;


namespace FrameReaderCameraEx
{
    public sealed partial class MainPage : Page
    {
        FrameReaderCamera camera;

        //####################################################################################
        // app events
        public MainPage() => InitializeComponent();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            camera = new FrameReaderCamera();
            camera.AfterFrameArrived += camera.UpdateDisplay; // update will be updated for each new frame
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            camera.DisableCallback();
            camera.HideXAML();
            await camera.Stop();
        }


        //####################################################################################
        // button events
        public async void StartReadingEvent(Object sender, RoutedEventArgs args)
        {
            await camera.Init();
            await camera.Start();
        }

        public async void StopReadingEvent(Object sender, RoutedEventArgs args) => await camera.Stop();

        public void PlayVideoEvent(Object sender, RoutedEventArgs args)
        {
            camera.EnableCallback();
            camera.DisplayXAML(imageElement1);
        }

        public async void StopVideoEvent(Object sender, RoutedEventArgs args)
        {
            camera.DisableCallback();
            await Task.Delay(50); // avoid to hide during display
            camera.HideXAML();
        }

        public async void GetEvent(Object sender, RoutedEventArgs args)
        {
            // block to check if callback is enabled
            bool callback_enabled = camera.IsCallbackActivated();
            if (callback_enabled)
            {
                camera.DisableCallback(); // must be disabled otherwise frame could not be captured by GetFrame method
                await Task.Delay(5); // wait for next frame
            }

            var frame = camera.GetFrame(); // frame is returned here, following code is used for display
            {
                if (frame != null)
                {
                    // for displaying, frame must be in bgra8 and premultiplied formats
                    frame = ImageConverter.ToDisplayableXaml(frame);

                    // affectation to image element
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(frame);
                    imageElement2.Source = source;
                }
            }

            if (callback_enabled) camera.EnableCallback(); // restore callback if enabled at the beginning
        }

        public void HideEvent(Object sender, RoutedEventArgs args)
        {
            imageElement2.Source = null;
        }


    }
}
