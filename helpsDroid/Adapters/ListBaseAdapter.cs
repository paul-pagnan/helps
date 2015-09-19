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
    
    public class ListBaseAdapter : BaseAdapter
    {
        public static Random rng = new Random();
        public List<Color> colors;
        public List<MyList> TheList;

        public void PopulateList(List<MyList> list)
        {
            TheList = list;
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

        public Color GetColor(int id)
        {
            if (id < 0)
                return Color.Transparent;
            try { 
            if (colors[id] != null)
                return colors[id];
            } catch(Exception ex)
            {
                return colors[rng.Next(colors.Count)];
            }
            return colors[rng.Next(colors.Count)];
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return null;
        }

        public void InitColors(View view)
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