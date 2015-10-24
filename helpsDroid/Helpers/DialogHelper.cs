using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using helps.Shared.DataObjects;

namespace helps.Droid.Helpers
{
    public static class DialogHelper
    {
        public static void ShowDialog(Context context, string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
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


        public static AlertDialog.Builder CreateEditConfirmDialog(Activity activity)
        {
            var builder = new AlertDialog.Builder(activity);
            builder.SetMessage(
                "Are you sure you want to discard changes to this booking?");
            builder.SetCancelable(false);
            builder.SetPositiveButton("Keep Editing", delegate { });
            return builder;
        }
    }
}