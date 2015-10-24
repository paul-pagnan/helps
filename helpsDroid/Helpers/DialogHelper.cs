using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Util;
using Exception = Java.Lang.Exception;
using TimeZone = Java.Util.TimeZone;

namespace helps.Droid.Helpers
{
    public static class DialogHelper
    {
        public static void ShowDialog(Context context, string message, string title)
        {
            var builder = new AlertDialog.Builder(context);
            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }

        public static ProgressDialog CreateProgressDialog(string message, Activity activity)
        {
            var mProgressDialog = new ProgressDialog(activity, Resource.Style.LightDialog);
            mProgressDialog.SetMessage(message);
            mProgressDialog.SetCancelable(false);
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            return mProgressDialog;
        }


        public static AlertDialog.Builder CreateEditConfirmDialog(Activity activity)
        {
            var builder = new AlertDialog.Builder(activity);
            builder.SetMessage(
                "Are you sure you want to discard changes to this booking?");
            builder.SetCancelable(false);
            builder.SetPositiveButton("Keep Editing", delegate { });
            return builder;
        }

        public static DatePickerDialog ShowDatePickerDialog(Context ctx, int textView)
        {
            try
            {
                var cal = Calendar.GetInstance(TimeZone.Default);
                var listener = new OnDateSetListener(textView);
                var datePicker = new DatePickerDialog(ctx, listener, 
                    cal.Get(Calendar.Year),
                    cal.Get(Calendar.Month),
                    cal.Get(Calendar.DayOfMonth));
                datePicker.SetCancelable(false);
                datePicker.Show();
            }
            catch (Exception ex)
            {
                ShowDialog(ctx, "An error occured while showing Date Picker\n\n Error Details:\n" + ex, "Exception");
            }
            return null;
        }

        public static TimePickerDialog ShowTimePickerDialog(Context ctx, int textView)
        {
            try
            {
                var listener = new OnTimeSetListener(textView);
                var timePicker = new TimePickerDialog(ctx, listener, DateTime.Now.Hour, 0, false);
                timePicker.SetCancelable(false);
                timePicker.Show();
            }
            catch (Exception ex)
            {
                ShowDialog(ctx, "An error occured while showing Time Picker\n\n Error Details:\n" + ex, "Exception");
            }
            return null;
        }


        private class OnDateSetListener : Java.Lang.Object, DatePickerDialog.IOnDateSetListener
        {
            private int textView;

            public OnDateSetListener(int textView)
            {
                this.textView = textView;
            }
            
            // when dialog box is closed, below method will be called.
            public void OnDateSet(DatePicker view, int selectedYear,
                int selectedMonth, int selectedDay)
            {
                var date = new DateTime(selectedYear, selectedMonth + 1, selectedDay);
                var txt = ListActionHelper.filterView.FindViewById<TextView>(textView);
                var formattedDateTime = date.ToString("ddd, dd MMM yyyy");
                txt.Text = formattedDateTime;
            }
        }


        private class OnTimeSetListener : Java.Lang.Object, TimePickerDialog.IOnTimeSetListener
        {
            private int textView;

            public OnTimeSetListener(int textView)
            {
                this.textView = textView;
            }

            public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
            {
                var date = new DateTime(10,10,10,hourOfDay, minute, 0);
                var txt = ListActionHelper.filterView.FindViewById<TextView>(textView);
                var formattedTime = date.ToString("hh:mm tt");
                txt.Text = formattedTime;
            }
        }
    }
}