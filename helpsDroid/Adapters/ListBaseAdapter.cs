using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using helps.Droid.Adapters.DataObjects;
using Android.Graphics;

namespace helps.Droid.Adapters
{
    
    public class ListBaseAdapter : BaseAdapter
    {
        public List<MyList> TheList;
        private static List<Color> colors;

        public ListBaseAdapter()
        {
            TheList = new List<MyList>();
        }

        public virtual void AddAll(List<MyList> list)
        {
            TheList = list;
        }
        
        public virtual void Clear()
        {
            TheList.Clear();
        }

        public override int Count
        {
            get { return TheList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return TheList[position].Id;
        }
      
        public static Color GetColor(int id)
        {
            Random random = new Random(id);

            return colors[random.Next(colors.Count - 1)];
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return null;
        }

        public static void InitColors(Resources res)
        {
            colors = new List<Color>();
            colors.Add(res.GetColor(Resource.Color.cat_1));
            colors.Add(res.GetColor(Resource.Color.cat_2));
            colors.Add(res.GetColor(Resource.Color.cat_3));
            colors.Add(res.GetColor(Resource.Color.cat_4));
            colors.Add(res.GetColor(Resource.Color.cat_5));
            colors.Add(res.GetColor(Resource.Color.cat_6));
            colors.Add(res.GetColor(Resource.Color.cat_7));
            colors.Add(res.GetColor(Resource.Color.cat_8));
            colors.Add(res.GetColor(Resource.Color.cat_9));
            colors.Add(res.GetColor(Resource.Color.cat_10));
            colors.Add(res.GetColor(Resource.Color.cat_11));
            colors.Add(res.GetColor(Resource.Color.cat_12));
            colors.Add(res.GetColor(Resource.Color.cat_13));
            colors.Add(res.GetColor(Resource.Color.cat_14));
            colors.Add(res.GetColor(Resource.Color.cat_15));
            colors.Add(res.GetColor(Resource.Color.cat_16));
            colors.Add(res.GetColor(Resource.Color.cat_17));
        }
    }
}