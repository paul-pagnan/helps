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
using System.Globalization;
using Android.Graphics;
using helps.Shared.DataObjects;
using helps.Shared.Consts;
using helps.Droid.Helpers;
using helps.Droid.Adapters;
using System.Threading.Tasks;

namespace helps.Droid
{
    [Activity(Label = "", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class ViewWorkshopActivity : Main
    {
        private WorkshopDetail workshop;
        private WorkshopBooking booking; 

        private TextView title;
        private TextView room;
        private TextView date;
        private TextView targetGroup;
        private TextView whatItCovers;
        private TextView placeAvailable;
        private LinearLayout sessionsList;
        private RelativeLayout toolbarLayout;
        private RelativeLayout sessionContainer;

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

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);


            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                int workshopId = extras.GetInt("WorkshopId");
                string[] colorArr = extras.GetString("Color").Split(',');
                var color = Color.Argb(Int32.Parse(colorArr[0]), Int32.Parse(colorArr[1]), Int32.Parse(colorArr[2]),
                    Int32.Parse(colorArr[3]));
                Toolbar.SetBackgroundColor(color);
                toolbarLayout = FindViewById<RelativeLayout>(Resource.Id.layouttoolbarLarge);
                toolbarLayout.SetBackgroundColor(color);
                Toolbar.NavigationIcon = Resources.GetDrawable(Resource.Drawable.ic_close_white_24dp);
                workshop = Services.Workshop.GetWorkshop(workshopId);
                InitComponents();
                UpdateFields();

                //Get Local Data First, then update later
                await Task.Factory.StartNew(() => LoadBooking(true));

                //Do a background Sync now
                await Task.Factory.StartNew(() => LoadBooking(false, false));
            }
        }

        private void UpdateFields()
        {
            title.Text = workshop.Title;
            room.Text = workshop.Room;

            var TimeString = (workshop.Time != null) ? System.Environment.NewLine + workshop.Time : "";
            date.Text = workshop.DateHumanFriendly + TimeString;

            targetGroup.Text = workshop.TargetGroup;
            whatItCovers.Text = workshop.Description;
            placeAvailable.Text = workshop.FilledPlaces + "/" + workshop.TotalPlaces;

            sessionsListAdapter.AddAll(workshop.Sessions);

            for (int i = 0; i < sessionsListAdapter.Count; i++)
            {
                var view = sessionsListAdapter.GetView(i, null, sessionsList);
                sessionsList.AddView(view);
            }
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

        private async void LoadBooking(bool localOnly, bool force = false)
        {
            booking = await Services.Workshop.GetBooking(workshop.Id, localOnly, force);
            RunOnUiThread(delegate
            {
                UpdateButtons();
            });
        }

        private void InitComponents()
        {
            title = FindViewById<TextView>(Resource.Id.title);
            room = FindViewById<TextView>(Resource.Id.textViewRoomValue);
            date = FindViewById<TextView>(Resource.Id.textViewDateValue);
            targetGroup = FindViewById<TextView>(Resource.Id.textViewTargetGroupValue);
            whatItCovers = FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue);
            placeAvailable = FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue);

            sessionsList = FindViewById<LinearLayout>(Resource.Id.listViewSessions);
            sessionsList.Orientation = Orientation.Vertical;
            sessionsListAdapter = new SessionListAdapter(this.LayoutInflater);

            sessionContainer = FindViewById<RelativeLayout>(Resource.Id.sessionContainer);
            if (workshop.Sessions.Count == 0)
                sessionContainer.Visibility = ViewStates.Gone;

            bookButton = FindViewById<Button>(Resource.Id.BookBtn);
            cancelButton = FindViewById<Button>(Resource.Id.CancelBtn);
            waitlistButton = FindViewById<Button>(Resource.Id.WaitlistBtn);
        }
    
        [Java.Interop.Export()]
        public async void Book(View view)
        {
            ProgressDialog dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;
            if (workshop.ProgramId.HasValue)
                response = await Services.Workshop.BookProgram(workshop.ProgramId.Value);
            else
                response = await Services.Workshop.Book(workshop.Id);
            dialog.Hide();

            if (response.Success)
            {
                bookButton.Visibility = ViewStates.Gone;
                cancelButton.Visibility = ViewStates.Visible;
                DialogHelper.ShowDialog(this, "You have been successfully booked into this workshop", "Workshop Booked");
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }

        [Java.Interop.Export()]
        public async void Cancel(View view)
        {
            ProgressDialog dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;
            if (workshop.ProgramId.HasValue)
                response = await Services.Workshop.CancelProgram(workshop.ProgramId.Value);
            else
                response = await Services.Workshop.CancelBooking(workshop.Id);
            dialog.Hide();

            if (response.Success)
            {
                bookButton.Visibility = ViewStates.Visible;
                cancelButton.Visibility = ViewStates.Gone;
                DialogHelper.ShowDialog(this, "The workshop has been successfully cancelled", "Workshop Cancelled");
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }
    }
}