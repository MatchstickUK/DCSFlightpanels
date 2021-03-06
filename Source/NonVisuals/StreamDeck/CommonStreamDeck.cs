﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using ClassLibraryCommon;

namespace NonVisuals.StreamDeck
{
    public static class CommonStreamDeck
    {
        public const string DCSBIOS_PLACE_HOLDER = "{dcsbios}";

        public const string HOME_LAYER_NAME = "Home";







        public static BitmapImage ConvertBitMap(Bitmap bitmap)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
            catch (Exception e)
            {
                Common.LogError(e, "Failed to convert bitmap to bitmapimage.");
            }
            return null;
        }
    }
}
