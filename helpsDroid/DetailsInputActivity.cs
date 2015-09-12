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
        Spinner language;
        TextView name;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Init();
            SetContentView(Resource.Layout.Activity_DetailsInput);
            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            t.InflateMenu(Resource.Menu.logout);
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
            name = FindViewById<TextView>(Resource.Id.textViewNameValue);
            name.Text = AuthSvc.CurrentUser().FirstName + " " + AuthSvc.CurrentUser().LastName;

            country = FindViewById<Spinner>(Resource.Id.country);
            country.Adapter = GetCountries();

            language = FindViewById<Spinner>(Resource.Id.language);
            language.Adapter = GetLanguages();
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
            return SortAndAssign(countries, "Australia");
        }

        private ArrayAdapter GetLanguages()
        {

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            LinkedList<string> languages = new LinkedList<string>();
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    
                    if(culture.IsNeutralCulture)
                        languages.AddLast(culture.DisplayName);

                }
                catch { }
            }
            return SortAndAssign(languages, "English");
        }


        private ArrayAdapter SortAndAssign(LinkedList<string> list, string DefaultValue)
        {
            List<string> sortedList = list.ToList();
            sortedList.Sort();
            sortedList.Insert(0, DefaultValue);
            ArrayAdapter arr = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, sortedList.ToArray<string>());
            arr.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
            return arr;
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }
    }
}