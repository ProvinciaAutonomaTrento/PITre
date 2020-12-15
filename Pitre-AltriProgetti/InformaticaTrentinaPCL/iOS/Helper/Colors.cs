using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class Colors
    {
        public Colors()
        {
        }

       

        public static UIColor FOOTER_SEPARATOR = UIColor.FromRGB(241, 241, 241);
        public static UIColor COLOR_SEARCH = UIColor.FromRGB(163, 163, 163);
        public static UIColor COLOR_SEARCH_FOOTER = UIColor.FromRGB(221, 221, 221);

        public static UIColor COLOR_TEXT_HEADER_DOCUMENT = UIColor.FromRGB(125, 125, 125); // colore data e nome 
        public static UIColor COLOR_TEXT_ACTION_DOCUMENT = UIColor.FromRGB(25, 69, 161); // colore azioni, accetta, accetta e trasmetti ecc
        public static UIColor COLOR_TEXT_TYPE_DOCUMENT = UIColor.FromRGB(33, 188, 211); // uscita entrata documento


        public static UIColor COLOR_BUTTON_DEFAULT = UIColor.FromRGB(24, 68, 161);
        public static UIColor COLOR_BUTTON_DISABLED_DEFAULT = UIColor.FromRGB(229, 229, 229);
        public static UIColor COLOR_NAVIGATION = UIColor.FromRGB(24, 68, 161);

        public static UIColor COLOR_BLUE_TEXT_ROW_COLOR = UIColor.FromRGB(24, 68, 161);

        public static UIColor COLOR_GREEN_SIGN = UIColor.FromRGB(20, 164, 91);
        public static UIColor COLOR_RED_REJECT = UIColor.FromRGB(232, 9, 12);

        public static UIColor COLOR_BUTTON_FOOTER_DOCUMENT_NOT_ENABLE = UIColor.LightGray;
        public static UIColor COLOR_BUTTON_FOOTER_DOCUMENT_ENABLE = UIColor.FromRGB(24, 68, 161);


        public static UIColor COLOR_background_alert = UIColor.FromRGB(243, 243, 243);
        public static UIColor COLOR_BUTTON_CLOSE_background = UIColor.FromRGB(255, 255, 255);


        public static UIColor COLOR_SWIPE_BUTTON_TABLE_DOCUMENT = UIColor.FromRGB(38, 50, 56);

        public static UIColor COLOR_RED_DELEGATE_SWIPE = UIColor.FromRGB(239, 83, 80);

        public static UIColor COLOR_ATTACHMENT_LINE_BACKGROUND = UIColor.FromRGB(245, 146, 58);

        public static UIColor FromHex(string hexValue, float alpha = 1.0f)
        {
            float red, green, blue;
            var colorString = hexValue.Replace("#", "");

            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

            }
        }

        public static UIColor MASTER_LANDSCAPE = Colors.FromHex("#f1f1f1", 1);
        public static UIColor MASTER_PORTRAIT = UIColor.White;

        public static UIColor SELECTED_DOCUMENT_OPEN_SECTION_BACKGROUND = UIColor.FromRGB(13, 71, 161);
    }
}
