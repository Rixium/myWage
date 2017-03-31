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
using PCLStorage;
using System.Threading.Tasks;
using System.IO;
using SQLite;

namespace Wagez {
    [Activity(Label = "CalendarActivity", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar.Fullscreen")]
    public class CalendarActivity : Activity {

        string workedHours;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CalendarScreen);

            workedHours = Intent.GetStringExtra("hoursWorked") ?? "Data not available";

            TextView hoursWorked = FindViewById<TextView>(Resource.Id.hoursWorked);
            hoursWorked.Text = workedHours + " hours";

            Button addToCalendar = FindViewById<Button>(Resource.Id.addToCalendar);
            DatePicker datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            string date = "";

            addToCalendar.Click += (object sender, EventArgs e) => {
                date += datePicker.DayOfMonth.ToString() + "-";
                date += (datePicker.Month + 1).ToString() + "-";
                date += datePicker.Year.ToString();

                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "savedHours.db3");

                var db = new SQLiteConnection(dbPath);

                db.CreateTable<SavedData>();
                SavedData newData = new SavedData(date, Decimal.Parse(workedHours));
                db.Insert(newData);

                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
        }

    }
}