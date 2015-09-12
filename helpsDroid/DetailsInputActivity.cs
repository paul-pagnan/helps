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
using System.Globalization;

namespace helps.Droid
{
    [Activity(Label = "Details Input", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class DetailsInputActivity : Main
    {
        Spinner spinnerYear;
        Spinner country;
                
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Activity_DetailsInput);
            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            SetActionBar(t);
            setPadding(t);
            InitComponents();
        }

        private void InitComponents()
        {
            //init 
            spinnerYear = FindViewById<Spinner>(Resource.Id.spinnerYear);
            ArrayAdapter arr = ArrayAdapter.CreateFromResource(this, Resource.Array.year, Android.Resource.Layout.SimpleDropDownItem1Line);
            arr.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
            spinnerYear.Adapter = arr;

            country = FindViewById<Spinner>(Resource.Id.country);
            country.Adapter = GetCountries();
        }


        private ArrayAdapter GetCountries()
        {
            
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            LinkedList<string> countries = new LinkedList<string>();
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    RegionInfo regionInfo = new RegionInfo(culture.LCID);
                    if (!(countries.Contains(regionInfo.EnglishName)))
                        countries.AddLast(regionInfo.EnglishName);
                }
                catch { }
            }
            List<string> sortedCountries = countries.ToList();
            sortedCountries.Sort();
            sortedCountries.Insert(0, "Australia");
            ArrayAdapter arr = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, sortedCountries.ToArray<string>());
            arr.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
            return arr;
        }
    }
}