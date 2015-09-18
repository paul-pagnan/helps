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

namespace helps.Droid.Helpers
{
    public static class DialogHelper
    {
        public static void ShowDialog(Context context, string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context, Resource.Style.LightDialog);
            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }

        public static ProgressDialog CreateProgressDialog(string message, Activity activity)
        {
            ProgressDialog mProgressDialog = new ProgressDialog(activity, Resource.Style.LightDialog);
            mProgressDialog.SetMessage(message);
            mProgressDialog.SetCancelable(false);
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            return mProgressDialog;
        }
    }
}