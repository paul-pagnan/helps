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
        private int ItemLayout = Resource.Layout.ListItem_MyBookings;
        private List<Session> SessionList;

        private ImageView ImgColor;
        private TextView TxtName;
        private TextView TxtWorkshopSetName;
        private TextView TxtDate;
        private TextView TxtTime;
        private TextView TxtLocation;

        public BookingsListAdapter(LayoutInflater inflater) 
        {
            this.inflater = inflater;
            SessionList = new List<Session>();
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
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Presenting Skills 101",
                    WorkshopSet = count++,
                    WorkshopSetName = "Reading and Writing Skills",
                    Time = "2 - 4pm",
                    DateHumanFriendly = "25/09/15",
                    Location = "CB11.08.401",
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Writing a literature review",
                    WorkshopSet = count++,
                    WorkshopSetName = "Writing Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "06/10/15",
                    Location = "CB06.02.180",
                });
            }
            //Shuffle<Session>(SessionList);
            base.PopulateList(SessionList.Select(x => new MyList() { Id = x.Id }).ToList());
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
            ImgColor.SetBackgroundColor(GetColor(SessionList[position].WorkshopSet));
            TxtName.Text = SessionList[position].Name;
            TxtWorkshopSetName.Text = SessionList[position].WorkshopSetName;
            TxtDate.Text = SessionList[position].DateHumanFriendly;
            TxtTime.Text = SessionList[position].Time;
            TxtLocation.Text = SessionList[position].Location;
        }

        private void InitComponents(View view)
        {
            ImgColor = view.FindViewById<ImageView>(Resource.Id.BookingColor);
            TxtName = view.FindViewById<TextView>(Resource.Id.BookingName);
            TxtWorkshopSetName = view.FindViewById<TextView>(Resource.Id.BookingWorkshopSetName);
            TxtDate = view.FindViewById<TextView>(Resource.Id.BookingDate);
            TxtTime = view.FindViewById<TextView>(Resource.Id.BookingTime);
            TxtLocation = view.FindViewById<TextView>(Resource.Id.BookingLocation);
        }
    }
}