using System;
using System.Collections;

namespace NttDataWA.Utils
{

    public class TypeIcon : Hashtable
    {

        private static TypeIcon theHash;

        private static TypeIcon theHashBig;

        private static TypeIcon theHashSmall;

        private TypeIcon() { }

        public static TypeIcon getInstance()
        {
            try
            {
                if (theHash != null) return theHash;

                theHash = new TypeIcon();
                theHash.Add("doc", "icon_word.png");
                theHash.Add("pdf", "icon_PDF.png");
                theHash.Add("xls", "icon_excel.png");
                theHash.Add("ppt", "icon_ppt.png");
                theHash.Add("rtf", "icon_wri.png");
                theHash.Add("zip", "icon_zip.png");
                theHash.Add("txt", "icon_txt.png");
                theHash.Add("htm", "icon_htm.png");
                theHash.Add("html", "icon_htm.png");
                theHash.Add("p7m", "icon_p7m.png");
                theHash.Add("p7x", "icon_p7m.png");
                theHash.Add("tif", "icon_tif.png");
                theHash.Add("tiff", "icon_tif.png");
                theHash.Add("jpg", "icon_jpg.png");
                theHash.Add("png", "icon_jpg.png");
                theHash.Add("sxw", "icon_sxw.png");
                theHash.Add("odt", "icon_odt.png");

                return theHash;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TypeIcon getInstanceBig()
        {
            try
            {
                if (theHashBig != null) return theHashBig;

                theHashBig = new TypeIcon();
                theHashBig.Add("doc", "big_doc3.png");
                theHashBig.Add("pdf", "big_pdf.png");
                theHashBig.Add("xls", "big_xls.png");
                theHashBig.Add("ppt", "big_ppt.png");
                theHashBig.Add("rtf", "big_wri.png");
                theHashBig.Add("zip", "big_zip.png");
                theHashBig.Add("txt", "big_txt.png");
                theHashBig.Add("htm", "big_html.png");
                theHashBig.Add("html", "big_html.png");
                theHashBig.Add("p7m", "big_p7m.png");
                theHashBig.Add("p7x", "big_p7m.png");
                theHashBig.Add("tif", "big_tif.png");
                theHashBig.Add("tiff", "big_tif.png");
                theHashBig.Add("jpg", "big_jpg.png");
                theHashBig.Add("png", "big_jpg.png");
                theHashBig.Add("sxw", "big_sxw.png");
                theHashBig.Add("odt", "big_odt.png");

                return theHashBig;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TypeIcon getInstanceSmall()
        {
            try
            {
                if (theHashSmall != null) return theHashSmall;

                theHashSmall = new TypeIcon();
                theHashSmall.Add("doc", "small_doc3.png");
                theHashSmall.Add("pdf", "small_pdf.png");
                theHashSmall.Add("xls", "small_xls.png");
                theHashSmall.Add("ppt", "small_ppt.png");
                theHashSmall.Add("rtf", "small_wri.png");
                theHashSmall.Add("zip", "small_zip.png");
                theHashSmall.Add("txt", "small_txt.png");
                theHashSmall.Add("htm", "small_html.png");
                theHashSmall.Add("html", "small_html.png");
                theHashSmall.Add("p7m", "small_p7m.png");
                theHashSmall.Add("p7x", "small_p7m.png");
                theHashSmall.Add("tif", "small_tif.png");
                theHashSmall.Add("tiff", "small_tif.png");
                theHashSmall.Add("jpg", "small_jpg.png");
                theHashSmall.Add("png", "small_jpg.png");
                theHashSmall.Add("sxw", "small_sxw.png");
                theHashSmall.Add("odt", "small_odt.png");

                return theHashSmall;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

    }

}