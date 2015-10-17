using Android.App;
using Android.Content;

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
    }
}