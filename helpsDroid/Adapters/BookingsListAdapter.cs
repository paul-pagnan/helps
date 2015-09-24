using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Linq;
using helps.Shared.DataObjects;
using helps.Droid.Adapters.DataObjects;

namespace helps.Droid.Adapters
{
    public class BookingsListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_Booking;
        private List<WorkshopPreview> WorkshopList;
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
            WorkshopList = new List<WorkshopPreview>();
            this.resources = resources;
            this.ShowLocation = ShowLocation;
        }

        public override void Clear()
        {
            WorkshopList.Clear();
            base.Clear();
        }

        public void AddAll(List<WorkshopPreview> workshops)
        {
            WorkshopList = workshops;
            base.AddAll(WorkshopList.Select(x => new MyList() { Id = x.Id }).ToList());
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
            TxtName.Text = WorkshopList[position].Name;
            TxtWorkshopSetName.Text = WorkshopList[position].WorkshopSetName;
            TxtDate.Text = WorkshopList[position].DateHumanFriendly;
            TxtTime.Text = WorkshopList[position].Time;

            if (ShowLocation)
            {
                TxtLocation.Text = WorkshopList[position].Location;
                TxtLocationIcon.Text = resources.GetString(Resource.String.fa_map_marker);
            }
            else
            {
                TxtLocation.Text = WorkshopList[position].FilledPlaces + "/" + WorkshopList[position].TotalPlaces;
                TxtLocationIcon.Text = resources.GetString(Resource.String.fa_users);
                if (WorkshopList[position].FilledPlaces >= WorkshopList[position].TotalPlaces)
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