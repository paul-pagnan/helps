using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using helps.Droid.Adapters.DataObjects;
using Android.Graphics;

namespace helps.Droid.Adapters
{
    public class BookingsListAdapter : BaseAdapter
    {
        List<Booking> bookingsList;
        LayoutInflater layoutInflater;
        private static Random rng = new Random();

        List<Color> colors;

        ImageView ImgColor;
        TextView TxtName;
        TextView TxtWorkshopSetName;
        TextView TxtDate;
        TextView TxtTime;
        TextView TxtLocation;


        public BookingsListAdapter(LayoutInflater layoutInflater)
        {
            this.layoutInflater = layoutInflater;
            FillBookings();
        }


        void FillBookings()
        {
            bookingsList = new List<Booking>();

            int count = 0;
            for(int i = 0; i <= 3; i++)
            {
                bookingsList.Add(new Booking
                {
                    Id = 111,
                    Name = "Writing an Essay",
                    WorkshopSet = count++,
                    WorkshopSetName = "Essay Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "Tomorrow",
                    Location = "CB11.05.401",
                });
                bookingsList.Add(new Booking
                {
                    Id = 111,
                    Name = "Presenting Skills 101",
                    WorkshopSet = count++,
                    WorkshopSetName = "Reading and Writing Skills",
                    Time = "2 - 4pm",
                    DateHumanFriendly = "25/09/15",
                    Location = "CB11.08.401",
                });
                bookingsList.Add(new Booking
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
            Shuffle<Booking>(bookingsList);
        }


        public override int Count
        {
            get { return bookingsList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return bookingsList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? layoutInflater.Inflate(
                Resource.Layout.ListItem_MyBookings, parent, false);

            InitColors(view);
            InitComponents(view);
            SetComponents(position);
            return view;
        }

        private void SetComponents(int position)
        {
            ImgColor.SetBackgroundColor(GetColor(bookingsList[position].WorkshopSet));
            TxtName.Text = bookingsList[position].Name;
            TxtWorkshopSetName.Text = bookingsList[position].WorkshopSetName;
            TxtDate.Text = bookingsList[position].DateHumanFriendly;
            TxtTime.Text = bookingsList[position].Time;
            TxtLocation.Text = bookingsList[position].Location;
        }

        public void InitComponents(View view)
        {
            ImgColor = view.FindViewById<ImageView>(Resource.Id.BookingColor);
            TxtName = view.FindViewById<TextView>(Resource.Id.BookingName);
            TxtWorkshopSetName = view.FindViewById<TextView>(Resource.Id.BookingWorkshopSetName);
            TxtDate = view.FindViewById<TextView>(Resource.Id.BookingDate);
            TxtTime = view.FindViewById<TextView>(Resource.Id.BookingTime);
            TxtLocation = view.FindViewById<TextView>(Resource.Id.BookingLocation);
        }

        public Color GetColor(int id)
        {
            if (colors[id] != null)
                return colors[id];

            int r = rng.Next(colors.Count);
            return colors[r];
        }

        private void InitColors(View view)
        {
            colors = new List<Color>();
            colors.Add(view.Resources.GetColor(Resource.Color.cat_1));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_2));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_3));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_4));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_5));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_6));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_7));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_8));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_9));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_10));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_11));
            colors.Add(view.Resources.GetColor(Resource.Color.cat_12));
        }

        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}