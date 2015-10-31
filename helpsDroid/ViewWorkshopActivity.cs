using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
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
        private static WorkshopDetail session;
        private WorkshopBooking booking;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewWorkshop; }
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var extras = Intent.Extras;
            if (extras != null)
            {
                //Get vars from bundle
                var workshopId = extras.GetInt("Id");

                cts.Cancel();
                //Get the Booking Details
                if (extras.GetBoolean("IsBooking"))
                    session = await Services.Workshop.GetWorkshopFromBooking(workshopId);
                else
                {
                    session = Services.Workshop.GetWorkshop(workshopId);
                    HideEdit = true;
                }

                ListBaseAdapter.InitColors(Resources);
                SetToolbarColor(ListBaseAdapter.GetColor(session.WorkshopSetId));
                InitWorkshopComponents();
                UpdateFields();

                cts.Cancel();
                //Load booking information so the buttons can be updated
                //Get Local Data First, then update later
                await Task.Factory.StartNew(() => LoadBooking(true));
                //Do a background Sync now
                await Task.Factory.StartNew(() => LoadBooking(false, false));
            }
        }

        private void UpdateButtons()
        {
            bookButton.Visibility = ViewStates.Gone;
            cancelButton.Visibility = ViewStates.Gone;
            if (session.FilledPlaces >= session.TotalPlaces)
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
            cts.Cancel();
            booking = await Services.Workshop.GetBooking(cts.Token, session.Id, localOnly, force);
            RunOnUiThread(delegate { UpdateFields(); });
        }

        private void UpdateFields()
        {
            if (booking != null)
                ShowNotifications(session.Id, true);
            title.Text = session.Title;
            editTxtNotes.Text = session.Notes;

            FindViewById<TextView>(Resource.Id.textViewRoomValue).Text = session.Room;
            var TimeString = (session.Time != null) ? Environment.NewLine + session.Time : "";
            FindViewById<TextView>(Resource.Id.textViewDateValue).Text = session.DateHumanFriendly + TimeString;
            FindViewById<TextView>(Resource.Id.textViewTargetGroupValue).Text = session.TargetGroup;
            FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue).Text = session.Description;
            FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue).Text = session.FilledPlaces + "/" +
                                                                                   session.TotalPlaces;
            sessionsListAdapter.Clear();
            sessionsListAdapter.AddAll(session.Sessions);
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
            if (session.Sessions.Count == 0)
                sessionContainer.Visibility = ViewStates.Gone;
            bookingsContainer = FindViewById<RelativeLayout>(Resource.Id.bookingsContainer);
            if (session.FilledPlaces == -1)
                bookingsContainer.Visibility = ViewStates.Gone;
            bookButton = FindViewById<Button>(Resource.Id.BookBtn);
            cancelButton = FindViewById<Button>(Resource.Id.CancelBtn);
            waitlistButton = FindViewById<Button>(Resource.Id.WaitlistBtn);
            if (HideEdit)
                FindViewById<FloatingActionButton>(Resource.Id.fab).Visibility = ViewStates.Gone;
        }

        [Java.Interop.Export()]
        public async void Book(View view)
        {
            var dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;
            if (session.ProgramId.HasValue)
                response = await Services.Workshop.BookProgram(cts.Token, session.ProgramId.Value);
            else
                response = await Services.Workshop.Book(cts.Token, session.Id);
            dialog.Hide();

            if (response.Success)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Booked Successfully");
                builder.SetMessage("Would you like to set up notifications now?");
                builder.SetCancelable(false);
                builder.SetPositiveButton("Create Notifications", delegate { ShowNotificationDialog(session.Id, session.Date, true); });
                builder.SetNegativeButton("Close", delegate { });
                builder.Show();
                booking = new WorkshopBooking();
                UpdateButtons();
                ShowNotifications(session.Id, true);
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }

        [Java.Interop.Export()]
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
            cts.Cancel();
            response = await Services.Workshop.CancelBooking(cts.Token, session.Id);
            dialog.Hide();

            if (response.Success)
            {
                DialogHelper.ShowDialog(this, "The sessionId has been successfully cancelled", "Workshop Cancelled");
                NotificationHelper.Cancel(this, session.Id);
                booking = null;
                UpdateButtons();
                FindViewById<RelativeLayout>(Resource.Id.notifications).Visibility = ViewStates.Gone;
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }

        [Java.Interop.Export()]
        public void Edit(View view)
        {
            EditNotes(session);
        }

        [Java.Interop.Export()]
        public void ShowNotificationDialog(View view)
        {
            ShowNotificationDialog(session.Id, session.Date, true);
        }
    }
}