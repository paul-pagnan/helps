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
using helps.Shared.DataObjects;
using helps.Shared.Consts;
using helps.Droid.Helpers;

namespace helps.Droid
{
    [Activity(Label = "My Information", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class DetailsInputActivity : Main
    {
        private Spinner country;
        private Spinner language;
        private TextView name;
        private ViewFlipper viewFlipper;
        private EditText date;
        private EditText month;
        private EditText year;
        private RadioGroup degree;
        private RadioGroup status;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_DetailsInput; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            InitComponents();

            var extras = Intent.Extras;
            if (extras != null)
            {
                var skip = extras.GetBoolean("Skip");
                if (skip)
                {
                    viewFlipper.ShowNext();
                    ActionBar.SetDisplayHomeAsUpEnabled(true);
                    ActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
        }

        private void InitComponents()
        {
            name = FindViewById<TextView>(Resource.Id.textViewNameValue);
            name.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;
            country = FindViewById<Spinner>(Resource.Id.country);
            country.Adapter = GetCountries();
            language = FindViewById<Spinner>(Resource.Id.language);
            language.Adapter = GetLanguages();
            date = FindViewById<EditText>(Resource.Id.studentDateDOB);
            month = FindViewById<EditText>(Resource.Id.studentMonthDOB);
            year = FindViewById<EditText>(Resource.Id.studentYearDOB);
            degree = (RadioGroup)FindViewById(Resource.Id.radioGroupDegree);
            status = (RadioGroup)FindViewById(Resource.Id.radioGroupStatus);
            viewFlipper = FindViewById<ViewFlipper>(Resource.Id.welcomeviewflipper);
            Populate();
        }

        private async void Populate()
        {
            var student = await Services.Student.GetStudent();
            if (student != null)
            {
                if (student.dob.HasValue)
                {
                    date.Text = ParseTwoDigits(student.dob.Value.Day.ToString());
                    month.Text = ParseTwoDigits(student.dob.Value.Month.ToString());
                    year.Text = student.dob.Value.Year.ToString();
                }
                if (student.country_origin != null)
                    country.Adapter = GetCountries(student.country_origin);

                if (student.first_language != null)
                    language.Adapter = GetLanguages(student.first_language);

                if (student.degree != null)
                    degree.Check(degree.GetChildAt((student.degree.Trim() == "UG") ? 0 : 1).Id);

                if (student.status != null)
                    status.Check(status.GetChildAt((student.status.Trim() == "Permanent") ? 0 : 1).Id);
            }
        }

        private string ParseTwoDigits(string v)
        {
            return (v.Length > 1) ? v : "0" + v;
        }

        [Java.Interop.Export()]
        public void Flip(View view)
        {
            viewFlipper.SetInAnimation(this, Resource.Animation.slide_in_from_right);
            viewFlipper.SetOutAnimation(this, Resource.Animation.slide_out_to_left);
            viewFlipper.ShowNext();
        }

        [Java.Interop.Export()]
        public async void Submit(View view)
        {
            ProgressDialog dialog = DialogHelper.CreateProgressDialog("Registering...", this);
            dialog.Show();
            RadioButton degreeIndex = (RadioButton)FindViewById(degree.CheckedRadioButtonId);
            RadioButton statusIndex = (RadioButton)FindViewById(status.CheckedRadioButtonId);
            try
            {
                var dob = DateTime.ParseExact(year.Text + month.Text + date.Text, "yyyyMMdd", CultureInfo.InvariantCulture);
                var request = new HelpsRegisterRequest
                {
                    StudentId = CurrentUser.StudentId,
                    DateOfBirth = dob,
                    Degree = degree.IndexOfChild(degreeIndex) + 1,
                    Status = status.IndexOfChild(statusIndex),
                    FirstLanguage = language.SelectedItem.ToString(),
                    CountryOrigin = country.SelectedItem.ToString()
                };
                var Response = await Services.Student.RegisterStudent(request);
                dialog.Hide();
                if (Response.Success)
                {
                    var intent = new Intent(this, typeof(DashboardActivity));
                    StartActivity(intent);
                    Finish();
                }
                else
                    DialogHelper.ShowDialog(this, Response.Message, Response.Title);
            }
            catch (Exception)
            {
                dialog.Hide();
                DialogHelper.ShowDialog(this, "Error", "Please enter a valid date of birth");
            }
        }

        private ArrayAdapter GetCountries(string overrideValue = "Australia")
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
            return SortAndAssign(countries, overrideValue);
        }

        private ArrayAdapter GetLanguages(string overrideValue = "English")
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
            return SortAndAssign(languages, overrideValue);
        }

        private ArrayAdapter SortAndAssign(LinkedList<string> list, string DefaultValue)
        {
            List<string> sortedList = list.ToList();
            sortedList.Sort();
            sortedList.Insert(0, DefaultValue);
            ArrayAdapter arr = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line,
                sortedList.ToArray<string>());
            arr.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
            return arr;
        }
    }
}