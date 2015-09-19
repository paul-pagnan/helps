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
using helps.Droid.Adapters.DataObjects;
using Android.Graphics;

namespace helps.Droid.Adapters
{
    public class SessionListBaseAdapter : BaseAdapter
    {
        public LayoutInflater layoutInflater;
        public static Random rng = new Random();
        public List<Session> SessionList;
        public List<Color> colors;

        private ImageView ImgColor;
        private TextView TxtName;
        private TextView TxtWorkshopSetName;
        private TextView TxtDate;
        private TextView TxtTime;
        private TextView TxtLocation;

        public SessionListBaseAdapter(LayoutInflater layoutInflater)
        {
            this.layoutInflater = layoutInflater;
            SessionList = new List<Session>();
        }

        public override int Count
        {
            get { return SessionList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return SessionList[position].Id;
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

        private Color GetColor(int id)
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
    }
}