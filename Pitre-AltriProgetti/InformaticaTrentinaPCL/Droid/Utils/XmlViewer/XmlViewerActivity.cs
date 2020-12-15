using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace InformaticaTrentinaPCL.Droid.Utils.XmlViewer
{
    
    /// <summary>
    /// Classe usata per aprire i file XML mediante ACTION_VIEW Intent
    /// </summary>
    
    [Activity(Label = "Xml Viewer")]
    [IntentFilter (new[]{Intent.ActionView},
        Categories=new[]{Intent.CategoryDefault},
        DataSchemes=new[]{"file","content"},
        DataHost="it.pi3.informaticatrentina",
        DataPathPattern=".*\\.xml",
        DataMimeType="*/*")]
    public class XmlViewerActivity : AppCompatActivity
    {
        protected int LayoutResource => Resource.Layout.activity_xml_reader;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                
            SetContentView(LayoutResource);
            
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                toolbar.Title = "XML Reader";
                toolbar.SetNavigationIcon(Resource.Drawable.ic_ico_back_grigia);
            }
            
            FindViewById<WebView>(Resource.Id.webview).LoadUrl(Intent.Data.ToString());

        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}