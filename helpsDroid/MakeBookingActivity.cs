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
using helps.Shared;
using helps.Shared.DataObjects;
using helps.Droid.Helpers;
using helps.Droid.Adapters;

namespace helps.Droid
{ 
    [Activity(Label = "Workshops", Theme = "@style/AppTheme.MyToolbar")]
    public class MakeBookingActivity : Main
    {
        private ViewFlipper viewFlipper;
        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_MakeBooking; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            InitMenu();

            viewFlipper = FindViewById<ViewFlipper>(Resource.Id.bookingflipper);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }

        private void InitMenu()
        {
            var workshopCategoryAdapter = new WorkshopCategoryListAdapter(this.LayoutInflater);
            var workshopSetListView = FindViewById<ListView>(Resource.Id.workshopSetList);
            workshopSetListView.Adapter = workshopCategoryAdapter;
            workshopSetListView.ItemClick += (sender, e) =>
            {
                InitWorkshopList(e.Id);
            };
        }

        private void InitWorkshopList(long id)
        {
            FlipView();
            DialogHelper.ShowDialog(this, "You clicked " + id, "Click Event");
        }

        public override void OnBackPressed()
        {
            if (Back())
                return;
            base.OnBackPressed();
        }

      
        public bool Back()
        {
            if (viewFlipper.DisplayedChild > 0)
                return FlipView();

            return false;
        }

        public bool FlipView()
        {
            int index = viewFlipper.DisplayedChild;
            int InAnimation = (index > 0) ? Resource.Animation.slide_in_from_left : Resource.Animation.slide_in_from_right;
            int OutAnimation = (index > 0) ? Resource.Animation.slide_out_to_right : Resource.Animation.slide_out_to_left;
                        
            viewFlipper.SetInAnimation(this, InAnimation);
            viewFlipper.SetOutAnimation(this, OutAnimation);
            if (index > 0)
                viewFlipper.ShowPrevious();
            else
                viewFlipper.ShowNext();
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}