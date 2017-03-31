using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Provider;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using SQLite;
using Xamarin.Forms;

namespace Wagez {

    public static class App {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }

    public static class BitmapHelpers {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height) {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width) {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }

    [Activity(Label = "MonthActivity", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar.Fullscreen")]
    public class MonthActivity : Activity {

        ImageView imageView;
        TextView errorBox;
        EditText monthText;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MonthScreen);

            if (IsThereAnAppToTakePictures()) {
                CreateDirectoryForPictures();
                Android.Widget.Button button = FindViewById<Android.Widget.Button>(Resource.Id.addImage);
                Android.Widget.Button finishButton = FindViewById<Android.Widget.Button>(Resource.Id.addMonth);
                errorBox = FindViewById<TextView>(Resource.Id.errorBox);
                monthText = FindViewById<EditText>(Resource.Id.monthName);
                errorBox.SetTextColor(Android.Graphics.Color.Red);
                imageView = FindViewById<ImageView>(Resource.Id.imageView);
                button.Click += TakeAPicture;

                var notesEntry = new Entry();
                notesEntry.Completed += (sender, e) => {
                    monthText.ClearFocus();
                };

                finishButton.Click += (object sender, EventArgs e) => {
                    if (monthText.Text != "") {
                        string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "savedMonths.db3");
                        var db = new SQLiteConnection(dbPath);

                        db.CreateTable<SavedData>();
                        SavedData newData = new SavedData(monthText.Text);
                        db.Insert(newData);
                        var intent = new Intent(this, typeof(HomeActivity));
                        StartActivity(intent);
                    } else {
                        errorBox.Text = "Please add month name.";
                    }
                };

            }

        }

        private void CreateDirectoryForPictures() {
            App._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "myWage");
            if (!App._dir.Exists()) {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures() {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs) {
            if (monthText.Text != "") {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App._file = new Java.IO.File(App._dir, String.Format("{0}_rota.jpg", monthText.Text));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
                StartActivityForResult(intent, 0);
            } else {
                errorBox.Text = "Please add month name.";
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null) {
                imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

    }
}