using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Android.Widget;
using helps.Shared;
using helps.Shared.DataObjects;

namespace helps.Droid
{
    public abstract class Main : Activity
    {
        public AuthService AuthSvc;
        public StudentService StudentSvc;
        public WorkshopService WorkshopSvc;
        public User CurrentUser;

        public Toolbar Toolbar { get; set; }

        protected abstract int LayoutResource { get; }

        public void Init()
        {
            AuthSvc = new AuthService();
            StudentSvc = new StudentService();
            WorkshopSvc = new WorkshopService();

            CurrentUser = AuthSvc.CurrentUser();
        }

        protected override void OnCreate(Bundle bundle)
        {
            Init();
            base.OnCreate(bundle);
            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            if (Toolbar == null)
                Toolbar = FindViewById<Toolbar>(Resource.Id.TtoolbarTransparent);
            if (Toolbar != null)
            {
                setPadding(Toolbar);
                SetActionBar(Toolbar);
                Toolbar.NavigationOnClick += (sender, e) =>
                {
                    Finish();
                };
            }
        }
        
        public void setPadding(Toolbar toolbar)
        {
            toolbar.SetPadding(0, getStatusBarHeight(), 0, 0);
        }

        public int getStatusBarHeight()
        {
            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                return Resources.GetDimensionPixelSize(resourceId);
            return 0;
        }        

        public void Logout()
        {
            AuthSvc.Logout();
            CurrentUser = null;
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_logout)
                Logout();
            return base.OnOptionsItemSelected(item);
        }
    }
}