using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using System;
using Emotion.Shared;

namespace Emotion.Droid
{
    [Activity(Label = "Emotion.Droid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap _bitmap;
        private ImageView _imageView;
        private Button _pictureButton;
        private TextView _resultTextView;
        private bool _isCaptureMode = true;

        private void CreateDirectoryForPictures()
        {
            _dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void OnActionClick(object sender, EventArgs eventArgs)
        {
            if (_isCaptureMode == true)
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                _file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
                StartActivityForResult(intent, 0);
            }
            else
            {
                _imageView.SetImageBitmap(null);
                if (_bitmap != null)
                {
                    _bitmap.Recycle();
                    _bitmap.Dispose();
                    _bitmap = null;
                }
                _pictureButton.Text = "Take Picture";
                _resultTextView.Text = "";
                _isCaptureMode = true;
            }
        }
        protected override void OnCreate(Bundle bundle)
        {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                _pictureButton = FindViewById<Button>(Resource.Id.GetPictureButton);
                _pictureButton.Click += OnActionClick;
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                _resultTextView = FindViewById<TextView>(Resource.Id.resultText);
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                _bitmap = BitmapHelpers.GetAndRotateBitmap(_file.Path);
                _bitmap = Bitmap.CreateScaledBitmap(_bitmap, 2000, (int)(2000 * _bitmap.Height / _bitmap.Width), false);
                _imageView.SetImageBitmap(_bitmap);
                _resultTextView.Text = "Loading...";
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    _bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    float result = await Core.GetAverageHappinessScore(stream);
                    _resultTextView.Text = Core.GetHappinessMessage(result);
                }
            }
            catch (Exception ex)
            {
                _resultTextView.Text = ex.Message;
            }
            finally
            {
                _pictureButton.Text = "Reset";
                _isCaptureMode = false;
            }
        }
    }
}

