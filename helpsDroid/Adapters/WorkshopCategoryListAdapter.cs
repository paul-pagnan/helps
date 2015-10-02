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
using helps.Shared.DataObjects;

namespace helps.Droid.Adapters
{
    public class WorkshopCategoryListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_WorkshopCategory;
        private List<WorkshopSet> TheList;

        private ImageView ImgColor;
        private TextView TxtName;
        private Resources resources;

        public WorkshopCategoryListAdapter(LayoutInflater inflater, Resources resources) 
        {
            this.inflater = inflater;
            this.resources = resources;
            TheList = new List<WorkshopSet>();
        }

        public override void Clear()
        {
            TheList.Clear();
            base.Clear();
        }

        public void AddAll(List<WorkshopSet> workshops)
        {
            TheList = workshops;
            base.AddAll(TheList.Select(x => new MyList() { Id = x.Id }).ToList());
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? inflater.Inflate(ItemLayout, parent, false);

            base.InitColors(resources);
            InitComponents(view);
            SetComponents(position);
            return view;
        }

        private void SetComponents(int position)
        {
            ImgColor.SetBackgroundColor(GetColor(position));
            TxtName.Text = TheList[position].Name;

            if (TheList[position].Id < 0)
                TxtName.SetTypeface(Typeface.Create("sans-serif-light", TypefaceStyle.Bold), TypefaceStyle.Bold);
            else
                TxtName.SetTypeface(Typeface.Create("sans-serif-light", TypefaceStyle.Normal), TypefaceStyle.Normal);
        }

        private void InitComponents(View view)
        {
            ImgColor = view.FindViewById<ImageView>(Resource.Id.WorkshopColor);
            TxtName = view.FindViewById<TextView>(Resource.Id.WorkshopName);
        }
    }
}