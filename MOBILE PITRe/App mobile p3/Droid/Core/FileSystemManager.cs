using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using InformaticaTrentinaPCL.Network;
using Uri = Android.Net.Uri;

using InformaticaTrentinaPCL.OpenFile.MVP;


namespace InformaticaTrentinaPCL.Droid.Core
{
    public class FileSystemManager : IFileSystemManager
    {
        public FileSystemManager()
        {
        }
        
        public void saveFileAndOpen(byte[] inputBytes, string fileName, string extension, Object extra)
        {

            if (!(extra is Activity))
            {
                Console.WriteLine("Errore: Object Extra deve essere un'Activity");
                throw new ArgumentException();
            }

            Activity activity = (Activity) extra;
            Java.IO.File dir = activity.CacheDir;
            Java.IO.File assist = new Java.IO.File(dir, fileName + "." + extension);
            
            System.IO.File.WriteAllBytes(assist.AbsolutePath, inputBytes);

            Uri uri = FileProvider.GetUriForFile(activity, "it.pi3.informaticatrentina", assist);

            Intent intent = new Intent(Intent.ActionView, uri);
            intent.SetFlags(ActivityFlags.GrantReadUriPermission);

            var componentName = intent.ResolveActivity(activity.PackageManager);
            if (componentName != null)
            {
                activity.StartActivity(intent);
            }
            else
            {
                OpenPlayStoreAlertDialog(activity);
            }
        }

        private void OpenPlayStoreAlertDialog(Activity activity)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder (activity);
            alert.SetTitle (activity.GetString(Resource.String.alert_dialog_play_store_title));
            alert.SetMessage (activity.GetString(Resource.String.alert_dialog_play_store_content));
            alert.SetPositiveButton (activity.GetString(Resource.String.alert_dialog_play_store_ok), (senderAlert, args) => {
                OpenPlayStore(activity);
            });

            alert.SetNegativeButton (activity.GetString(Resource.String.alert_dialog_play_store_cancel), (senderAlert, args) => {
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void OpenPlayStore(Activity activity)
        {
            try
            {
                activity.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://")));
            }
            catch (Exception e)
            {
                activity.StartActivity(new Intent(Intent.ActionView, Uri.Parse("http://play.google.com/store")));
            }
        }
    }
    
    
}