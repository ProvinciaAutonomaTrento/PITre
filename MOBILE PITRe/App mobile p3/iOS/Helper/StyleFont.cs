using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class StyleFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:InformaticaTrentinaPCL.iOS.Helper.StyleFont"/> class.
        /// </summary>
        /// <param name="nameFamily">Name family.</param>
        /// <param name="sizeText">Size text.  in pt </param>
        /// <param name="colorHex">Color hex.</param>
        /// <param name="alignment">Alignment.</param>
        /// <param name="dimension">Dimension.</param>
        public StyleFont(string nameFamily,nfloat sizeText, string colorHex,UITextAlignment alignment,string dimension)
        {
            this.family = nameFamily;
            sizeText -= Utility.IsTablet()?0:3;
            this.size = sizeText * 1.33f;
            this.color = Colors.FromHex(colorHex);
            this.align = alignment;
            this.dimension = dimension;
        }

        public nfloat size = 0;
        public string family = "";
        public UIColor color = UIColor.Black;
        public string dimension = "";
        public UITextAlignment align = UITextAlignment.Natural;

	}
}
