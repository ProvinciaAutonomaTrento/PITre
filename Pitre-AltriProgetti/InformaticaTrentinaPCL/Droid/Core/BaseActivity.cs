using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Calligraphy;
using InformaticaTrentinaPCL.Droid.Home;
using Java.Lang;

namespace InformaticaTrentinaPCL.Droid
{
    public class BaseActivity : AppCompatActivity
    {
        
        public string Title
        {
            get
            {
                if (this.TitleFormatted != null)
                    return this.TitleFormatted.ToString();
                return (string) null;
            }
            set
            {
                this.TitleFormatted = null;
                throw new RuntimeException("INVOKE SetPageTitle or SetSectionTitle to set Activity Title");
            }
        }

        public void SetPageTitle(string title)
        {
            TitleFormatted = null;
            Toolbar.UpdateTitle(title);
        }

        public void SetSectionTitle(string title)
        {
            TitleFormatted = null;
            Toolbar.UpdateToolbarForSectionType(SupportActionBar, title, OnBackPressed);
        }
                
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResource);
            Toolbar = FindViewById<CustomToolbar>(Resource.Id.toolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }
            
            if (Resources.GetBoolean(Resource.Boolean.isSmartphone))
            {
                RequestedOrientation = ScreenOrientation.Portrait;
            }
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
		{
			base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
		}

        public CustomToolbar Toolbar
        {
            get;
            set;
        }

        protected virtual int LayoutResource
        {
            get;
        }

        protected int ActionBarIcon
        {
            set { Toolbar?.SetNavigationIcon(value); }
        }
    }
}
