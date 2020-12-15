using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    
    /// <summary>
    ///  Nella classe ci sono tutti gli stili di font utilizzati sull app . Inoltre calcola ed imposta la proporzione
    ///  per ogni label.
    /// </summary>
    public class Font
    {
        public Font()
        {
        }

        /*   Name Font utilizzato
         * 
         *   Source Sans Pro: 
             SourceSansPro-ExtraLight, 
             SourceSansPro-SemiBold", 
			 SourceSansPro-Bold, 
			 SourceSansPro-Italic, 
			 SourceSansPro-Regular", 
			 SourceSansPro-ExtraLightItalic, 
			 SourceSansPro-Light, 
			 SourceSansPro-SemiBold"Italic, 
			 SourceSansPro-LightItalic, 
			 SourceSansPro-BlackItalic, 
         * 
         * 
         * */


#region photoshop size diminuita di 3 rispetto photoshop

        public static StyleFont LABEL_DATA = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 12 : 15, "#333333", UITextAlignment.Left, "");


        public static StyleFont HEADER_TABLE = new StyleFont("SourceSansPro-Bold",Utility.IsTablet()?9:12, "#333333", UITextAlignment.Natural, "UPPERCASE");
        public static StyleFont ROW_TABLE_TITLE_BLUE = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 13 : 16, "#0d47a1", UITextAlignment.Left, "");

        public static StyleFont ROW_TABLE_SUB_DESC_BLACK = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 19 : 22, "#333333", UITextAlignment.Left, "");


        public static StyleFont ROW_TABLE_SUB_TITLE_BLACK = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 13 : 16, "#333333", UITextAlignment.Left, "");
        public static StyleFont ROW_TABLE_TITLE_BLACK = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 15 : 18, "#333333", UITextAlignment.Left, "");

        public static StyleFont TITLE_BLACK = new StyleFont("SourceSansPro-SemiBold", Utility.IsTablet() ? 15 : 18, "#333333", UITextAlignment.Left, "UPPERCASE");

        public static StyleFont ROW_TABLE_DESC_UPPERCASE = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 19 : 22, "#333333", UITextAlignment.Left,"UPPERCASE");
        public static StyleFont ROW_TABLE_DESC_LOWERCASE = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 19 : 22, "#333333", UITextAlignment.Left, "");

        public static StyleFont LINK_TITLE_BUTTON_BLUE = new StyleFont("SourceSansPro-SemiBold", Utility.IsTablet() ? 13 : 16, "#0d47a1", UITextAlignment.Left, "UPPERCASE");
        public static StyleFont LINK_TITLE_BUTTON_GRAY = new StyleFont("SourceSansPro-Regular", Utility.IsTablet() ? 9 : 12, "#adadad", UITextAlignment.Left, "");


#endregion

        public static StyleFont PAGE_TITLE = new StyleFont("SourceSansPro-Regular", 20, "#ffffff", UITextAlignment.Center, "UPPERCASE");
        public static StyleFont PAGE_TITLE_BLUE = new StyleFont("SourceSansPro-Regular", 20, "#0d47a1", UITextAlignment.Center, "UPPERCASE");
        public static StyleFont SECTION_TITLE = new StyleFont("SourceSansPro-Regular" , 20, "#ffffff", UITextAlignment.Center,"");
        public static StyleFont MODALE_TITLE = new StyleFont("SourceSansPro-SemiBold", 18, "#333333",UITextAlignment.Natural,"UPPERCASE");
        public static StyleFont LINK_TO_PAGE_SECTION = new StyleFont("SourceSansPro-Regular", 16, "#0d47a1", UITextAlignment.Natural, "");
        public static StyleFont CONFIRM_PAGE = new StyleFont("SourceSansPro-SemiBold", 32 , "#333333", UITextAlignment.Natural, "");
        public static StyleFont ENABLED_BUTTONS = new StyleFont("SourceSansPro-SemiBold", 16 , "#ffffff", UITextAlignment.Natural, "");
        public static StyleFont DISABLED_BUTTONS = new StyleFont("SourceSansPro-SemiBold", 16 , "#b6b6b6", UITextAlignment.Natural, "");
		public static StyleFont INPUT_TEXT_LABEL = new StyleFont("SourceSansPro-Regular", 16 , "#adadad", UITextAlignment.Natural, "");
		public static StyleFont INPUT_TEXT = new StyleFont("SourceSansPro-Regular", 16 , "#333333", UITextAlignment.Natural, "");

        // SPECIAL LINK
		public static StyleFont LINK_PASSWORD = new StyleFont("SourceSansPro-SemiBold", 12 , "#0d47a1",UITextAlignment.Natural, "");
		public static StyleFont LINK_NRP = new StyleFont("SourceSansPro-Regular", 12 , "#adadad", UITextAlignment.Natural, "");   // non ricordi il pin 
		public static StyleFont LINK_ROT = new StyleFont("SourceSansPro-Bold", 16 , "#0d47a1",UITextAlignment.Natural,"");  // richiedi otp


		// LIST
		public static StyleFont SENDER = new StyleFont("SourceSansPro-Regular", 12 , "#333333", UITextAlignment.Natural, "");
		public static StyleFont DOCUMENT_NAME_1 = new StyleFont("SourceSansPro-Regular", 16 , "#333333", UITextAlignment.Natural, "");
		public static StyleFont DESCRIPION = new StyleFont("SourceSansPro-SemiBold", 12 , "#333333", UITextAlignment.Natural, "");

		public static StyleFont ICON_DESCRIPION = new StyleFont("SourceSansPro-Bold", 12, "#00bcd4", UITextAlignment.Natural, "");


		// DETAIL PAGE
		public static StyleFont LABEL = new StyleFont("SourceSansPro-Bold", 12 , "#333333",UITextAlignment.Natural,"");
		public static StyleFont DOCUMENT_NAME_2 = new StyleFont("SourceSansPro-Regular", 22 , "#333333", UITextAlignment.Natural, "");
		public static StyleFont DETAILS = new StyleFont("SourceSansPro-Regular", 16, "#333333", UITextAlignment.Natural, "");
		public static StyleFont INPUT = new StyleFont("SourceSansPro-Regular", 14, "#333333", UITextAlignment.Natural, "");



        private static nfloat Size(StyleFont nameStyle)
        {
            var intsizeFont = nameStyle.size * (int)Utility.getScreenWidth() / (int)Utility.getOriginalScreenWidth();
            if (!Utility.IsTablet())
                intsizeFont+=2;
            
            return intsizeFont;
        }

        /// <summary>
        /// Sets the custom style font.
        /// </summary>
        /// <returns> setta il font desiderato presente nella classe </returns>
        /// <param name="nameStyle"> 1 parametro label , il 2 parametro StyleFont</param>
        public static void SetCustomStyleFont(UILabel label, StyleFont nameStyle, UITextAlignment costomFont = UITextAlignment.Natural , bool upper = true)
        {
            var fontnew = UIFont.FromName(nameStyle.family, Size(nameStyle));

                label.Font = fontnew;
                label.TextColor = nameStyle.color;
            label.TextAlignment = costomFont==UITextAlignment.Natural?nameStyle.align:costomFont;
               if (nameStyle.dimension.Contains("UPPERCASE"))
                if (upper)label.Text = label.Text.ToUpper();
        }

     

        /// <summary>
        /// Sets the custom style font.
        /// </summary>
        /// <returns> setta il font desiderato presente nella classe </returns>
        /// <param name="nameStyle"> 1 parametro UITextField , il 2 parametro StyleFont</param>
        public static void SetCustomStyleFont(UITextView label, StyleFont nameStyle, UITextAlignment costomFont = UITextAlignment.Natural)
		{
            var fontnew = UIFont.FromName(nameStyle.family, Size(nameStyle));
			label.Font = fontnew;
			label.TextColor = nameStyle.color;
			label.TextAlignment = costomFont == UITextAlignment.Natural ? nameStyle.align : costomFont;
			if (nameStyle.dimension.Contains("UPPERCASE"))
				label.Text = label.Text.ToUpper();
		}

		/// <summary>
		/// Sets the custom style font.
		/// </summary>
		/// <returns> setta il font desiderato presente nella classe </returns>
		/// <param name="nameStyle"> 1 parametro UITextField , il 2 parametro StyleFont</param>
		public static void SetCustomStyleFont(UITextField label, StyleFont nameStyle, UITextAlignment costomFont = UITextAlignment.Natural)
		{
            var fontnew = UIFont.FromName(nameStyle.family, Size(nameStyle));
			label.Font = fontnew;
			label.TextColor = nameStyle.color;
			label.TextAlignment = costomFont == UITextAlignment.Natural ? nameStyle.align : costomFont;
			if (nameStyle.dimension.Contains("UPPERCASE"))
				label.Text = label.Text.ToUpper();
		}

		/// <summary>
		/// Sets the custom style font.
		/// </summary>
		/// <returns> setta il font desiderato presente nella classe </returns>
		/// <param name="nameStyle"> 1 parametro UIButton , il 2 parametro StyleFont</param>
		public static void SetCustomStyleFont(UIButton button, StyleFont nameStyle)
		{
            var fontnew = UIFont.FromName(nameStyle.family, Size(nameStyle));
			button.Font = fontnew;
        	button.SetTitleColor(nameStyle.color, UIControlState.Normal);
			if (nameStyle.dimension.Contains("UPPERCASE"))
				button.SetTitle(button.Title(UIControlState.Normal).ToUpper(), UIControlState.Normal);
		}

		/// <summary>
		/// Ti permette di vedere che font sono installati sull applicazione. Quindi vedere come si chiama lo stile e chiamarlo di conseguenza
		/// </summary>
		public static void GetInfoFamilyStypeFromApp()
		{
			foreach (string family in UIFont.FamilyNames)
			{
                Console.WriteLine(family + ": ");
                    
                foreach (string name in UIFont.FontNamesForFamilyName(family))
					  Console.WriteLine(name + ", ");
			}
		}

	}
}
