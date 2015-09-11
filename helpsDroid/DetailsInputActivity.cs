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

namespace helps.Droid
{
    [Activity(Label = "Details Input", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.AppTheme")]
    public class DetailsInputActivity : Main
    {
        protected override async void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.Activity_DetailsInput);
            var t = FindViewById<Toolbar>(Resource.Id.TtoolbarTransparent);

            SetActionBar(t);
            setPadding(t);
            base.OnCreate(bundle);
        }
    }
}