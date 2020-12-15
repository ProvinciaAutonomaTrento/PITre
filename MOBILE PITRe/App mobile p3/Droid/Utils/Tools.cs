using System;
using System.Collections.Generic;
using Android.OS;
using Android.Text;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class Tools
    {
        public static ISpanned GetSpannedString(string text)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(text, FromHtmlOptions.ModeCompact);
            }
            else
            {
                return Html.FromHtml(text);
            }
        }
         public static Bundle ConvertDictionaryInBundle(Dictionary<string, string> dictionary)
        {
            Bundle bundle = new Bundle();
            foreach (var item in dictionary)
            {
                bundle.PutString(item.Key, item.Value);
            }
            return bundle;
        }
    }
}
