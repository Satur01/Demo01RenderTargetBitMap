using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Demo01RenderTargetBitMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnTakePhoto_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI cameraUi = new CameraCaptureUI();

            cameraUi.PhotoSettings.AllowCropping = false;
            cameraUi.PhotoSettings.MaxResolution =
                CameraCaptureUIMaxPhotoResolution.MediumXga;

            Windows.Storage.StorageFile capturedMedia =
                await cameraUi.CaptureFileAsync(CameraCaptureUIMode.Photo);

            using (var streamCamera = await capturedMedia.OpenAsync(FileAccessMode.Read))
            {

                BitmapImage bitmapCamera = new BitmapImage();
                bitmapCamera.SetSource(streamCamera);
                imgSource.Source = bitmapCamera;
                txtDate.Text = DateTime.Now.ToString("D");
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {

            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });
            StorageFile file = await picker.PickSaveFileAsync();



            if (file != null)
            {

                RenderTargetBitmap renderTargetBitMap = new RenderTargetBitmap();

                //Por medio de este método podemos convertir nuestro elementos XAML en una imagen
                await renderTargetBitMap.RenderAsync(grdRender, (int)grdRender.Width, (int)grdRender.Height);
                var pixels = await renderTargetBitMap.GetPixelsAsync();

                using (IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder =
                        await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, randomAccessStream);
                    byte[] bytes = pixels.ToArray();
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitMap.PixelWidth, (uint)renderTargetBitMap.PixelHeight, 96, 96, bytes);

                    await encoder.FlushAsync();
                }

            }

        }


    }
}
