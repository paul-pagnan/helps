using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Database.Sqlite;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views.Animations;
using Android.Widget;
using helps.Droid.Adapters;
using helps.Droid.Helpers;
using helps.Shared.DataObjects;
using Java.Interop;
using Android.Views;
using com.refractored.fab;

namespace helps.Droid
{
    public class ViewSessionBase : Main
    {
        private bool IsEditing;
        protected bool HideEdit;
        private ViewFlipper flipper;
        private RelativeLayout toolbarLayout;
        protected EditText editTxtNotes;
        private FloatingActionButton fab;
        protected TextView title;
        private static TextView notifications;
        private Color color;
        private const int colorDiff = 35;
        private bool locked;

        protected override int LayoutResource { get; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);

            var extras = Intent.Extras;
            if (extras != null)
            {              
                //Update the view
                InitComponents();
                // Maintain view once the view has been rotated
                if (bundle != null)
                {
                    var flipperPosition = bundle.GetInt("TAB_NUMBER");
                    if (flipperPosition > 0)
                    {
                        IsEditing = true;
                        AnimateButton();
                    }
                    editTxtNotes.Text = bundle.GetString("NOTES");
                    flipper.DisplayedChild = flipperPosition;
                }
            }
        }

        protected void ShowNotifications(int id)
        {
            FindViewById<RelativeLayout>(Resource.Id.notifications).Visibility = ViewStates.Visible;
            UpdateNotifications(id);
        }

        protected void InitComponents()
        {
            title = FindViewById<TextView>(Resource.Id.title);
            notifications = FindViewById<TextView>(Resource.Id.textViewnotification);
            flipper = FindViewById<ViewFlipper>(Resource.Id.flipper);
            editTxtNotes = FindViewById<EditText>(Resource.Id.editTxtNotes);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            if (!HideEdit)
            {
                fab.Visibility = ViewStates.Visible;
            }
        }

        protected void EditNotes(WorkshopDetail session)
        {
            var animOut = AnimationUtils.LoadAnimation(this, Resource.Animation.fadeout);
            fab.StartAnimation(animOut);
            animOut.AnimationEnd += async (sender, e) =>
            {
                if (IsEditing)
                {
                    var dialog = DialogHelper.CreateProgressDialog("Saving...", this);
                    dialog.Show();
                    var response = await Services.Workshop.AddNotes(editTxtNotes.Text, session.Id);
                    dialog.Hide();
                    if (response.Success)
                    {
                        IsEditing = !IsEditing;
                        AnimateButton();
                        FlipView();
                    }
                    else
                        DialogHelper.ShowDialog(this, response.Message, response.Title);
                }
                else
                {
                    locked = DateTime.UtcNow.AddDays(-7) > session.DateEnd;
                    editTxtNotes.Enabled = !locked;
                    if (locked && editTxtNotes.Text == "")
                        editTxtNotes.Text = "No notes";
                    IsEditing = !IsEditing;
                    AnimateButton();
                    FlipView();
                }
            };
        }

        protected void ShowNotificationDialog(int id, DateTime startDate)
        {
            var notifier = new NotificationHelper(id, startDate);
            notifier.ShowDialog(this, id, startDate);
        }

        private void AnimateButton()
        {
            if (IsEditing)
                fab.SetImageDrawable(GetDrawable(Resource.Drawable.ic_check_24px));
            else
                fab.SetImageDrawable(GetDrawable(Resource.Drawable.ic_mode_edit_24px));

            SetButtonColor();
            var animIn = AnimationUtils.LoadAnimation(this, Resource.Animation.fadein);
            fab.StartAnimation(animIn);
        }


        private void FlipView(bool trash = false)
        {
            var index = flipper.DisplayedChild;
            var InAnimation = (index > 0)
                ? Resource.Animation.slide_in_from_left
                : Resource.Animation.appear_from_top_right;
            var OutAnimation = (index > 0)
                ? Resource.Animation.dissapear_to_top_right
                : Resource.Animation.slide_out_to_left;

            if (trash)
                OutAnimation = Resource.Animation.dissapear_to_bottom_right;

            flipper.SetInAnimation(this, InAnimation);
            flipper.SetOutAnimation(this, OutAnimation);
            if (index > 0)
                flipper.ShowPrevious();
            else
                flipper.ShowNext();
        }

        private bool Back()
        {
            if (flipper.DisplayedChild > 0 && !locked)
            {
                var builder = DialogHelper.CreateEditConfirmDialog(this);
                builder.SetNegativeButton("Discard", delegate
                {
                    IsEditing = !IsEditing;
                    AnimateButton();
                    FlipView(true);
                });
                builder.Show();
                return true;
            }
            return false;
        }

        public override void OnBackPressed()
        {
            if (Back())
                return;
            base.OnBackPressed();
        }

        private void SetButtonColor()
        {
            var opp = (IsEditing) ? (255 / 2) : 0;
            fab.ColorNormal = Color.Argb(color.A, color.R + colorDiff + opp, color.G + opp + colorDiff,
                color.B + opp + colorDiff);
            fab.ColorPressed = Color.Argb(color.A, color.R + opp + (colorDiff / 3), color.G + opp + (colorDiff / 3),
                color.B + opp + (colorDiff / 3));
            fab.ColorRipple = Color.Argb(color.A, color.R + opp, color.G + opp, color.B + opp);
        }

        protected override void OnSaveInstanceState(Bundle bundle)
        {
            var position = flipper.DisplayedChild;
            bundle.PutInt("TAB_NUMBER", position);
            bundle.PutString("NOTES", editTxtNotes.Text);
        }

        protected void SetToolbarColor(Color toolbarColor)
        {
            color = toolbarColor;
            //Style the view to match workshop
            Toolbar.SetBackgroundColor(color);
            toolbarLayout = FindViewById<RelativeLayout>(Resource.Id.layouttoolbarLarge);
            toolbarLayout.SetBackgroundColor(color);
            Toolbar.NavigationIcon = Resources.GetDrawable(Resource.Drawable.ic_close_white_24dp);

            SetTaskDescription(new ActivityManager.TaskDescription(
                Resources.GetString(Resource.String.app_name),
                BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher),
                color));
            SetButtonColor();
        }

        public static void UpdateNotifications(int id)
        {
            var nots = Services.Notification.GetNotifications(id);
            notifications.Text = (nots.Any(x => x.selected)) ? "" : "No notifications set ";
            foreach (var notification in nots.Where(x => x.selected))
                notifications.Text += notification.title + System.Environment.NewLine;
            notifications.Text = notifications.Text.Substring(0, notifications.Text.Length - 1);
        }
    }
}