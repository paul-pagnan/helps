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
using Android.Graphics;
using Android.Util;

namespace helps.Droid.Extensions
{

    public class FontAwesome : TextView
    {

        public FontAwesome(Context context) : base(context) {
            init();
        }

        public FontAwesome(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs) {
            init();
        }

        public void init() {
            var typeface = Typeface.CreateFromAsset(Context.Assets, "fontawesome-webfont.ttf");
            SetTypeface(typeface, TypefaceStyle.Normal);
        }

    }
}