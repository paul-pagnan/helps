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
    public class WorkshopCategoryListAdapter : ListBaseAdapter
    {
        private LayoutInflater inflater;
        private int ItemLayout = Resource.Layout.ListItem_WorkshopCategory;
        private List<WorkshopCategory> TheList;

        private ImageView ImgColor;
        private TextView TxtName;

        public WorkshopCategoryListAdapter(LayoutInflater inflater) 
        {
            this.inflater = inflater;
            TheList = new List<WorkshopCategory>();
            PopulateList();
        }

        public void PopulateList()
        {
            int count = 0;
            TheList.Add(new WorkshopCategory
            {
                Id = -1,
                Name = "View All"
            });
            for (int i = 0; i <= 3; i++)
            {
                TheList.Add(new WorkshopCategory
                {
                    Id = count++,
                    Name = "Writing an Essay"
                });
                TheList.Add(new WorkshopCategory
                {
                    Id = count++,
                    Name = "Presenting Skills 101"
                });
                TheList.Add(new WorkshopCategory
                {
                    Id = count++,
                    Name = "Writing a literature review"
                });
            }
            //Shuffle<Session>(SessionList);
            base.PopulateList(TheList.Select(x => new MyList() { Id = x.Id }).ToList());
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
            ImgColor.SetBackgroundColor(GetColor(TheList[position].Id));
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