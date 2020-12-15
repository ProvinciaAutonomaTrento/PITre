using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocs.mobile.fe.model;
using VTDocs.mobile.fe;
using VTDocsMobile.VTDocsWSMobile;
using log4net;
using System.IO;


namespace VTDocs.mobile.fe.controllers
{
    public class LoginController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(LoginController));

        [NoCache]
        public ActionResult Login()
        {
            try
            {
                //waking up the  BE, may give error..(channel security error)
                bool online = WSStub.beIsReady();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("beIsReady gave error {0} :{1} mabee a wakeup error", e.Message, e.StackTrace); 
            }

            ViewData["WAPath"] = Request.Url.ToString().Replace(Request.AppRelativeCurrentExecutionFilePath.Replace("~", ""), "");
            string images ="";
            List<String> lstImages = ListImages();

            if (lstImages.Count == 0)
            {
                ViewData["Images"] = string.Empty;
                return View();
            }

            foreach (string a in lstImages)
                images += "\""+ a+"\",";


            images += "§#§"; //Il mitico carattere di galasso!!!!
            images =  images.Replace (",§#§","");
            ViewData["Images"] = images;
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult Login(LoginModel model)
        {
            logger.Info("begin");
            try
            {
                //waking up the  BE, may give error..(channel security error)
                bool online = WSStub.beIsReady();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("beIsReady gave error {0} :{1} mabee a wakeup error", e.Message, e.StackTrace);
            }
             if (ModelState.IsValid)
            {
                string username = model.UserName;
                string password = model.Password;
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserName = username;
                loginRequest.Password = password;
                try
                {
                    LoginResponse loginResponse = WSStub.login(loginRequest);
                    if (loginResponse.Code == LoginResponseCode.OK)
                    {
                        logger.Info("login success");
                        NavigationHandler.clearSession();
                        NavigationHandler.LoggedInfo = loginResponse.UserInfo;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        logger.Info("error in login");
                        ModelState.AddModelError("", LoginValidation.ErrorCodeToString(loginResponse.Code));
                    }
                }
                catch (Exception e)
                {
                    logger.Info("exception: "+e);
                    ModelState.AddModelError("", LoginValidation.ErrorCodeToString(LoginResponseCode.SYSTEM_ERROR));
                }
            }
            logger.Info("end");
            return View(model);
        }

        private List<String> ListImages()
        {
            logger.Info("begin");
            string idDevice = System.Configuration.ConfigurationManager.AppSettings["IdDevice"];
            string skin = System.Configuration.ConfigurationManager.AppSettings["Skin"];
            string appPath = Request.PhysicalApplicationPath;
            string device = string.Empty;
            
            //USERGAGENT
            string userAgent=Request.UserAgent.ToLower();
            if (userAgent.Contains("ipad") ||
                userAgent.Contains("gt-p51") ||
                userAgent.Contains("nexus 7") ||
                  userAgent.ToUpper().Contains("GT-P7500") ||
                    userAgent.ToUpper().Contains("GT P7500") ||
                idDevice == "I")
                    device = "Ipad\\";

            if (userAgent.Contains ("sch-i800") || 
                userAgent.Contains ("gt-p1000") ||
                userAgent.Contains ("gt-p31") ||
                userAgent.Contains ("gt-i93") ||
                idDevice == "G")
                    device = "Galaxy\\";

            string filePathSkined = String.Format("{0}content\\{1}\\img\\{2}", appPath, skin,device);
            string filePath = String.Format("{0}content\\img\\{1}", appPath,device);

            List<string> tmpImgs = GetFilesRecursive(filePathSkined);
            tmpImgs.AddRange ( GetFilesRecursive(filePath));
            List<string> imgs = new List<string>();
            List<string> validExtensions = new List<string> { ".jpg", ".png", ".gif" };

            string WAPath = Request.Url.ToString().Replace (Request.AppRelativeCurrentExecutionFilePath.Replace ("~",""),"");
            
            foreach (string i in tmpImgs)
            {
                if(validExtensions.Contains (Path.GetExtension(i)))
                    imgs.Add(WAPath+(i.Replace(appPath,"/").Replace("\\", "/")));
                
            }

            
            logger.Info("end");
            return imgs;
        }


        private static List<string> GetFilesRecursive(string b)
        {
            // 1.
            // Store results in the file results list.
            List<string> result = new List<string>();

            // 2.
            // Store a stack of our directories.
            Stack<string> stack = new Stack<string>();

            // 3.
            // Add initial directory.
            stack.Push(b);

            // 4.
            // Continue while there are directories to process
            while (stack.Count > 0)
            {
                // A.
                // Get top directory
                string dir = stack.Pop();

                try
                {
                    // B
                    // Add all files at this directory to the result List.
                    result.AddRange(Directory.GetFiles(dir, "*.*"));

                    // C
                    // Add all directories at this directory.
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }
            return result;
        }

    }
}
