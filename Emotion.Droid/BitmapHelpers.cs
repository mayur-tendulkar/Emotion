using Android.Graphics;
using Android.Media;
using Android.Widget;

namespace Emotion.Droid
{
    public static class BitmapHelpers
    {
        public static Bitmap GetAndRotateBitmap(string fileName)
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(fileName);
            using (Matrix mtx = new Matrix())
            {
                if (Android.OS.Build.Product.Contains("Emulator"))
                {
                    mtx.PreRotate(90);
                }
                else
                {
                    ExifInterface exif = new ExifInterface(fileName);
                    var orientation = (Android.Media.Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);
                    switch (orientation)
                    {
                        case Android.Media.Orientation.Rotate90:
                            mtx.PreRotate(90);
                            break;
                        case Android.Media.Orientation.Rotate180:
                            mtx.PreRotate(180);
                            break;
                        case Android.Media.Orientation.Rotate270:
                            mtx.PreRotate(270);
                            break;
                        case Android.Media.Orientation.Normal:
                            // Normal, do nothing
                            break;
                        default:
                            break;
                    }
                }
                if (mtx != null)
                    bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, mtx, false);
            }
            return bitmap;
        }
    }
}