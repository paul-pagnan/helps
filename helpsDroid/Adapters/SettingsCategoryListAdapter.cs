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
using Android.Content.Res;
using helps.Shared;
using helps.Shared.DataObjects;

namespace helps.Droid.Adapters
{
    public class SettingsCategoryListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_SettingsCategory;
        List<MyList> Settings;

        private TextView TxtName;
        private Resources resources;

        public SettingsCategoryListAdapter(LayoutInflater inflater, Resources resources) 
        {
            this.inflater = inflater;
            this.resources = resources;
            Settings = new List<MyList>();
        }

        public override void Clear()
        {
            Settings.Clear();
            base.Clear();
        }
        public void AddAllSettings(List<MyList> settings)
        {
            Settings = settings;
            base.AddAll(Settings.Select(x => new MyList() { Id = x.Id,title = x.title }).ToList());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? inflater.Inflate(ItemLayout, parent, false);
            InitComponents(view, position);
            return view;
        }
        
        private void InitComponents(View view, int position)
        {
            view.FindViewById<TextView>(Resource.Id.SettingsName).Text = Settings[position].title;
            if (position == 0)
            {
                view.FindViewById<CheckBox>(Resource.Id.notificationsEnabled).Checked = SettingService.notificationsEnbabled();
                view.FindViewById<CheckBox>(Resource.Id.notificationsEnabled).Visibility = ViewStates.Visible;
            }

            if(Settings[position].hideArrow)
                view.FindViewById(Resource.Id.icoArrow).Visibility = ViewStates.Gone;
        }
    }
}