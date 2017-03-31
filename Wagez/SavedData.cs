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
using SQLite.Net.Attributes;
using Android.Database.Sqlite;

namespace Wagez {

    class SavedData {

        [PrimaryKey, AutoIncrement]
        public string date { get; set; }

        public decimal hoursWorked { get; set; }

        public string month { get; set; }

        public SavedData(string date, decimal hoursWorked) {
            this.date = date;
            this.hoursWorked = hoursWorked;
        }

        public SavedData(string month) {
            this.month = month;
        }

        public SavedData() {

        }

        public override string ToString() {
            return date + ": " + hoursWorked + " hours.";
        }
        
    }

}