using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocsMobile.VTDocsWSMobile;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;

namespace VTDocs.mobile.fe.pages
{
    public class GeneralPage<C> : System.Web.Mvc.ViewPage<C>
    {
        private NavigationHandler _navigationHandler = new NavigationHandler();
        private ConfigurationHandler _configurationHandler = new ConfigurationHandler();

        protected RuoloInfo RuoloInfo
        {
            get
            {
                return _navigationHandler.RuoloInfo;
            }
        }

        protected ViewType ViewType
        {
            get
            {
                return _navigationHandler.ViewType;
            }
        }

        protected string serialize(Object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream,obj);
            byte[] input = stream.ToArray();
            return Convert.ToBase64String(input);
        }

        protected string ResolveSkinUrl(string url)
        {
            string temp = "~/Content/" + _configurationHandler.SkinFolder + url;
            return ResolveUrl(temp);
        }

        protected string JQuery
        {
            get
            {
                return ResolveUrl("~/Scripts/jquery-1.4.2.min.js");
            }
        }

        protected string LoginImage
        {
            get
            {
                return ResolveSkinUrl(_configurationHandler.LoginImagePath);
            }
        }

        protected string IconImage
        {
            get
            {
                return ResolveSkinUrl(_configurationHandler.IconImagePath);
            }
        }

        protected string Titolo
        {
            get
            {
                return _configurationHandler.Titolo;
            }
        }

        protected string MobileVersion
        {
            get
            {
                return _configurationHandler.GetMobileVersion;
            }
        }
    }
}