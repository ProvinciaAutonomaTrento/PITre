using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Droid.Assign
{
    public class AssigneeUiHelper
    {
        public static void UpdateIcon(ImageView imageView, AbstractRecipient.RecipientType recipientType, bool isImageVisible = true)
        {
            int imageResId = -1;

            if (isImageVisible)
            {
                switch (recipientType)
                {
                    case AbstractRecipient.RecipientType.USER:
                        imageResId = Resource.Drawable.ic_persona;
                        break;
                    case AbstractRecipient.RecipientType.ROLE:
                        imageResId = Resource.Drawable.ic_ruolo;
                        break;
                    case AbstractRecipient.RecipientType.OFFICE:
                        imageResId = Resource.Drawable.ic_office;
                        break;
                }
            }

            if (imageResId > 0)
            {
                imageView.Visibility = ViewStates.Visible;
                imageView.SetImageResource(imageResId);
            }
            else
            {
                imageView.Visibility = ViewStates.Gone;
            }
        }
    }
}