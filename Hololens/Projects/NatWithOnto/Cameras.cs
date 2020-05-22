/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;


using ImageConversion;


namespace Cameras
{
    //#################################################################################################
    //#                                  FrameReaderCamera                                            #
    //#                                                                                               #
    //#  public Camera()                                                                              #
    //#  public bool IsRunning()                                                                      #
    //#  public bool IsCallbackActivated()                                                            #
    //#  public async Task Init()                                                                     #
    //#  public async Task Init(MediaStreamType streamType, MediaFrameSourceKind sourceKind)          #
    //#  public async Task Start()                                                                    #
    //#  public async Task Stop()                                                                     #
    //#  public void EnableCallback()                                                                 #
    //#  public void DisableCallback()                                                                #
    //#  public void UpdateDisplay()                                                                  #
    //#  public void DisplayXAML(Image image)                                                         #
    //#  public void HideXAML()                                                                       #
    //#  public SoftwareBitmap GetFrame()                                                             #
    //#  public byte[] GetFrameJpg()                                                                  #
    //#                                                                                               #
    //#  private async Task FindSource(MediaStreamType streamType, MediaFrameSourceKind sourceKind)   #
    //#  private MediaCaptureInitializationSettings SettingsForMediaCapture()                         #
    //#  private async Task InitMediaCapture()                                                        #
    //#  private async Task SetFrameFormat()                                                          #
    //#  private bool ReadFrame()                                                                     #
    //#  private void FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)          #
    //#                                                                                               #
    //#################################################################################################
    /// <summary>
    /// A camera used to get frames as in this tutorial : https://docs.microsoft.com/en-gb/windows/uwp/audio-video-camera/process-media-frames-with-mediaframereader
    /// </summary>
    public partial class FrameReaderCamera
    {
        MediaFrameSourceGroup _selectedGroup;
        MediaFrameSourceInfo _selectedSourceInfo;
        MediaFrameSource _frameSource;

        MediaCapture _mediaCapture;
        MediaFrameReader _mediaFrameReader;

        SoftwareBitmap _frame;
        Image _imageElement = null;

        bool _running = false;
        bool _callback_activated = false;
        Task _displaying = Task.CompletedTask;

        public delegate void Callback();
        public Callback AfterFrameArrived;

        //#############################################################################################
        //###################################      public      ########################################

        //#############################################################################################
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FrameReaderCamera()
        {

        }


        //#############################################################################################
        /// <summary>
        /// Accessor method that indicate if the video capture state
        /// </summary>
        /// <returns> if true, frames are captured and could be returned using <see cref="GetFrame"/> </returns>
        public bool IsRunning() { return _running; }

        //#############################################################################################
        /// <summary>
        ///  Accessor method that indicate if the optional callback is enabled.
        /// </summary>
        /// <returns> if true, callback is launch when a new frame is available </returns>
        public bool IsCallbackActivated() { return _callback_activated; }


        //#############################################################################################
        /// <summary>
        /// Default initialization for using the main camera. See <see cref="Init(MediaStreamType streamType, MediaFrameSourceKind sourceKind)"/>.
        /// </summary>
        public async Task Init()
        {
            await Init(MediaStreamType.VideoRecord, MediaFrameSourceKind.Color);
        }


        //#############################################################################################
        /// <summary>
        /// Initialize the camera. Check available cameras before build the FrameReader object and define the output format.
        /// Main camera could be initialized using MediaStreamType.VideoRecord and MediaFrameSourceKind.Color as arguments.
        /// </summary>
        /// <param name="streamType">
        ///     Type of media that the media reader will be used for.
        ///     Values : VideoRecord (recommanded for accessing frame), VideoPreview, Audio, Photo.
        /// </param>
        /// <param name="sourceKind">
        ///     For cameras, specify the type of image produced.
        ///     Values : Color (recommanded), Infrared, Depth.
        /// </param>
        public async Task Init(MediaStreamType streamType, MediaFrameSourceKind sourceKind)
        {
            await FindSource(streamType, sourceKind);
            await InitMediaCapture();
            await SetFrameFormat();
        }


        //#############################################################################################
        /// <summary>
        /// Start video capture.
        /// By default, if the callback is enabled, _imageElement is updated.
        /// </summary>
        public async Task Start()
        {
            if (!_running)
            {
                // create and start the reader
                _mediaFrameReader = await _mediaCapture.CreateFrameReaderAsync(_frameSource, MediaEncodingSubtypes.Argb32);
                await _mediaFrameReader.StartAsync();

                _running = true;
            }
        }


        //#############################################################################################
        /// <summary>
        /// Stop the video capture. After this call, frame could be get with <see cref="GetFrame"/> but keep in mind that it returns the last available frame if exist.
        /// </summary>
        public async Task Stop()
        {
            if (_running)
            {
                await _mediaFrameReader.StopAsync();

                _running = false;
            }
        }


        //#############################################################################################
        /// <summary>
        /// Allow <see cref="FrameArrived"/> method to be launched after MediaFrameReader.FrameArrived event.
        /// When this callback is enabled, <see cref="GetFrame()"/> doesn't work as the dispatcher stack it last in task order.
        /// </summary>
        public void EnableCallback()
        {
            _mediaFrameReader.FrameArrived += FrameArrived;
            _callback_activated = true;
        }


        //#############################################################################################
        /// <summary>
        /// Stop the call to <see cref="FrameArrived"/> method when MediaFrameReader.FrameArrived event happen.
        /// </summary>
        public void DisableCallback()
        {
            _mediaFrameReader.FrameArrived -= FrameArrived;
            _callback_activated = false;
        }


        //#############################################################################################
        /// <summary>
        /// When _imageElement is set, update it by displaying the last frame
        /// </summary>
        public void UpdateDisplay()
        {
            if (_imageElement != null && _displaying.IsCompleted) // check if _imageElement is set and if last display is finished
            {
                // to avoid multiple i/o conflict, update is made in a separate task schedule by the _imageElement itself
                _displaying = _imageElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        // for being displayable, frame formats must be bgra8 and premultiplied formats
                        _frame = ImageConverter.ToDisplayableXaml(_frame);

                        // get frame source data and set it to _imageElement
                        var bitmapSource = new SoftwareBitmapSource();
                        await bitmapSource.SetBitmapAsync(_frame);
                        _imageElement.Source = bitmapSource;
                    }
                    catch { }
                }).AsTask();
            }
        }


        //#############################################################################################
        /// <summary>
        /// Set _imageElement attribute with Image argument. Image is updated in <see cref="UpdateDisplay"/> that is called by default with <see cref="FrameArrived"/>.
        /// </summary>
        /// <param name="image"> XAML Image object where frames will be displayed by default </param>
        public void DisplayXAML(Image image)
        {
            if (_imageElement == null)
            {
                _imageElement = image;
            }
        }


        //#############################################################################################
        /// <summary>
        /// Unset _imageElement attribute. Image is not automatically updated even the callback is ebabled.
        /// </summary>
        public void HideXAML()
        {
            if (_imageElement != null)
            {
                _imageElement = null;
            }
        }


        //#############################################################################################
        /// <summary>
        /// Return the last available frame.
        /// </summary>
        /// <returns> The frame is a SoftwareBitmap object that could be manipulate for tranformations. </returns>
        public SoftwareBitmap GetFrame()
        {
            // check if a frame could be returned
            if (ReadFrame())
            {
                return SoftwareBitmap.Copy(_frame); // return a copy of the last frame to avoid multiple i/o
            }
            else
            {
                return null;
            }
        }


        //#############################################################################################
        /// <summary>
        /// Same as <see cref="GetFrame"/> but frame is in jpeg format
        /// </summary>
        /// <returns> a byte array representing the frame in jpeg format </returns>
        public byte[] GetFrameJpg()
        {
            // check if a frame could be returned
            if (ReadFrame())
            {
                return ImageConverter.ToJPG(_frame).Result; // return a copy converted
            }
            else
            {
                return null;
            }
        }


        //#############################################################################################
        //###################################      private     ########################################


        //#############################################################################################
        /// <summary>
        /// Find a source coresponding to parameters of <see cref="Init(MediaStreamType, MediaFrameSourceKind)"/>
        /// </summary>
        /// <param name="streamType"> MediaStreamType object property </param>
        /// <param name="sourceKind"> MediaFrameSourceKind object property </param>
        private async Task FindSource(MediaStreamType streamType, MediaFrameSourceKind sourceKind)
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync(); // list available sources

            // indicate that source is not set
            _selectedGroup = null;
            _selectedSourceInfo = null;

            foreach (var sourceGroup in frameSourceGroups)
            {
                foreach (var sourceInfo in sourceGroup.SourceInfos)
                {
                    // if a source is matching with arguments
                    if (sourceInfo.MediaStreamType == streamType && sourceInfo.SourceKind == sourceKind)
                    {
                        _selectedSourceInfo = sourceInfo;
                        break;
                    }
                }
                if (_selectedSourceInfo != null)
                {
                    _selectedGroup = sourceGroup;
                    break;
                }
            }

            // in case no source was found
            if (_selectedSourceInfo == null) System.Diagnostics.Debug.WriteLine("Source not find");
        }


        //#############################################################################################
        /// <summary>
        /// Return settings concerning MediaCapture object.
        /// </summary>
        /// <returns> MediaCaptureInitializationSettings object with mode ExclusiveControl for sharing mode, CPU for memory preference and Video for capture mode </returns>
        private MediaCaptureInitializationSettings SettingsForMediaCapture()
        {
            return new MediaCaptureInitializationSettings()
            {
                SourceGroup = _selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
        }


        //#############################################################################################
        /// <summary>
        /// Create and initialize a MediaCapture object
        /// </summary>
        private async Task InitMediaCapture()
        {
            _mediaCapture = new MediaCapture(); // creation

            try
            {
                // intialisation
                MediaCaptureInitializationSettings settings = SettingsForMediaCapture();
                await _mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
            }
        }


        //#############################################################################################
        /// <summary>
        /// Set the frame format for capture. Needed for allowing camera launch.
        /// </summary>
        private async Task SetFrameFormat()
        {
            _frameSource = _mediaCapture.FrameSources[_selectedSourceInfo.Id]; // get the source found in FindSource method
            var format = _frameSource.SupportedFormats.FirstOrDefault();
            await _frameSource.SetFormatAsync(format);
        }


        //#############################################################################################
        /// <summary>
        /// Capture a frame from the videostream. Doesn't work if callback is enabled.
        /// </summary>
        /// <returns> true indicate that _frame attribute is updated </returns>
        private bool ReadFrame()
        {
            using (var mediaFrameReference = _mediaFrameReader.TryAcquireLatestFrame()) // look for the last frame captured
            {
                var videoMediaFrame = mediaFrameReference?.VideoMediaFrame; // object that represent the frame
                var softwareBitmap = videoMediaFrame?.SoftwareBitmap; // frame as SoftwareBitmap

                if (softwareBitmap != null)
                {
                    _frame = softwareBitmap; // frame is saved inside _frame attribute
                    mediaFrameReference.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        //#############################################################################################
        /// <summary>
        /// The callback method called when a new frame is available. By default, try to capture the frame and display it in _imageElement if set.
        /// Note that parameters are not used.
        /// </summary>
        /// <param name="sender"> the object itself </param>
        /// <param name="args"> class MediaFrameArrivedEventArgs </param>
        private void FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            // run only if a display is set
            if (_imageElement != null)
            {
                // if the frame is available
                if (ReadFrame())
                {
                    AfterFrameArrived?.Invoke();
                }
            }
        }


    }



    //####################################################################################
    //#                                  PreviewCamera                                   #
    //#                                                                                  #
    //# public Camera(int width, int height)                                             #
    //# public bool IsCaptureEnabled()                                                   #
    //# public bool IsCaptureDisplayed()                                                 #
    //# public bool IsFrameDisplayed()                                                   #
    //# public async Task StartCapture()                                                 #
    //# public async Task StopCapture()                                                  #
    //# public async Task DisplayCapture(CaptureElement capture_element)                 #
    //# public async Task HideCapture()                                                  #
    //# public async Task CaptureFrame()                                                 #
    //# public async Task DisplayFrame(Image image)                                      #
    //# public void HideFrame()                                                          #
    //# public SoftwareBitmap GetFrame()                                                 #
    //# public async Task<byte[]> GetFrameJpg()                                          #
    //#                                                                                  #
    //####################################################################################
    /// <summary>
    /// Simple camera to use in XAML code. Allow to display both video or a single frame. Output resolution could be set.
    /// </summary>
    public partial class PreviewCamera
    {

        private MediaCapture _mediaCapture;
        private DisplayRequest _displayRequest;

        private VideoFrame _videoFrame;
        private SoftwareBitmapSource _source;
        private WriteableBitmap _bitmapBuffer;

        private SoftwareBitmap _frame = null;
        private CaptureElement _captureElement = null;
        private Image _image = null;

        private bool _capture_enabled = false;
        private bool _capture_displayed = false;
        private bool _frame_displayed = false;


        //################################################################################
        /// <summary>
        /// Constructor that set the frame resolution.
        /// </summary>
        /// <param name="width"> width of frame </param>
        /// <param name="height"> height of frame </param>
        public PreviewCamera(int width, int height)
        {
            _videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, width, height); // frame resolution
            _source = new SoftwareBitmapSource(); // a new source is needed for starting camera
            _bitmapBuffer = new WriteableBitmap(width, height); // buffer for image conversion
        }


        //################################################################################
        /// <summary>
        /// Indicate if video capture is running 
        /// </summary>
        /// <returns> true : the video capture is running </returns>
        public bool IsCaptureEnabled() { return _capture_enabled; }


        //################################################################################
        /// <summary>
        /// Indicate if video capture is display in a XAML captureElement
        /// </summary>
        /// <returns> true : the video capture is displayed </returns>
        public bool IsCaptureDisplayed() { return _capture_displayed; }


        //################################################################################
        /// <summary>
        /// Indicate if a frame is displayed
        /// </summary>
        /// <returns> true : the frame is displayed </returns>
        public bool IsFrameDisplayed() { return _frame_displayed; }


        //################################################################################
        /// <summary>
        /// Start the camera capture. After, frame could be captured.
        /// </summary>
        public async Task StartCapture()
        {
            // if the camera doesn't already capture video
            if (!_capture_enabled)
            {
                // needed for the system
                _displayRequest = new DisplayRequest();
                _displayRequest.RequestActive();

                // create and initialyze MediaCapture
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();

                // videostream must be send to a CaptureElement
                _captureElement = new CaptureElement { Source = _mediaCapture };
                await _mediaCapture.StartPreviewAsync(); // capture start here

                _capture_enabled = true;
            }
        }


        //################################################################################
        /// <summary>
        /// Stop the camera capture. Afetr, frame could not be captured.
        /// </summary>
        public async Task StopCapture()
        {
            if (_capture_enabled)
            {
                await _mediaCapture.StopPreviewAsync(); // stop the preview

                // unset capture element
                _captureElement.Source = null;
                _captureElement = null;

                _capture_enabled = false;
                _capture_displayed = false;
            }
        }


        //################################################################################
        /// <summary>
        /// Display the video in XAML capture element.
        /// </summary>
        /// <param name="capture_element"> XAML capture element defined in xaml file </param>
        public async Task DisplayCapture(CaptureElement capture_element)
        {
            if (_capture_enabled)
            {
                // capture must be stopped before modify _captureElement
                await _mediaCapture.StopPreviewAsync();
                _captureElement.Source = null;

                // set capture element
                _captureElement = capture_element;
                _captureElement.Source = _mediaCapture;

                await _mediaCapture.StartPreviewAsync(); // restart the capture

                _capture_displayed = true;
            }
        }


        //################################################################################
        /// <summary>
        /// Stop the video display.
        /// </summary>
        public async Task HideCapture()
        {
            if (_capture_displayed)
            {
                await _mediaCapture.StopPreviewAsync(); // capture must be stopped before modify _captureElement

                // set capture element
                _captureElement.Source = null;
                _captureElement = new CaptureElement { Source = _mediaCapture };

                await _mediaCapture.StartPreviewAsync(); // restart the capture

                _capture_displayed = false;
            }
        }


        //################################################################################
        /// <summary>
        /// Save a frame in its _frame attribute as a SoftwareBitmap object.
        /// </summary>
        public async Task CaptureFrame()
        {
            if (_capture_enabled)
            {
                await _mediaCapture.GetPreviewFrameAsync(_videoFrame);
                _frame = _videoFrame.SoftwareBitmap;
            }
        }


        //################################################################################
        /// <summary>
        /// Display the frame in XAML Image element.
        /// </summary>
        /// <param name="image"></param>
        public async Task DisplayFrame(Image image)
        {
            if (_frame != null)
            {
                await _source.SetBitmapAsync(_frame); // get a frame from video stream capture

                // set image as an attribute
                _image = image;
                _image.Source = _source;

                _frame_displayed = true;
            }
        }


        //################################################################################
        /// <summary>
        /// Hide frame displayed in XAML image element source
        /// </summary>
        public void HideFrame()
        {
            if (_frame_displayed)
            {
                _image.Source = null;
            }
        }


        //################################################################################
        /// <summary>
        /// Method for get a saved frame in the camera. Must be called after <see cref="CaptureFrame"/>
        /// </summary>
        /// <returns> SoftwareBitmap object that contain a copy the frame </returns>
        public SoftwareBitmap GetFrame()
        {
            if (_frame != null)
            {
                return SoftwareBitmap.Copy(_frame); // return a copy
            }
            else
            {
                return null;
            }
        }


        //################################################################################
        /// <summary>
        /// Same as <see cref="GetFrame"/> but frame format is jpeg
        /// </summary>
        /// <returns> a byte array representing the frame in jpeg format </returns>
        public async Task<byte[]> GetFrameJpg()
        {
            return await ImageConverter.ToJPG(GetFrame());
        }


    }
}
