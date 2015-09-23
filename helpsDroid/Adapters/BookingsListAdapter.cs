using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using helps.Droid.Adapters.DataObjects;
using Android.Graphics;
using System.Linq;

namespace helps.Droid.Adapters
{
    public class BookingsListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_Booking;
        private List<Session> SessionList;
        private bool ShowLocation;
        private Android.Content.Res.Resources resources;

        private ImageView ImgColor;
        private TextView TxtName;
        private TextView TxtWorkshopSetName;
        private TextView TxtDate;
        private TextView TxtTime;
        private TextView TxtLocation;
        private TextView TxtLocationIcon;

        public BookingsListAdapter(LayoutInflater inflater, Android.Content.Res.Resources resources, bool ShowLocation) 
        {
            this.inflater = inflater;
            SessionList = new List<Session>();
            this.resources = resources;
            PopulateList();
        }

        public void PopulateList()
        {
            int count = 0;
            for(int i = 0; i <= 3; i++)
            {
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Writing an Essay",
                    WorkshopSet = count++,
                    WorkshopSetName = "Essay Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "Tomorrow",
                    Location = "CB11.05.401",
                    FilledPlaces = 10,
                    TotalPlaces = 25
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Presenting Skills 101",
                    WorkshopSet = count++,
                    WorkshopSetName = "Reading and Writing Skills",
                    Time = "2 - 4pm",
                    DateHumanFriendly = "Thu 25/09/15",
                    Location = "CB11.08.401",
                    FilledPlaces = 5,
                    TotalPlaces = 14
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Writing a literature review",
                    WorkshopSet = count++,
                    WorkshopSetName = "Writing Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "Wed 06/10/15",
                    Location = "CB06.02.180",
                    FilledPlaces = 30,
                    TotalPlaces = 30
                });
            }
            //Shuffle<Session>(SessionList);
            //base.PopulateList(SessionList.Select(x => new MyList() { Id = x.Id }).ToList());
        }

        public override void Clear()
        {
            SessionList.Clear();
            base.Clear();
        }

        public void AddAll(List<Session> sessions)
        {
            SessionList = sessions;
            base.AddAll(SessionList.Select(x => new MyList() { Id = x.Id }).ToList());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? inflater.Inflate(ItemLayout, parent, false);

            base.InitColors(view);
            InitComponents(view);
            SetComponents(position);
            return view;
        }

        private void SetComponents(int position)
        {
            ImgColor.SetBackgroundColor(GetColor(position));
            TxtName.Text = SessionList[position].Name;
            TxtWorkshopSetName.Text = SessionList[position].WorkshopSetName;
            TxtDate.Text = SessionList[position].DateHumanFriendly;
            TxtTime.Text = SessionList[position].Time;

            if (ShowLocation)
            {
                TxtLocation.Text = SessionList[position].Location;
                TxtLocationIcon.Text = resources.GetString(Resource.String.fa_map_marker);
            }
            else
            {
                TxtLocation.Text = SessionList[position].FilledPlaces + "/" + SessionList[position].TotalPlaces;
                TxtLocationIcon.Text = resources.GetString(Resource.String.fa_users);
                if (SessionList[position].FilledPlaces >= SessionList[position].TotalPlaces)
                    TxtLocation.SetTextColor(Color.Red);
                else
                    TxtLocation.SetTextColor(Color.ParseColor(resources.GetString(Resource.Color.primary_text_default_material_light)));
            }          
        }

        private void InitComponents(View view)
        {
            ImgColor = view.FindViewById<ImageView>(Resource.Id.BookingColor);
            TxtName = view.FindViewById<TextView>(Resource.Id.BookingName);
            TxtWorkshopSetName = view.FindViewById<TextView>(Resource.Id.BookingWorkshopSetName);
            TxtDate = view.FindViewById<TextView>(Resource.Id.BookingDate);
            TxtTime = view.FindViewById<TextView>(Resource.Id.BookingTime);
            TxtLocation = view.FindViewById<TextView>(Resource.Id.BookingLocation);
            TxtLocationIcon = view.FindViewById<TextView>(Resource.Id.icoLocation);
        }
    }
}