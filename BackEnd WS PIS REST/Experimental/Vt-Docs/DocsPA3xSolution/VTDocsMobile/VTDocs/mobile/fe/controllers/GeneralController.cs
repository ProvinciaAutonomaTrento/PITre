using System.Web.Mvc;
using VTDocs.mobile.fe;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.model;
using System.Web.Script.Serialization;
using System;
using System.Web;
using System.Collections.Generic;
using VTDocs.mobile.fe.utils;
using System.Text;
using VTDocs.mobile.fe.commands.model;
using VTDocs.mobile.fe.commands;
using log4net;
using VTDocs.mobile.fe.converters;

namespace VTDocs.mobile.fe.controllers
{
    public class GeneralController : Controller
    {
        private List<JavaScriptConverter> _converters = ConverterUtils.JavaScriptConverters;
        private NavigationHandler _navigationHandler = new NavigationHandler();

        public GeneralController()
        {
           LogicalThreadContext.Properties["UserId"] = NavigationHandler.UserId;
        }

        public NavigationHandler NavigationHandler
        {
            get { return _navigationHandler; }
        }

        public VTDocsWSMobileClient WSStub
        {
            get {
                return new VTDocsWSMobileClient();
            }
        }

        protected JsonResult CommandExecute(Command<MainModel> command){
            return JsonWithConverters(command.Execute());
        }

        protected virtual JsonResult JsonWithConverters(object data){
            return JsonWithConverters(data,null,null);
        }

        protected virtual JsonResult JsonWithConverters(object data, string contentType, Encoding contentEncoding)
        {
            return new JsonResultWithConverters
                       {
                           Data = data,
                           ContentType = contentType,
                           ContentEncoding = contentEncoding,
                           Converters = _converters
                       };
        }

        private class JsonResultWithConverters : JsonResult
        {
            private ILog logger = LogManager.GetLogger(typeof(JsonResultWithConverters));

            public IList<JavaScriptConverter> Converters
            {
                get;
                set;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
                if (ContentEncoding != null)
                {
                    response.ContentEncoding = ContentEncoding;
                }

                if (Data != null)
                {
                    JavaScriptSerializer serializer = CreateJsonSerializer();
                    string serialized = serializer.Serialize(Data);
                    logger.Debug("serialized data: " + serialized);
                    response.Write(serialized);
                }
            }

            private JavaScriptSerializer CreateJsonSerializer()
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(Converters);
                return serializer;
            }

        }
    }

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }

}