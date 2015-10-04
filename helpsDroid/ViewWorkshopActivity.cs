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
using Android.Animation;
using com.refractored.fab;
using Newtonsoft.Json.Bson;
using Android.Views.Animations;
using Java.Lang;

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
        private int colorDiff = 35;

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


            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                //Get vars from bundle
                int workshopId = extras.GetInt("WorkshopId");

                string[] colorArr = extras.GetString("Color").Split(',');
                color = Color.Argb(Int32.Parse(colorArr[0]), Int32.Parse(colorArr[1]), Int32.Parse(colorArr[2]),
                    Int32.Parse(colorArr[3]));

                //Style the view to match workshop
                Toolbar.SetBackgroundColor(color);
                toolbarLayout = FindViewById<RelativeLayout>(Resource.Id.layouttoolbarLarge);
                toolbarLayout.SetBackgroundColor(color);
                Toolbar.NavigationIcon = Resources.GetDrawable(Resource.Drawable.ic_close_white_24dp);

                if (IsBooking = extras.GetBoolean("IsBooking"))
                    workshop = await Services.Workshop.GetWorkshopFromBooking(workshopId);
                else
                    workshop = Services.Workshop.GetWorkshop(workshopId);

                //Update the view
                InitComponents();
                UpdateFields();

                if (bundle != null)
                {
                    int flipperPosition = bundle.GetInt("TAB_NUMBER");
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

            fab.ColorNormal = Color.Argb(color.A, color.R + colorDiff, color.G + colorDiff, color.B + colorDiff);
            fab.ColorPressed = Color.Argb(color.A, color.R + (colorDiff / 3), color.G + (colorDiff / 3), color.B + (colorDiff / 3));
            fab.ColorRipple = color;
            fab.HasShadow = true;
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
            booking = await Services.Workshop.GetBooking(workshop.Id, localOnly, force, true);
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

            bookingsContainer = FindViewById<RelativeLayout>(Resource.Id.bookingsContainer);
            if (workshop.FilledPlaces == -1)
                bookingsContainer.Visibility = ViewStates.Gone;

            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //if (IsBooking)
                fab.Visibility = ViewStates.Visible;

            bookButton = FindViewById<Button>(Resource.Id.BookBtn);
            cancelButton = FindViewById<Button>(Resource.Id.CancelBtn);
            waitlistButton = FindViewById<Button>(Resource.Id.WaitlistBtn);

            mainLayout = FindViewById<ScrollView>(Resource.Id.mainLayout);
            editLayout = FindViewById<ScrollView>(Resource.Id.editLayout);
            flipper = FindViewById<ViewFlipper>(Resource.Id.flipper);
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
        public void Cancel(View view)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
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
            ProgressDialog dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();
            GenericResponse response = null;

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

        [Java.Interop.Export()]
        public async void Edit(View view)
        {
            Animation animOut = AnimationUtils.LoadAnimation(this, Resource.Animation.fadeout);
            fab.StartAnimation(animOut);
            animOut.AnimationEnd += (sender, e) =>
            {
                IsEditing = !IsEditing;
                AnimateButton();
                FlipView();
                //Do something productive here
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
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(
                    "Are you sure you want to discard changes to this booking?");
                builder.SetCancelable(false);
                builder.SetPositiveButton("Keep Editing", delegate {  });
                builder.SetNegativeButton("Discard", delegate { IsEditing = !IsEditing; AnimateButton(); FlipView(true); });
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

            int opp = (IsEditing) ? (255 / 2) : 0;
            fab.ColorNormal = Color.Argb(color.A, color.R + opp + colorDiff, color.G + opp + colorDiff, color.B + opp + colorDiff);
            fab.ColorPressed = Color.Argb(color.A, color.R + opp + (colorDiff / 3), color.G + opp + (colorDiff / 3), color.B + opp + (colorDiff / 3));
            fab.ColorRipple = Color.Argb(color.A, color.R + opp, color.G + opp, color.B + opp);

            Animation animIn = AnimationUtils.LoadAnimation(this, Resource.Animation.fadein);
            fab.StartAnimation(animIn);
        }

        

        public bool FlipView(bool trash = false)
        {
            int index = flipper.DisplayedChild;
            int InAnimation = (index > 0) ? Resource.Animation.slide_in_from_left : Resource.Animation.appear_from_top_right;
            int OutAnimation = (index > 0) ? Resource.Animation.dissapear_to_top_right : Resource.Animation.slide_out_to_left;

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
            int position = flipper.DisplayedChild;
            bundle.PutInt("TAB_NUMBER", position);
        }
    }
}