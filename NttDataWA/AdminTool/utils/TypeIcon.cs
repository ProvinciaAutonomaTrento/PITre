using System;
using System.Collections;

namespace SAAdminTool {

	public class TypeIcon : Hashtable {
		
		private static TypeIcon theHash;

		private TypeIcon() {}

		public static TypeIcon getInstance() {
			if (theHash != null) 
				return theHash;

			theHash = new TypeIcon();
			theHash.Add("doc","icon_word.gif");
			theHash.Add("pdf","icon_PDF.gif");
			theHash.Add("xls","icon_excel.gif");
			theHash.Add("ppt","icon_ppt.gif");
			theHash.Add("rtf","icon_wri.gif");
			theHash.Add("zip","icon_zip.gif");
			theHash.Add("txt","icon_txt.gif");
			theHash.Add("htm","icon_htm.gif");
			theHash.Add("html","icon_htm.gif");
			theHash.Add("p7m","icon_p7m.gif");
			theHash.Add("p7x","icon_p7m.gif");
			theHash.Add("tif","icon_tif.gif");
			theHash.Add("tiff","icon_tif.gif");
			theHash.Add("jpg","icon_jpg.gif");
            theHash.Add("sxw", "icon_sxw.gif");
            theHash.Add("odt", "icon_odt.gif");

			return theHash;
		}
	}
}
