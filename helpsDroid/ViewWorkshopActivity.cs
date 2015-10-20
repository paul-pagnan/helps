using System;
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
    public class ViewWorkshopActivity : ViewSessionBase
    {
        private LinearLayout sessionsList;        
        private RelativeLayout sessionContainer;
        private RelativeLayout bookingsContainer;
        private SessionListAdapter sessionsListAdapter;
        private Button bookButton;
        private Button cancelButton;
        private Button waitlistButton;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewWorkshop; }
        }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var extras = Intent.Extras;
            if (extras != null)
            {
                //Get vars from bundle
                var workshopId = extras.GetInt("WorkshopId");

                //Get the Booking Details
                if (extras.GetBoolean("IsBooking"))
                    workshop = await Services.Workshop.GetWorkshopFromBooking(workshopId);
                else
                {
                    workshop = Services.Workshop.GetWorkshop(workshopId);
                    HideEdit = true;
                }

                ListBaseAdapter.InitColors(Resources);
                SetToolbarColor(ListBaseAdapter.GetColor(workshop.WorkshopSetId));
                InitWorkshopComponents();
                UpdateFields();

                //Load booking information so the buttons can be updated
                //Get Local Data First, then update later
                await Task.Factory.StartNew(() => LoadBooking(true));
                //Do a background Sync now
                await Task.Factory.StartNew(() => LoadBooking(false, false));
            }
        }

        private void ShowNotifications()
        {
            FindViewById<RelativeLayout>(Resource.Id.notifications).Visibility = ViewStates.Visible;
            UpdateNotifications();
        }

        private void UpdateButtons()
        {
            bookButton.Visibility = ViewStates.Gone;
            cancelButton.Visibility = ViewStates.Gone;
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

     
        private async void LoadBooking(bool localOnly, bool force = false)
        {
            booking = await Services.Workshop.GetBooking(workshop.Id, localOnly, force);
            RunOnUiThread(delegate { UpdateFields(); });
        }

        private void UpdateFields()
        {
            if (booking != null)
                ShowNotifications();
            title.Text = workshop.Title;
            editTxtNotes.Text = workshop.Notes;

            FindViewById<TextView>(Resource.Id.textViewRoomValue).Text = workshop.Room;
            var TimeString = (workshop.Time != null) ? Environment.NewLine + workshop.Time : "";
            FindViewById<TextView>(Resource.Id.textViewDateValue).Text = workshop.DateHumanFriendly + TimeString;
            FindViewById<TextView>(Resource.Id.textViewTargetGroupValue).Text = workshop.TargetGroup;
            FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue).Text = workshop.Description;
            FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue).Text = workshop.FilledPlaces + "/" + workshop.TotalPlaces;
            sessionsListAdapter.Clear();
            sessionsListAdapter.AddAll(workshop.Sessions);
            for (var i = 0; i < sessionsListAdapter.Count; i++)
            {
                var view = sessionsListAdapter.GetView(i, null, sessionsList);
                sessionsList.AddView(view);
            }
            UpdateButtons();
            
        }
        private void InitWorkshopComponents()
        {
            InitComponents();
            sessionsList = FindViewById<LinearLayout>(Resource.Id.listViewSessions);
            sessionsList.Orientation = Orientation.Vertical;
            sessionsListAdapter = new SessionListAdapter(LayoutInflater);
            sessionContainer = FindViewById<RelativeLayout>(Resource.Id.sessionContainer);
            if (workshop.Sessions.Count == 0)
                sessionContainer.Visibility = ViewStates.Gone;
            bookingsContainer = FindViewById<RelativeLayout>(Resource.Id.bookingsContainer);
            if (workshop.FilledPlaces == -1)
                bookingsContainer.Visibility = ViewStates.Gone;
            bookButton = FindViewById<Button>(Resource.Id.BookBtn);
            cancelButton = FindViewById<Button>(Resource.Id.CancelBtn);
            waitlistButton = FindViewById<Button>(Resource.Id.WaitlistBtn);
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
    }
}