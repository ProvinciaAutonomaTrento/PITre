using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.model;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using VTDocs.mobile.fe.converters;

namespace VTDocs.mobile.fe.usercontrols
{
    public class GeneralUserControl : System.Web.Mvc.ViewUserControl<MainModel>
    {
        public MvcHtmlString MainModel
        {
            get
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.RegisterConverters(ConverterUtils.JavaScriptConverters);
                return MvcHtmlString.Create(ser.Serialize(Model));
            }
        }

        public MvcHtmlString WAContext
        {
            get
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ConfigurationHandler configHandler = new ConfigurationHandler();
                ser.RegisterConverters(ConverterUtils.JavaScriptConverters);
                string WAPath = ResolveUrl("~");
                var context = new
                {
                    WAPath = WAPath.Substring(0, WAPath.Length - 1),
                    SkinFolder = configHandler.SkinFolder
                };
                return MvcHtmlString.Create(ser.Serialize(context));
            }

        }
    }
}