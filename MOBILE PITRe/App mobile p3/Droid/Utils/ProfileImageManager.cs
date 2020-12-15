using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Lang;
using Android.Preferences;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Utils;
using Java.IO;
using Console = System.Console;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class ProfileImageManager
    {

        private Context context;
        private readonly string IMAGE_PATH = "imagePath.jpg";
        private readonly string KEY_IMAGE_SAVED = "KEY_IMAGE_SAVED";
        
        public ProfileImageManager(Context context)
        {
            this.context = context;
        }
        
        public async Task<Uri> SaveImage(Bitmap bitmap)
        {
            bool isImageValid = bitmap != null;
            
            //Save image on disk
            if (isImageValid)
            {
                var stream = new FileStream(context.FilesDir+"/"+IMAGE_PATH, FileMode.Create, FileAccess.ReadWrite, 
                    FileShare.None);       
                isImageValid = await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 70, stream); 
                bitmap.Recycle();
                stream.FlushAsync();
                stream.Close();
                stream.Dispose();
            }
            
            //save 
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (context);
            ISharedPreferencesEditor editor = prefs.Edit ();
            editor.PutBoolean(KEY_IMAGE_SAVED, isImageValid);
            editor.Apply();

            return GetImageUri(isImageValid);
        }

        public Uri LoadImage()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (context);          
            bool isImageValid = prefs.GetBoolean(KEY_IMAGE_SAVED, false);
            return GetImageUri(isImageValid);
        }

        //   public void ShowChooserDialog(Activity activity, Action takePhoto, Action choosePhoto, Action deletePhoto)
        public void ShowChooserDialog(Activity activity, Action choosePhoto, Action deletePhoto)
        {
            bool isImageSet = LoadImage() != null;
            List<String> items = new List<String>();
           items.Add(LocalizedString.LABEL_USE_GALLERY.Get());
            if (isImageSet){
                items.Add(LocalizedString.LABEL_REMOVE_IMAGE.Get());
            }
            
            ArrayAdapter<string> adapter =
                new ArrayAdapter<string>(activity, Resource.Layout.simple_list_item);
            adapter.AddAll(items);
            
            using (var dialogBuilder = new AlertDialog.Builder (activity))
            {
                TextView title = (TextView) LayoutInflater.From(activity)
                    .Inflate(Resource.Layout.view_lista_ragioni_title_dialog, null, false);
                title.Text = LocalizedString.SELECT_USER_IMAGE.Get();
                dialogBuilder.SetCustomTitle(title);
                dialogBuilder.SetAdapter (adapter, (d, args) => {
                    switch (args.Which)
                    {
                        
                        case 0:
                            choosePhoto();
                            break;
                                
                        case 1:
                            deletePhoto();
                            break;
                    }                   
                });
                dialogBuilder.Show ();
            }
        }

        

        private Uri GetImageUri(bool isImageSet)
        {
            if (isImageSet)
            {
                File dir = context.FilesDir;
                File assist = new File(dir, IMAGE_PATH);
                return FileProvider.GetUriForFile(context, "it.pi3.informaticatrentina", assist);
            }
            return null;
        }
    }
}