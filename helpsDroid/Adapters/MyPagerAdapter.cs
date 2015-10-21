using Android.App;
using Android.Views;
using helps.Droid;
using Java.Lang;

namespace helps.Droid.Adapters
{
    public class MyPagerAdapter : Android.Support.V13.App.FragmentPagerAdapter
    {
        private readonly string[] Titles =
        {
            "Current", "Past"
        };

        private LayoutInflater layoutInflater;
        private bool isWorkshop;

        public MyPagerAdapter(FragmentManager fm, LayoutInflater layoutInflater, bool isWorkshop) : base(fm)
        {
            this.layoutInflater = layoutInflater;
            this.isWorkshop = isWorkshop;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(Titles[position]);
        }

        public override int Count
        {
            get { return Titles.Length; }
        }

        public override Fragment GetItem(int position)
        {
            return BookingsTabFragment.NewInstance(position, isWorkshop);
        }
    }
}