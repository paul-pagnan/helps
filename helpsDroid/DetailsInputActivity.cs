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
    [Activity(Label = "My Information", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class DetailsInputActivity : Main
    {
        private Spinner spinnerYear;
        private Spinner country;
        private Spinner language;
        private TextView name;
        private ViewFlipper viewFlipper;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Init();
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
            name = FindViewById<TextView>(Resource.Id.textViewNameValue);
            name.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;

            country = FindViewById<Spinner>(Resource.Id.country);
            country.Adapter = GetCountries();

            language = FindViewById<Spinner>(Resource.Id.language);
            language.Adapter = GetLanguages();

            viewFlipper = FindViewById<ViewFlipper>(Resource.Id.welcomeviewflipper);
        }


        [Java.Interop.Export()]
        public async void Flip(View view)
        {
            viewFlipper.SetInAnimation(this, Resource.Animation.slide_in_from_right);
            viewFlipper.SetOutAnimation(this, Resource.Animation.slide_out_to_left);
            viewFlipper.ShowNext();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.logout, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Logout();
            return base.OnOptionsItemSelected(item);
        }
    }
}