using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using System;

namespace Wagez {
    [Activity(Label = "myWage", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar.Fullscreen")]

    public class MainActivity : Activity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            EditText hourlyRate = FindViewById<EditText>(Resource.Id.hourlyRate);
            EditText hoursWorked = FindViewById<EditText>(Resource.Id.hoursWorked);
            EditText breakLength = FindViewById<EditText>(Resource.Id.breakLength);
            Switch paidBreaks = FindViewById<Switch>(Resource.Id.paidBreaks);
            Button calculateButton = FindViewById<Button>(Resource.Id.calculateButton);
            TextView calculatedWage = FindViewById<TextView>(Resource.Id.wageCalculated);
            calculatedWage.TextAlignment = Android.Views.TextAlignment.Center;
            hourlyRate.TextAlignment = Android.Views.TextAlignment.Center;
            hoursWorked.TextAlignment = Android.Views.TextAlignment.Center;

            Button addToCalendar = FindViewById<Button>(Resource.Id.addToCalendar);

            calculateButton.Click += (object sender, EventArgs e) => {
                if (hourlyRate.Text.ToString() != "") {
                    decimal hourRate = Decimal.Parse(hourlyRate.Text.ToString());
                    if (hoursWorked.Text.ToString() != "") {
                        decimal workedHours = Decimal.Parse(hoursWorked.Text.ToString());

                        if (breakLength.Text.ToString() == "") {
                            breakLength.Text = "0";
                        }

                        decimal lengthOfBreak = Decimal.Parse(breakLength.Text.ToString());

                        bool breaksPaid = paidBreaks.Checked;

                        decimal totalWage;

                        if (breaksPaid) {
                            totalWage = hourRate * workedHours;
                        } else {
                            totalWage = hourRate * (workedHours - lengthOfBreak);

                        }

                        totalWage = System.Math.Round(totalWage, 2);
                        calculatedWage.Text = "" + totalWage;
                    } else {
                        calculatedWage.Text = "Please input hours worked.";
                    }
                } else {
                    calculatedWage.Text = "Please input hourly rate.";
                }
            };

            addToCalendar.Click += (object sender, EventArgs e) => {
                decimal workedHours = Decimal.Parse(hoursWorked.Text.ToString());
                decimal lengthOfBreak = Decimal.Parse(breakLength.Text.ToString());

                if (workedHours > 0) {
                    var intent = new Intent(this, typeof(CalendarActivity));
                    if (paidBreaks.Checked) {
                        intent.PutExtra("hoursWorked", hoursWorked.Text.ToString());
                    } else {
                        decimal newValue = workedHours - lengthOfBreak;
                        intent.PutExtra("hoursWorked", "" + newValue);
                    }
                    StartActivity(intent);
                } else {
                    calculatedWage.Text = "Please input hours worked.";
                }
            };

        }

    }
}

