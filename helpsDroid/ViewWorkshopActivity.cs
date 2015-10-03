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

namespace helps.Droid
{
    [Activity(Label = "", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class ViewWorkshopActivity : Main
    {
        private WorkshopDetail workshop;
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

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewWorkshop; }
        }

        protected override void OnCreate(Bundle bundle)
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

        private void InitComponents()
        {
            title = FindViewById<TextView>(Resource.Id.title);
            room = FindViewById<TextView>(Resource.Id.textViewRoomValue);
            date = FindViewById<TextView>(Resource.Id.textViewDateValue);
            targetGroup = FindViewById<TextView>(Resource.Id.textViewTargetGroupValue);
            whatItCovers = FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue);
            placeAvailable = FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue);

            sessionsList = FindViewById<LinearLayout>(Resource.Id.listViewSessions);
            sessionsList.Orientation =  Orientation.Vertical;
            sessionsListAdapter = new SessionListAdapter(this.LayoutInflater);

            sessionContainer = FindViewById<RelativeLayout>(Resource.Id.sessionContainer);
            if (workshop.Sessions.Count == 0)
                sessionContainer.Visibility = ViewStates.Gone;
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
                //TODO Update book button and refresh the page
            }
            else
                DialogHelper.ShowDialog(this, response.Message, response.Title);
        }
    }
}