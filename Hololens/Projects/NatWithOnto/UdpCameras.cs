/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Threading.Tasks;


using UdpSockets;
using Cameras;


namespace UdpCameras
{
    //#####################################################################################################
    //#                                UdpPreviewCamera : DataUdpSocket                                   #
    //#                                                                                                   #
    //#  public UdpPreviewCamera(int width, int height)                                                   #
    //#  public async Task Start()                                                                        #
    //#                                                                                                   #
    //#  protected async override void PrepareData()                                                      #
    //#                                                                                                   #
    //#####################################################################################################
    /// <summary>
    /// A DataUdpSocket class with integrated camera of type PreviewCamera.
    /// Provide an easy way to send video on a private network.
    /// </summary>
    public class UdpPreviewCamera : DataUdpSocket
    {
        PreviewCamera _camera;

        //#################################################################################################
        /// <summary>
        /// Constructor that create the Camera and the socket (inheritance). As for simple camera constructor, resolution could be set.
        /// </summary>
        /// <param name="width"> width frame resolution </param>
        /// <param name="height"> height frame resolution </param>
        public UdpPreviewCamera(int width, int height)
        {
            _camera = new PreviewCamera(width, height);
        }


        //#################################################################################################
        /// <summary>
        /// Start camera capture.
        /// </summary>
        public async Task Start()
        {
            await _camera.StartCapture();
        }


        //#################################################################################################
        /// <summary>
        /// Overrided method from DataUdpSocket that the goal is to set frame sending.
        /// </summary>
        protected async override void PrepareData()
        {
            // get the frame in jpg format
            await _camera.CaptureFrame();
            byte[] jpg = await _camera.GetFrameJpg();

            // reply as standard DataUdpSocket
            BindData(jpg, 65000);
            SendNbPacket();
        }

    }


    //#####################################################################################################
    //#                                   UdpFRCamera : DataUdpSocket                                     #
    //#                                                                                                   #
    //#  public UdpFRCamera()                                                                             #
    //#  public async Task Start()                                                                        #
    //#                                                                                                   #
    //#  protected override void PrepareData()                                                            #
    //#                                                                                                   #
    //#####################################################################################################
    /// <summary>
    /// A DataUdpSocket class with integrated camera of type FrameReaderCamera.
    /// Provide an easy way to send video on a private network.
    /// </summary>
    public class UdpFRCamera : DataUdpSocket
    {
        FrameReaderCamera _camera;


        //#################################################################################################
        /// <summary>
        /// Constructor that create the camera and the socket by inheritance.
        /// </summary>
        public UdpFRCamera()
        {
            _camera = new FrameReaderCamera();
        }


        //#################################################################################################
        /// <summary>
        /// Start camera capture.
        /// </summary>
        public async Task Start()
        {
            await _camera.Init();
            await _camera.Start();
        }


        //#################################################################################################
        /// <summary>
        /// Get a frame in jpeg from the camera and prepare to send as inherited class.
        /// </summary>
        protected override void PrepareData()
        {
            byte[] jpg = _camera.GetFrameJpg(); // get frame as jpg byte array

            // reply as standard DataUdpSocket
            BindData(jpg, 65000);
            SendNbPacket();
        }

    }

}
