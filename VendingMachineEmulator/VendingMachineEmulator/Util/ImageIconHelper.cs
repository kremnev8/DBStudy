using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VendingMachineEmulator.Util
{
    public static class ImageIconHelper
    {
        public static void SetImage(this Image image, string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
        }
        
        public static BitmapImage GetImage(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.EndInit();
            
            return bitmap;
        }
    }
}