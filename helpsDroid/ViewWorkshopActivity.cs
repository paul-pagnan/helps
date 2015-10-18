using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using com.refractored.fab;
using helps.Droid.Adapters;
using helps.Droid.Helpers;
using helps.Shared.DataObjects;
using Java.Interop;
using Environment = System.Environment;

namespace helps.Droid
{
    [Activity(Label = "", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class ViewWorkshopActivity : Main
    {
        private static WorkshopDetail workshop;
        private WorkshopBooking booking;

        private TextView title;
        private TextView room;
        private TextView date;
        private TextView targetGroup;
        private TextView whatItCovers;
        private TextView placeAvailable;
        private static TextView notifications;
        private EditText editTxtNotes;

        private LinearLayout sessionsList;
        private RelativeLayout toolbarLayout;
        private RelativeLayout sessionContainer;
        private RelativeLayout bookingsContainer;

        private ScrollView mainLayout;
        private ScrollView editLayout;
        private ViewFlipper flipper;


        private SessionListAdapter sessionsListAdapter;

        private Button bookButton;
        private Button cancelButton;
        private Button waitlistButton;
        private FloatingActionButton fab;

        private bool IsBooking;

        private Color color;
        private readonly int colorDiff = 35;

        private bool IsEditing;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewWorkshop; }
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);

            var extras = Intent.Extras;
            if (extras != null)
            {
                //Get vars from bundle
                var workshopId = extras.GetInt("WorkshopId");

                //Get the Booking Details
                if (IsBooking = extras.GetBoolean("IsBooking"))
                    workshop = await Services.Workshop.GetWorkshopFromBooking(workshopId);
                else
                    workshop = Services.Workshop.GetWorkshop(workshopId);

                ListBaseAdapter.InitColors(Resources);
                color = ListBaseAdapter.GetColor(workshop.WorkshopSetId);

                //Style the view to match workshop
                Toolbar.SetBackgroundColor(color);
                toolbarLayout = FindViewById<RelativeLayout>(Resource.Id.layouttoolbarLarge);
                toolbarLayout.SetBackgroundColor(color);
                Toolbar.NavigationIcon = Resources.GetDrawable(Resource.Drawable.ic_close_white_24dp);

                SetTaskDescription(new ActivityManager.TaskDescription(
                    Resources.GetString(Resource.String.app_name),
                    BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher),
                    color));

                //Update the view
                InitComponents();
                UpdateFields();

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

                //Load booking information so the buttons can be updated
                //Get Local Data First, then update later
                await Task.Factory.StartNew(() => LoadBooking(true));

                //Do a background Sync now
                await Task.Factory.StartNew(() => LoadBooking(false, false));
            }
        }

        private void UpdateFields()
        {
            if(booking != null)
                ShowNotifications();

            title.Text = workshop.Title;
            room.Text = workshop.Room;

            var TimeString = (workshop.Time != null) ? Environment.NewLine + workshop.Time : "";
            date.Text = workshop.DateHumanFriendly + TimeString;

            targetGroup.Text = workshop.TargetGroup;
            whatItCovers.Text = workshop.Description;
            placeAvailable.Text = workshop.FilledPlaces + "/" + workshop.TotalPlaces;

            sessionsListAdapter.AddAll(workshop.Sessions);

            for (var i = 0; i < sessionsListAdapter.Count; i++)
            {
                var view = sessionsListAdapter.GetView(i, null, sessionsList);
                sessionsList.AddView(view);
            }
            UpdateButtons();
            editTxtNotes.Text = workshop.Notes;
            SetButtonColor();
        }

        private void ShowNotifications()
        {
            FindViewById<RelativeLayout>(Resource.Id.notifications).Visibility = ViewStates.Visible;
            UpdateNotifications();
        }

        private void UpdateButtons()
        {
            bookButton.Visibility = ViewStates.Gone;

            if (workshop.FilledPlaces >= workshop.TotalPlaces)
            {
                waitlistButton.Visibility = ViewStates.Visible;
                return;
            }

            if (booking == null)
                bookButton.Visibility = ViewStates.Visible;
            else
                cancelButton.Visibility = ViewStates.Visible;
        }

        public static void UpdateNotifications()
        {
            var nots = Services.Notification.GetNotifications(workshop.Id);
            notifications.Text = (nots.Any(x => x.selected)) ? "" : "No notifications set ";
            foreach (var notification in nots.Where(x => x.selected))
            {
                notifications.Text += notification.title + Environment.NewLine;
            }
            notifications.Text = notifications.Text.Substring(0, notifications.Text.Length - 1);
        }

        private async void LoadBooking(bool localOnly, bool force = false)
        {
            booking = await Services.Workshop.GetBooking(workshop.Id, localOnly, force);
            RunOnUiThread(delegate { UpdateFields(); });
        }

        private void InitComponents()
        {
            title = FindViewById<TextView>(Resource.Id.title);
            room = FindViewById<TextView>(Resource.Id.textViewRoomValue);
            date = FindViewById<TextView>(Resource.Id.textViewDateValue);
            targetGroup = FindViewById<TextView>(Resource.Id.textViewTargetGroupValue);
            whatItCovers = FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue);
            placeAvailable = FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue);
            notifications = FindViewById<TextView>(Resource.Id.textViewnotification);

            sessionsList = FindViewById<LinearLayout>(Resource.Id.listViewSessions);
            sessionsList.Orientation = Orientation.Vertical;
            sessionsListAdapter = new SessionListAdapter(LayoutInflater);

            sessionContainer = FindViewById<RelativeLayout>(Resource.Id.sessionContainer);
            if (workshop.Sessions.Count == 0)
                sessionContainer.Visibility = ViewStates.Gone;

            bookingsContainer = FindViewById<RelativeLayout>(Resource.Id.bookingsContainer);
            if (workshop.FilledPlaces == -1)
                bookingsContainer.Visibility = ViewStates.Gone;

            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            if (IsBooking)
            {
                fab.Visibility = ViewStates.Visible;
            }

            bookButton = FindViewById<Button>(Resource.Id.BookBtn);
            cancelButton = FindViewById<Button>(Resource.Id.CancelBtn);
            waitlistButton = FindViewById<Button>(Resource.Id.WaitlistBtn);

            mainLayout = FindViewById<ScrollView>(Resource.Id.mainLayout);
            editLayout = FindViewById<ScrollView>(Resource.Id.editLayout);
            flipper = FindViewById<ViewFlipper>(Resource.Id.flipper);

            editTxtNotes = FindViewById<EditText>(Resource.Id.editTxtNotes);
        }

        [Export]
        public async void Book(View view)
        {
            var dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;
            if (workshop.ProgramId.HasValue)
                response = await Services.Workshop.BookProgram(workshop.ProgramId.Value);
            else
                response = await Services.Workshop.Book(workshop.Id);
            dialog.Hide();

            if (response.Success)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Booked Successfully");
                builder.SetMessage("Would you like to set up notifications now?");
                builder.SetCancelable(false);
                builder.SetPositiveButton("Create Notifications", delegate { ShowNotificationDialog(null); });
                builder.SetNegativeButton("Close", delegate { });
                builder.Show();
                booking = new WorkshopBooking();
                UpdateButtons();
                ShowNotifications();                
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }

        [Export]
        public void Cancel(View view)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Are you sure?");
            builder.SetMessage(
                "NOTE: Cancelling a booking which is part of a program (series of workshops) will cancel all bookings in that program");
            builder.SetCancelable(false);
            builder.SetPositiveButton("Yes", delegate { ActuallyCancel(); });
            builder.SetNegativeButton("No", delegate { });
            builder.Show();
        }

        private async void ActuallyCancel()
        {
            var dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;
            response = await Services.Workshop.CancelBooking(workshop.Id);
            dialog.Hide();

            if (response.Success)
            {
                DialogHelper.ShowDialog(this, "The workshop has been successfully cancelled", "Workshop Cancelled");
                NotificationHelper.Cancel(this, workshop.Id);
                booking = null;
                UpdateButtons();
                FindViewById<RelativeLayout>(Resource.Id.notifications).Visibility = ViewStates.Gone;
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }

        [Export]
        public void Edit(View view)
        {
            var animOut = AnimationUtils.LoadAnimation(this, Resource.Animation.fadeout);
            fab.StartAnimation(animOut);
            animOut.AnimationEnd += async (sender, e) =>
            {
                if (IsEditing)
                {
                    var dialog = DialogHelper.CreateProgressDialog("Saving...", this);
                    dialog.Show();
                    var response = await Services.Workshop.AddNotes(editTxtNotes.Text, workshop.Id);
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
                    IsEditing = !IsEditing;
                    AnimateButton();
                    FlipView();
                }
            };
        }

        public override void OnBackPressed()
        {
            if (Back())
                return;
            base.OnBackPressed();
        }

        public bool Back()
        {
            if (flipper.DisplayedChild > 0)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetMessage(
                    "Are you sure you want to discard changes to this booking?");
                builder.SetCancelable(false);
                builder.SetPositiveButton("Keep Editing", delegate { });
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

        private void SetButtonColor()
        {
            var opp = (IsEditing) ? (255/2) : 0;
            fab.ColorNormal = Color.Argb(color.A, color.R + colorDiff + opp, color.G + opp + colorDiff,
                color.B + opp + colorDiff);
            fab.ColorPressed = Color.Argb(color.A, color.R + opp + (colorDiff/3), color.G + opp + (colorDiff/3),
                color.B + opp + (colorDiff/3));
            fab.ColorRipple = Color.Argb(color.A, color.R + opp, color.G + opp, color.B + opp);
        }

        public bool FlipView(bool trash = false)
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
            return true;
        }

        protected override void OnSaveInstanceState(Bundle bundle)
        {
            var position = flipper.DisplayedChild;
            bundle.PutInt("TAB_NUMBER", position);
            bundle.PutString("NOTES", editTxtNotes.Text);
        }


        [Export]
        public void ShowNotificationDialog(View view)
        {
            var notifier = new NotificationHelper(workshop);
            notifier.ShowDialog(this, workshop);
        }
    }
}