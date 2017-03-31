using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System.IO;
using Android.Graphics;

namespace Wagez {
    [Activity(Label = "myWage", MainLauncher = true, Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar.Fullscreen")]
    public class HomeActivity : Activity {

        string dbPath;
        List<Button> buttons = new List<Button>();
        LinearLayout l;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            try {
                dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "savedMonths.db3");
            } catch (Exception ex) {

            }

            SetContentView(Resource.Layout.HomeScreen);

            Button deleteData = FindViewById<Button>(Resource.Id.deleteButton);
            Button addMonth = FindViewById<Button>(Resource.Id.addMonth);
            l = FindViewById<LinearLayout>(Resource.Id.linearLayout);

            TextView savedData = FindViewById<TextView>(Resource.Id.savedData);

            try {
                var db = new SQLiteConnection(dbPath);
                var table = db.Table<SavedData>();

                foreach (var item in table) {
                    SavedData newData = new SavedData(item.month);
                    Button newButton = new Button(this);
                    buttons.Add(newButton);
                    newButton.Text = newData.month;
                    l.AddView(newButton);
                }
            } catch (Exception ex) {

            }

            deleteData.Click += (object sender, EventArgs e) => {
                File.Delete(dbPath);
                foreach (Button button in buttons) {
                    button.Visibility = ViewStates.Invisible;
                }
            };

            addMonth.Click += (object sender, EventArgs e) => {
                var intent = new Intent(this, typeof(MonthActivity));
                StartActivity(intent);
            };
        }

    }
}