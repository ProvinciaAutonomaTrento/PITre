using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Widget;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    /// <summary>
    /// Classe utilizzata per customizzazioni UI in base alla versione di Android installata sul device.
    /// </summary>
    public class UIUtility
    {
        /// <summary>
        /// Metodo che restituisce l'oggetto Color in base al resourceId passato come parametro.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="context">Context.</param>
        /// <param name="resourceId">Resource identifier.</param>
        public static Color GetColor(Context context, int resourceId)
        {
            Color color;
			if ((int)Build.VERSION.SdkInt < 23)
#pragma warning disable CS0618 // Type or member is obsolete
                color = context.Resources.GetColor(resourceId);
#pragma warning restore CS0618 // Type or member is obsolete
			else
				color = context.Resources.GetColor(resourceId, null);

            return color;
		}

        /// <summary>
        /// Metodo che setta lo style (parametro) alla textview (parametro).
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="textview">Textview.</param>
        /// <param name="styleId">Style identifier.</param>
        public static void setTextAppearance(Context context, TextView textview, int styleId)
        {
            if ((int)Build.VERSION.SdkInt < 23)
#pragma warning disable CS0618 // Type or member is obsolete
                textview.SetTextAppearance(context, styleId);
#pragma warning restore CS0618 // Type or member is obsolete
            else
                textview.SetTextAppearance(styleId);

        }
        
        public static void setTitleSubtitle(TextView target, 
                                            string title, string subtitle, 
                                            int titleStyleId = Resource.Style.list_document_name, 
                                            int subtitleStyleId = Resource.Style.list_sender)
        {
            if (target == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(subtitle))
            {
                target.Text = title;
                setTextAppearance(target.Context, target, titleStyleId);
            }
            else
            {
                SpannableString text = new SpannableString(title + "\n" + subtitle);
                text.SetSpan(new TextAppearanceSpan(target.Context, titleStyleId), 0, title.Length, SpanTypes.ExclusiveExclusive );
                text.SetSpan(new TextAppearanceSpan(target.Context, subtitleStyleId), title.Length + 1, text.Length(), SpanTypes.ExclusiveExclusive);
                target.SetText(text, TextView.BufferType.Spannable);    
                
            }
        }

        public static void SetTextColor(TextView textView, int colorResId)
        {
            Color color = textView.Context.Resources.GetColor(colorResId);
            textView.SetTextColor(color);
        }
    }
}
