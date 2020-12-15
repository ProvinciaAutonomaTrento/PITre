 using System;  
 using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Globalization;
using System.Web.Script.Serialization;

namespace VTDocs.mobile.fe.utils
{
    public static class CustomHtmlHelper
    {
        public static bool HasErrors(this HtmlHelper htmlHelper)
        {
            return !htmlHelper.ViewData.ModelState.IsValid;
        }

        public static MvcHtmlString GetJsonErrors(this HtmlHelper htmlHelper)
        {
            string res = "";
            foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    string errorMessage = modelError.ErrorMessage;
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        if (!string.IsNullOrEmpty(res)) res = res + "<br/>";
                        res=res+errorMessage;
                    }
                }
            }
            var errors = new
            {
                titolo = res,
                confirm = "OK"
            };
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return MvcHtmlString.Create(ser.Serialize(errors));
        }
    }
}
