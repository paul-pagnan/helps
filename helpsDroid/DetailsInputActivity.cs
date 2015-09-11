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
using Java.Util;

namespace helps.Droid
{
    [Activity(MainLauncher = true, Icon = "@drawable/helps_icon", Label = "Details Input", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class DetailsInputActivity : Main
    {
        Spinner spinnerYear;
        AutoCompleteTextView country;
                
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

            country = FindViewById<AutoCompleteTextView>(Resource.Id.country);
            country.Adapter = arr;
        }

        //private ArrayAdapter GetCountries()
        //{
        //    Locale[] locales = Locale.GetAvailableLocales();
        //    string[] countries = new String();

        //    foreach (Locale locale in locales)
        //    {
        //        string country = locale.GetDisplayCountry(locale);
        //        countries.AddLast(country);
        //    }

        //    ArrayAdapter arr = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, );
        //    arr.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
        //    return arr;
        //}
    }
}