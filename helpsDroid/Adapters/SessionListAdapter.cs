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
    public class SessionListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_Session;
        private List<SessionPreview> SessionList;

        private TextView TxtTitle;

        private TextView TxtTime;
        private TextView TxtLocation;

        public SessionListAdapter(LayoutInflater inflater) 
        {
            this.inflater = inflater;
            SessionList = new List<SessionPreview>();
        }

        public override void Clear()
        {
            SessionList.Clear();
            base.Clear();
        }

        public void AddAll(List<SessionPreview> sessions)
        {
            SessionList = sessions;
            base.AddAll(SessionList.Select(x => new MyList() { Id = x.Id }).ToList());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? inflater.Inflate(ItemLayout, parent, false);

            InitComponents(view);
            SetComponents(position);
            return view;
        }

        private void SetComponents(int position)
        {
            TxtTitle.Text = SessionList[position].Title;
            TxtTime.Text = SessionList[position].Time;
            TxtLocation.Text = SessionList[position].Location;
        }

        private void InitComponents(View view)
        {
            TxtTitle = view.FindViewById<TextView>(Resource.Id.TxtTitle);
            TxtTime = view.FindViewById<TextView>(Resource.Id.TxtTime);
            TxtLocation = view.FindViewById<TextView>(Resource.Id.TxtLocation);
        }
    }
}