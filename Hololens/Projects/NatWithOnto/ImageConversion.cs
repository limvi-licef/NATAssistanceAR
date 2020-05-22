/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;


namespace ImageConversion
{
    //############################################################################################
    //#                                 ImageConverter                                           #
    //#                                                                                          #
    //#  public async Task<byte[]> ToJPG(SoftwareBitmap bitmap)                                  #
    //#  public SoftwareBitmap ToDisplayableXaml(SoftwareBitmap bitmap)                          #
    //#                                                                                          #
    //############################################################################################
    /// <summary>
    /// Static class for converting SoftwareBitmap objects.
    /// </summary>
    public static class ImageConverter
    {
        //########################################################################################
        /// <summary>
        /// Convert to the bitmap object to a jpeg suence of bytes
        /// </summary>
        /// <param name="bitmap"> SoftwareBitmap object </param>
        /// <returns> a byte array jpeg image </returns>
        public static async Task<byte[]> ToJPG(SoftwareBitmap bitmap)
        {
            if (bitmap != null)
            {
                using (var stream_buffer = new InMemoryRandomAccessStream())
                {
                    // create encoder, bind to it software bitmap image and encode
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream_buffer); // stream buffer is used by the encoder
                    encoder.SetSoftwareBitmap(bitmap);
                    await encoder.FlushAsync(); // encode and store data in stream buffer

                    byte[] jpg = new byte[stream_buffer.Size]; // the expected byte array
                    await stream_buffer.ReadAsync(jpg.AsBuffer(), (uint)stream_buffer.Size, InputStreamOptions.None); // data transfer from buffer to byte array

                    return jpg;
                }
            }
            else
            {
                return null;
            }
        }


        //########################################################################################
        /// <summary>
        /// Convert the bitmap object to a displayable format to use in XAML.
        /// </summary>
        /// <param name="bitmap"> SoftwareBitmap object </param>
        /// <returns> a SoftwareBitmap object converted to Bgra8 and premultiplied format </returns>
        public static SoftwareBitmap ToDisplayableXaml(SoftwareBitmap bitmap)
        {
            if (bitmap != null)
            {
                if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || bitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
                {
                    return SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
                else
                {
                    return bitmap;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
